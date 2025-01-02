using decaf.common.Templates;
using decaf.db.common;
using decaf.db.common.Builders;

namespace decaf.tests.common.Mocks
{
    public class MockCreateTablePipeline : db.common.ANSISQL.CreateTableBuilderPipeline
    {
        public MockCreateTablePipeline(
            DecafOptions options,
            IConstants constants,
            IParameterManager parameterManager,
            ITypeParser typeParser,
            IValueParser valueParser) :
            base(options, constants, parameterManager, typeParser, valueParser)
        {
        }
    }
}