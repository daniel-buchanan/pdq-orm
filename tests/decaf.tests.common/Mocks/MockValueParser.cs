using System;
using System.Collections.Generic;
using decaf.common.Utilities.Reflection;
using decaf.db.common.Builders;

namespace decaf.tests.common.Mocks
{
    public class MockValueParser(IReflectionHelper reflectionHelper, IConstants constants)
        : db.common.ValueParser(reflectionHelper, constants)
    {
        protected override List<Tuple<string, string>> Replacements => new List<Tuple<string, string>>();

        public override bool ValueNeedsQuoting(Type type)
            => true;

        protected override bool BooleanFromString(string input)
        {
            if (bool.TryParse(input, out var result))
                return result;
            return false;
        }

        protected override string BooleanToString(bool input)
            => input ? "True" : "False";

        protected override byte[] BytesFromString(string input)
            => System.Text.Encoding.UTF8.GetBytes(input);

        protected override string BytesToString(byte[] input)
            => System.Text.Encoding.UTF8.GetString(input);
    }
}