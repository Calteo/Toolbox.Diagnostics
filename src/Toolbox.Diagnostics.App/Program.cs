using System.Diagnostics;
using System.Security;
using Toolbox.CommandLine;

namespace Toolbox.Diagnostics.App
{
    internal class Program
    {
        static int Main(string[] args)
        {
			try
			{
				var parser = new Parser(typeof(ConsoleOptions));
				var result = parser.Parse(args);

				var rc = result
							.OnError(OnError)
							.OnHelp(OnHelp)
							.On<ConsoleOptions>(OnConsole)
							.Return;

				return rc;
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.ToString());
				return 3;
			}
        }

        public static TraceSource CreateSource()
        {
            return new TraceSource("TestApp", SourceLevels.All);
        }

        private static int OnConsole(ConsoleOptions arg)
		{
            var source = CreateSource();
            
            source.Listeners.Add(new ObjectTraceTextWriterListener("object", Console.Out));

            source.TraceInformation("simple information");

            source.TraceData(TraceEventType.Information, 42, "Hello");

            var data = new SimpleData { Name = "Bob", Number = 678798798 };

            source.TraceData(TraceEventType.Warning, 43, "Something important", data);
            source.TraceData(TraceEventType.Error, 44, "no data given", null);
            source.TraceData(TraceEventType.Error, 45, "array of data", new[] { data, data });
            source.TraceData(TraceEventType.Error, 46, "List of data", new List<SimpleData> { data, data });
            source.TraceData(TraceEventType.Error, 47, "List of string", new List<string> { "One", "Two", "Three" });

            var list = new List<int>();
            var random = new Random(47);
            for (int i = 0; i < 100; i++)
            {
                list.Add(random.Next(10000));
            }
            source.TraceData(TraceEventType.Information, 48, list);

            var password = new SecureString();
            foreach (var c in "passW0rd")
            {
                password.AppendChar(c);
            }

            source.TraceData(TraceEventType.Critical, 49, "some password", password);

            var root = new Node();
            root.Children.AddRange(new[] { new Node(root), new Node(root) });

            source.TraceData(TraceEventType.Transfer, 50, root);

            var method = typeof(Program).GetMethod(nameof(OnHelp), System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);

            source.TraceData(TraceEventType.Information, 78, method);

            source.Close();

            return 0;
		}

		private static int OnHelp(ParseResult options)
		{
			Console.WriteLine(options.GetHelpText(Console.WindowWidth));
			return 0;
		}

		private static int OnError(ParseResult options)
		{
			Console.WriteLine(options.Text);
			return 2;
		}
	}
}