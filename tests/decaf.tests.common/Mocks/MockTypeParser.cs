using System;
using decaf.db.common;

namespace decaf.tests.common.Mocks
{
    public class MockTypeParser : TypeParser
    {
        public override string Parse(Type type)
            => type == null
                ? "NULL"
                : type.Name;
    }
}