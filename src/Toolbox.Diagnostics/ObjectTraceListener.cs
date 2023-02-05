using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace Toolbox.Diagnostics
{
    /// <summary>
    /// Base class for <see cref="TraceListener"/> with object support
    /// </summary>
    public abstract class ObjectTraceListener : TraceListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectTraceListener"/> class.
        /// </summary>
        public ObjectTraceListener() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectTraceListener"/> class.
        /// </summary>        
        public ObjectTraceListener(string? name)
            : base(name) 
        {
            RegisterConverter<TraceConverterString>();
            RegisterConverter<TraceConverterValueType>();
            RegisterConverter<TraceConverterSecureString>();

            ObjectConverter = new TraceConverterObject(this);
            EnumerableConverter = new TraceConverterEnumerable(this);

            MaxCollectionCount = 20;
        }

        /// <inheritdoc/>
        public override bool IsThreadSafe => true;

        private Queue<TraceItem> Items { get; } = new Queue<TraceItem>();

        protected T? GetAttribute<T>(string key)
        {
            var value = Attributes[key];

            if (value == null) return default;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        protected void SetAttribute<T>(string key, T value)
        {
            if (value == null)
                Attributes.Remove(key);
            else
                Attributes[key] = Convert.ToString(value);
        }

        protected override string[] GetSupportedAttributes()
        {
            var names = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(p => p.GetCustomAttribute<SupportedAttributeAttribute>(true)?.Name)
                            .Where(n => n != null);

            return (base.GetSupportedAttributes()?.Concat(names) ?? names)
                        .OfType<string>().ToArray();
        }

        #region Append
        private const string AttributeMaxCollectionCount = "maxCollectionCount";
        /// <summary>
        /// Gets or sets the length of elements trace from a collection.
        /// </summary>
        /// <remarks>
        /// This property can be set from the configuration file with the attribute 'maxCollectionCount'.
        /// </remarks>        
        [SupportedAttribute(AttributeMaxCollectionCount)]
        public int MaxCollectionCount
        {
            get => GetAttribute<int>(AttributeMaxCollectionCount);
            set => SetAttribute(AttributeMaxCollectionCount, value);
        }
        #endregion

        protected void Enqueue(TraceItem item)
        {
            lock (Items) 
            { 
                Items.Enqueue(item); 
            }

            if (OutputWorker == null)
            {
                OutputWorker = new Thread(DoOutput)
                {
                    IsBackground = true,
                    Name = $"{GetType().Name}.{Name}.Output",
                    Priority = ThreadPriority.Lowest
                };
                OutputRunning = true;
                OutputWorker.Start();
            }
            OutputWait.Set();
        }

        abstract protected void Write(TraceItem item);

        protected virtual void Init()
        {
        }

        #region OutputWorker
        private Thread? OutputWorker { get; set; }
        private bool OutputRunning { get; set; }
        private AutoResetEvent OutputWait { get; } = new AutoResetEvent(false);
        private void DoOutput()
        {
            Init();

            do
            {
                TraceItem? item = null;

                lock (Items)
                {
                    if (Items.Count > 0)
                        item = Items.Dequeue();
                }
                if (item != null)
                {
                    Write(item);
                }
                else if (OutputRunning)
                {
                    OutputWait.WaitOne(1000);
                }
            }
            while (OutputRunning || Items.Count > 0);
        }
        #endregion

        /// <inheritdoc/>
        public override void Flush()
        {
            base.Flush();
            OutputWait.Set();
        }

        /// <inheritdoc/>
        public override void Close()
        {
            OutputRunning = false;
            OutputWait.Set();
            OutputWorker?.Join();
            OutputWorker = null;

            Flush();

            base.Close();
        }
        private static StackFrame[] GetFrames()
        {
            var frames = new StackTrace(2, true).GetFrames()
                .SkipWhile(f => "System.Diagnostics" == f.GetMethod()?.DeclaringType?.Namespace || typeof(ObjectTraceListener).IsAssignableFrom(f.GetMethod()?.DeclaringType))
                .ToArray();
            return frames;
        }

        /// <inheritdoc/>
        public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? format, params object?[]? args)
        {
            var frames = GetFrames();

            Enqueue(
                new TraceItem(                    
                    source,
                    eventType,
                    id,
                    Environment.CurrentManagedThreadId,
                    Environment.ProcessId,
                    string.Format(format ?? "", args ?? Array.Empty<object>()),
                    frames,
                    Array.Empty<TraceCapture>()
                ));
        }

        /// <inheritdoc/>
        public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id)
        {
            TraceEvent(eventCache, source, eventType, id, null);
        }

        /// <inheritdoc/>
        public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? message)
        {
            TraceEvent(eventCache, source, eventType, id, message, null);
        }

        /// <inheritdoc/>
        public override void TraceData(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, object? data)
        {
            TraceData(eventCache, source, eventType, id, new[] { data });
        }

        /// <inheritdoc/>
        public override void TraceData(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, params object?[]? data)
        {
            var frames = GetFrames();

            var captures = data?.Aggregate(
                new List<TraceCapture>(),
                (list, obj) =>
                {
                    var capture = obj != null
                                    ? GetConverter(obj).CaptureCore(obj)
                                    : new TraceCapture { Text = "<null>" };

                    capture.Name = $"[{list.Count}]";
                    list.Add(capture);

                    return list;
                }).ToArray() 
                ?? Array.Empty<TraceCapture>();

            Enqueue(
                new TraceItem(
                    source,
                    eventType,
                    id,
                    Environment.CurrentManagedThreadId,
                    Environment.ProcessId,
                    data == null ? "no objects" : $"{data.Length} object(s)",
                    frames,
                    captures
                ));
        }

        public override void Write(string? message)
        {
            TraceEvent(null, "", TraceEventType.Information, 0, message);
        }

        public override void WriteLine(string? message)
        {
            Write(message + Environment.NewLine);
        }

        private Dictionary<Type, TraceConverterBase> TraceConverters { get; } = new Dictionary<Type, TraceConverterBase>();
        public void RegisterConverter<T>() where T : TraceConverterBase
        {
            var converter = (T)(Activator.CreateInstance(typeof(T), this)
                            ?? throw new InvalidOperationException($"Construction of {typeof(T)} failed."));

            TraceConverters[converter.ConvertType] = converter;
        }

        private TraceConverterObject ObjectConverter { get; }
        private TraceConverterEnumerable EnumerableConverter { get; }

        internal TraceConverterBase GetConverter(object obj)
        {
            var converter = GetConverter(obj.GetType());

            if (converter == null)
            {
                if (obj is IEnumerable)
                    return EnumerableConverter;

                return ObjectConverter;
            }

            return converter;
        }

        internal TraceConverterBase? GetConverter(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!TraceConverters.TryGetValue(type, out var converter))
            {
                var attribute = type.GetCustomAttribute<TraceConverterAttribute>();
                if (attribute != null)
                {
                    converter = attribute.CreateConverter(type);
                    if (converter != null)
                    {
                        converter.Listener = this;
                        TraceConverters[type] = converter;
                    }
                }
            }

            if (type.BaseType == null)
            {
                return null;
            }

            return converter ?? GetConverter(type.BaseType);
        }
    }
}