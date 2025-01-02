using System.Collections.Generic;
using decaf.common.Templates;
using decaf.db.common.Builders;
using decaf.state;

namespace decaf.tests.common.Mocks;

public class MockSqlFactory : SqlFactory
{
    private readonly IBuilderPipeline<IDeleteQueryContext> delete;
    private readonly IBuilderPipeline<IInsertQueryContext> insert;
    private readonly IBuilderPipeline<IUpdateQueryContext> update;
    private readonly IBuilderPipeline<ISelectQueryContext> select;
    private readonly IBuilderPipeline<ICreateTableQueryContext> createTable;
    private readonly IBuilderPipeline<IDropTableQueryContext> dropTable;

    public MockSqlFactory(
        IBuilderPipeline<IDeleteQueryContext> delete,
        IBuilderPipeline<IInsertQueryContext> insert,
        IBuilderPipeline<IUpdateQueryContext> update,
        IBuilderPipeline<ISelectQueryContext> select,
        IBuilderPipeline<ICreateTableQueryContext> createTable,
        IBuilderPipeline<IDropTableQueryContext> dropTable)
    {
        this.delete = delete;
        this.insert = insert;
        this.update = update;
        this.select = select;
        this.createTable = createTable;
        this.dropTable = dropTable;
    }

    protected override SqlTemplate ParseQuery(IDeleteQueryContext context)
        => delete.Execute(context);

    protected override SqlTemplate ParseQuery(IInsertQueryContext context)
        => insert.Execute(context);

    protected override SqlTemplate ParseQuery(ICreateTableQueryContext context)
        => createTable.Execute(context);

    protected override SqlTemplate ParseQuery(IDropTableQueryContext context)
        => dropTable.Execute(context);

    protected override SqlTemplate ParseQuery(ISelectQueryContext context)
        => select.Execute(context);

    protected override SqlTemplate ParseQuery(IUpdateQueryContext context)
        => update.Execute(context);

    protected override IDictionary<string, object> ParseParameters(ISelectQueryContext context, bool includePrefix)
        => select.GetParameterValues(context, includePrefix);

    protected override IDictionary<string, object> ParseParameters(IDeleteQueryContext context, bool includePrefix)
        => delete.GetParameterValues(context, includePrefix);

    protected override IDictionary<string, object> ParseParameters(IUpdateQueryContext context, bool includePrefix)
        => update.GetParameterValues(context, includePrefix);

    protected override IDictionary<string, object> ParseParameters(IInsertQueryContext context, bool includePrefix)
        => insert.GetParameterValues(context, includePrefix);
}