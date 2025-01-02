using decaf.common.Utilities.Reflection;
using decaf.db.common;
using decaf.db.common.Builders;

namespace decaf.sqlite
{
    public class SqliteValueParser(IReflectionHelper reflectionHelper, IConstants constants)
        : ValueParser(reflectionHelper, constants)
    {
        protected override List<Tuple<string, string>> Replacements { get; } = new()
        {
            new Tuple<string, string>("'", "''"),
            new Tuple<string, string>("%%", ""),
            new Tuple<string, string>("--", "")
        };

        protected override byte[] BytesFromString(string input)
        {
            if (!(input.StartsWith(@"\x") || input.StartsWith(@"\\x")))
                return new byte[0];

            if (input.StartsWith(@"\\x")) input = input.Substring(3);
            else if (input.StartsWith(@"\x")) input = input.Substring(2);

            int NumberChars = input.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(input.Substring(i, 2), 16);
            return bytes;
        }

        protected override string BytesToString(byte[] input)
        {
            var valueString = BitConverter.ToString(input).Replace("-", "");
            return $@"\x{valueString}";
        }
    }
}

