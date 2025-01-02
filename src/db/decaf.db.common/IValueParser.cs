using System;

namespace decaf.db.common
{
    public interface IValueParser
    {
        /// <summary>
        /// Parse a string to a given type
        /// </summary>
        /// <typeparam name="T">The type to parse the string to</typeparam>
        /// <param name="value">The string value to parse</param>
        /// <returns>The converted value, or the default for the type specified.</returns>
        T FromString<T>(string value);

        /// <summary>
        /// Parses a value and performs replaces and corrections to ensure validity
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <returns></returns>
        string ToString<T>(T value);

        /// <summary>
        /// Parses a value and performs replaces and corrections to ensure validity
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="type">The type fo perform parsing/validation on</param>
        /// <returns></returns>
        string ToString(object value, Type type);

        /// <summary>
        /// Determines if a value needs to be quoted
        /// </summary>
        /// <param name="type">The type to use.</param>
        /// <returns>True if the value needs quoting, False if not.</returns>
        bool ValueNeedsQuoting(Type type);

        /// <summary>
        /// Determines if a value needs to be quoted
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to check for quoting</param>
        /// <returns>True if the value needs quoting, False if not.</returns>
        bool ValueNeedsQuoting<T>(T value);

        /// <summary>
        /// Quote the input value if necessary.
        /// </summary>
        /// <param name="value">The value to be inputted and parsed, and quoted if necessary.</param>
        /// <param name="useColumnQuotes">
        /// Whether the input value being parsed should use column quotes,
        /// the default is value quotes.</param>
        /// <typeparam name="T">The type of the incoming value.</typeparam>
        /// <returns>A string representation of the value, quoted if necessary.</returns>
        string QuoteIfNecessary<T>(T value, bool useColumnQuotes = false);

        /// <summary>
        /// Quote the input value if necessary.
        /// </summary>
        /// <param name="type">The type to determine quoting from.</param>
        /// <param name="value">The value to be inputted and parsed, and quoted if necessary.</param>
        /// <param name="useColumnQuotes">
        /// Whether the input value being parsed should use column quotes,
        /// the default is value quotes.</param>
        /// <returns>A string representation of the value, quoted if necessary.</returns>
        string QuoteIfNecessary(Type type, string value, bool useColumnQuotes = false);
    }
}