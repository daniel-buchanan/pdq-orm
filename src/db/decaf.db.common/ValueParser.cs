using System;
using System.Collections.Generic;
using System.Globalization;
using decaf.common.Utilities.Reflection;
using decaf.db.common.Builders;

namespace decaf.db.common
{
    public abstract class ValueParser(IReflectionHelper reflectionHelper, IConstants constants) : IValueParser
    {
        protected readonly IReflectionHelper ReflectionHelper = reflectionHelper;
        protected readonly IConstants Constants = constants;

        protected abstract List<Tuple<string, string>> Replacements { get; }

        /// <inheritdoc/>
        public T FromString<T>(string value)
        {
            var isNullable = ReflectionHelper.IsNullableType<T>();
            var underlyingType = ReflectionHelper.GetUnderlyingType<T>();

            if (isNullable && value == null) return default(T);
            if (string.IsNullOrWhiteSpace(value)) return default(T);

            if (underlyingType == typeof(byte[]))
                return ChangeType<T>(BytesFromString(value));

            if (underlyingType == typeof(bool))
                return ChangeType<T>(BooleanFromString(value));

            if (underlyingType == typeof(DateTime))
                return ChangeType<T>(Convert.ToDateTime(value));

            if (underlyingType == typeof(int))
                return ChangeType<T>(Convert.ToInt32(value));

            if (underlyingType == typeof(double) ||
                underlyingType == typeof(Single) ||
                underlyingType == typeof(float))
                return ChangeType<T>(Convert.ToDecimal(value));

            if (underlyingType == typeof(string))
            {
                var s = Convert.ToString(value);
                if (string.IsNullOrWhiteSpace(s)) return default(T);
                foreach (var r in Replacements) s = s.Replace(r.Item1, r.Item2);
                return ChangeType<T>(s);
            }

            return ChangeType<T>(Convert.ToString(value));
        }

        private static T ChangeType<T>(object input)
            => (T)Convert.ChangeType(input, typeof(T));

        protected abstract byte[] BytesFromString(string input);

        protected virtual bool BooleanFromString(string input)
            => input == Constants.True; 

        /// <inheritdoc/>
        public string ToString<T>(T value) => ToString(value, value?.GetType() ?? typeof(T));

        /// <inheritdoc/>
        public string ToString(object value, Type type)
        {
            var underlyingType = ReflectionHelper.GetUnderlyingType(type);

            if (value == null) return string.Empty;

            if (underlyingType == typeof(byte[]))
                return BytesToString(value as byte[]);

            if (underlyingType == typeof(bool))
                return BooleanToString(Convert.ToBoolean(value));

            if (underlyingType == typeof(DateTime))
                return Convert.ToDateTime(value).ToString("yyyy-MM-ddTHH:mm:ss");

            if (underlyingType == typeof(int))
                return Convert.ToInt32(value).ToString();

            if (underlyingType == typeof(double) ||
                underlyingType == typeof(float) ||
                underlyingType == typeof(Single) ||
                underlyingType == typeof(decimal))
                return Convert.ToDecimal(value).ToString(CultureInfo.InvariantCulture);

            if (underlyingType == typeof(string))
            {
                var s = Convert.ToString(value);
                if (string.IsNullOrWhiteSpace(s)) return default(string);
                foreach (var r in Replacements) s = s.Replace(r.Item1, r.Item2);
                return s;
            }

            if (underlyingType.IsEnum)
                return value.ToString();

            return Convert.ToString(value);
        }

        protected abstract string BytesToString(byte[] input);

        protected virtual string BooleanToString(bool input)
            => input ? Constants.True : Constants.False;

        /// <inheritdoc/>
        public virtual bool ValueNeedsQuoting(Type type)
        {
            var underlyingType = ReflectionHelper.GetUnderlyingType(type);

            if (underlyingType == typeof(bool)) return true;
            if (underlyingType == typeof(byte[])) return true;
            if (underlyingType == typeof(DateTime)) return true;
            if (underlyingType == typeof(int)) return false;
            if (underlyingType == typeof(uint)) return false;
            if (underlyingType == typeof(double) ||
                underlyingType == typeof(float) ||
                underlyingType == typeof(decimal))
                return true;

            // default to true, including string types
            return true;
        }

        /// <inheritdoc/>
        public bool ValueNeedsQuoting<T>(T value)
            => ValueNeedsQuoting(typeof(T));

        /// <inheritdoc/>
        public string QuoteIfNecessary<T>(T value, bool useColumnQuotes = false)
            => QuoteIfNecessary(typeof(T), value.ToString(), useColumnQuotes);

        public string QuoteIfNecessary(Type type, string value, bool useColumnQuotes = false)
        {
            var quote = useColumnQuotes
                ? Constants.ColumnQuote
                : Constants.ValueQuote;
            var requiresQuoting = ValueNeedsQuoting(type);
            
            return !requiresQuoting 
                ? ToString(value) 
                : string.Format(Constants.QuoteFormat, quote, value);
        }
    }
}

