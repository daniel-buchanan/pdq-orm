using decaf.common.Templates;
using decaf.db.common;
using decaf.db.common.Builders;

namespace decaf.tests.common.Mocks
{
    public class MockDropTablePipeline : db.common.ANSISQL.DropTableBuilderPipeline
    {
        public MockDropTablePipeline(
            DecafOptions options,
            IConstants constants,
            IParameterManager parameterManager,
            IValueParser valueParser) :
            base(options, constants, parameterManager, valueParser)
        {
        }
    }
}