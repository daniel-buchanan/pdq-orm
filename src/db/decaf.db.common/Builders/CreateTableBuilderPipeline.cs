using System.Linq;
using decaf.common.Templates;
using decaf.state;
using decaf.state.Ddl.Definitions;

namespace decaf.db.common.Builders;

public abstract class CreateTableBuilderPipeline : BuilderPipeline<ICreateTableQueryContext>
{
    protected CreateTableBuilderPipeline(
        DecafOptions options,
        IConstants constants,
        IParameterManager parameterManager) :
        base(options, constants, parameterManager)
    {
        Add(AddCreateTableInternal, providesParameters: false);
        Add(AddColumns, providesParameters: false);
        Add(AddEndTable, providesParameters: false);
        Add(AddIndicies, providesParameters: false);
        Add(AddPrimaryKey, providesParameters: false);
    }

    private void AddCreateTableInternal(IPipelineStageInput<ICreateTableQueryContext> input)
    {
        AddCreateTable(input);
        input.Builder.IncreaseIndent();
    }

    protected abstract void AddCreateTable(IPipelineStageInput<ICreateTableQueryContext> input);

    private void AddColumns(IPipelineStageInput<ICreateTableQueryContext> input)
    {
        var columns = input.Context.Columns.ToArray();
        for (var i = 0; i < columns.Length; i++)
        {
            var isLast = i == columns.Length - 1;
            var c = columns[i];
            var comma = isLast
                ? string.Empty
                : ",";
            var column = FormatColumn(c);
            input.Builder.AppendLine("{0}{1}", column, comma);
        }

        input.Builder.DecreaseIndent();
    }

    protected abstract string FormatColumn(IColumnDefinition column);
    
    protected abstract void AddIndicies(IPipelineStageInput<ICreateTableQueryContext> input);

    protected abstract void AddEndTable(IPipelineStageInput<ICreateTableQueryContext> input);
    
    protected abstract void AddPrimaryKey(IPipelineStageInput<ICreateTableQueryContext> input);
}