using System.Linq;
using decaf.common.Templates;
using decaf.db.common.Builders;
using decaf.state;
using decaf.state.Ddl.Definitions;

namespace decaf.db.common.ANSISQL;

public class CreateTableBuilderPipeline(
    DecafOptions options,
    IConstants constants,
    IParameterManager parameterManager,
    ITypeParser typeParser,
    IValueParser valueParser)
    : db.common.Builders.CreateTableBuilderPipeline(options, constants, parameterManager)
{
    protected readonly ITypeParser TypeParser = typeParser;
    protected readonly IValueParser ValueParser = valueParser;

    protected override void AddCreateTable(IPipelineStageInput<ICreateTableQueryContext> input)
    {
        input.Builder.AppendLine("create table {0} as (", input.Context.Name);
    }

    protected override string FormatColumn(IColumnDefinition column)
    {
        var type = TypeParser.Parse(column.Type);
        var name = ValueParser.QuoteIfNecessary(column.Name);
        var notNull = column.Nullable
            ? "null"
            : "not null";
        return $"{name} as {type} {notNull}";
    }

    protected override void AddIndicies(IPipelineStageInput<ICreateTableQueryContext> input)
    {
        var indexes = input.Context.Indexes.ToArray();
        foreach (var idx in indexes)
        {
            var columns = string.Join(",", idx.Columns.Select(QuoteValue));
            var name = ValueParser.QuoteIfNecessary(idx.Name);
            var table = ValueParser.ValueNeedsQuoting(input.Context.Name);
            
            input.Builder.AppendLine("create index {0} on {1} ({2}){3}", name, table, columns, Constants.EndStatement);
        }
    }

    protected override void AddEndTable(IPipelineStageInput<ICreateTableQueryContext> input)
        => input.Builder.AppendLine(Constants.ClosingParen);

    protected override void AddPrimaryKey(IPipelineStageInput<ICreateTableQueryContext> input)
    {
        var pk = input.Context.PrimaryKey;
        if (pk is null) return;
        
        var table = ValueParser.QuoteIfNecessary(input.Context.Name);
        var name = ValueParser.QuoteIfNecessary(pk.Name);
        var columns = string.Join(",", pk.Columns.Select(QuoteValue));
        input.Builder.AppendLine("alter table {0} constraint {1} primary key ({2}){3}", table, name, columns, Constants.EndStatement);
    }
    
    private string QuoteValue(IColumnDefinition columnDefinition) 
        => ValueParser.QuoteIfNecessary(columnDefinition.Type, columnDefinition.Name);
}