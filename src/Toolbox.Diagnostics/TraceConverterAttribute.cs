using System;

namespace Toolbox.Diagnostics
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class TraceConverterAttribute : Attribute
    {
        public TraceConverterAttribute(Type converterType, params object[] args)
        {
            if (!typeof(TraceConverterBase).IsAssignableFrom(converterType))
                throw new ArgumentException($"Converter type does not inherit from {typeof(TraceConverterBase).FullName}.", nameof(converterType));

            ConvertType = converterType;
            Arguments = args;
        }

        public Type ConvertType { get; }
        public object[] Arguments { get; }

        internal TraceConverterBase? CreateConverter(Type type)
        {
            var converter = (TraceConverterBase?)Activator.CreateInstance(ConvertType);
            if (!ConvertType.IsAssignableFrom(type))
                throw new ArgumentException($"Converter {ConvertType.FullName} can not convert object of type {type.FullName}.");

            return converter;
        }
    }
}
