using decaf.common.Templates;
using decaf.db.common;
using decaf.db.common.Builders;
using decaf.state;
using decaf.state.Ddl.Definitions;

namespace decaf.sqlite.Builders;

public class CreateTableBuilderPipeline : db.common.ANSISQL.CreateTableBuilderPipeline
{
    private readonly ITypeParser typeParser;
    
    public CreateTableBuilderPipeline(
        DecafOptions options, 
        IConstants constants, 
        IParameterManager parameterManager,
        ITypeParser typeParser,
        IValueParser valueParser) : 
        base(options, constants, parameterManager, typeParser, valueParser) 
        => this.typeParser = typeParser;

    protected override void AddCreateTable(IPipelineStageInput<ICreateTableQueryContext> input)
        => input.Builder.AppendLine("create table {0} {1}", input.Context.Name, Constants.OpeningParen);

    protected override string FormatColumn(IColumnDefinition column)
    {
        var type = typeParser.Parse(column.Type);
        var name = ValueParser.QuoteIfNecessary(column.Name);
        var notNull = column.Nullable
            ? "null"
            : "not null";
        return $"{name} {type} {notNull}";
    }
}