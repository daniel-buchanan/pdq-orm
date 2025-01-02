using decaf.common.Templates;
using decaf.db.common.Builders;

namespace decaf.db.common.ANSISQL;

public class DropTableBuilderPipeline : db.common.Builders.DropTableBuilderPipeline
{
    public DropTableBuilderPipeline(
        DecafOptions options, 
        IConstants constants, 
        IParameterManager parameterManager,
        IValueParser valueParser) 
        : base(options, constants, parameterManager, valueParser) { }
}