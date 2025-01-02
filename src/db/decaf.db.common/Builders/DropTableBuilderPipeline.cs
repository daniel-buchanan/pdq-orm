using decaf.common.Templates;
using decaf.state;

namespace decaf.db.common.Builders;

public abstract class DropTableBuilderPipeline : BuilderPipeline<IDropTableQueryContext>
{
    private readonly IValueParser valueParser;
    
    protected DropTableBuilderPipeline(
        DecafOptions options,
        IConstants constants,
        IParameterManager parameterManager, 
        IValueParser valueParser) :
        base(options, constants, parameterManager)
    {
        this.valueParser = valueParser;
        Add(AddDropTable, providesParameters: false);
        Add(AddCascade, providesParameters: false);
        Add(AddEndTable, providesParameters: false);
    }

    protected virtual void AddDropTable(IPipelineStageInput<IDropTableQueryContext> input)
    {
        var name = valueParser.ValueNeedsQuoting(input.Context.Name)
            ? string.Format(Constants.QuoteFormat, Constants.ColumnQuote, input.Context.Name)
            : input.Context.Name;
        input.Builder.AppendLine("drop table {0}", name);
    }

    protected virtual void AddEndTable(IPipelineStageInput<IDropTableQueryContext> input)
        => input.Builder.Append(Constants.EndStatement);

    protected virtual void AddCascade(IPipelineStageInput<IDropTableQueryContext> input)
    {
        if (!input.Context.Cascade) return;
        
        var val = input.Context.Cascade
            ? Constants.Cascade
            : string.Empty;
        input.Builder.AppendLine(val);
    }
}