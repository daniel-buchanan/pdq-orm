using System.Collections.Generic;
using decaf.common;
using decaf.common.Utilities;
using decaf.state.Ddl.Definitions;

namespace decaf.state;

internal class CreateTableQueryContext : QueryContext, ICreateTableQueryContext
{
    private readonly List<IColumnDefinition> columns;
    private readonly List<IIndexDefinition> indicies;

    public CreateTableQueryContext(
        IAliasManager aliasManager,
        IHashProvider hashProvider) :
        base(aliasManager, QueryTypes.CreateTable, hashProvider)
    {
        columns = new List<IColumnDefinition>();
        indicies = new List<IIndexDefinition>();
    }

    public static ICreateTableQueryContext Create(IAliasManager aliasManager, IHashProvider hashProvider)
        => new CreateTableQueryContext(aliasManager, hashProvider);

    public string Name { get; private set; }

    public void WithName(string name)
        => Name = name;

    public IEnumerable<IColumnDefinition> Columns => columns;

    public void AddColumns(params IColumnDefinition[] cols)
        => columns.AddRange(cols);

    public IEnumerable<IIndexDefinition> Indexes => indicies;

    public void AddIndexes(params IIndexDefinition[] indexes)
        => indicies.AddRange(indexes);

    public IPrimaryKeyDefinition PrimaryKey { get; private set; }

    public void AddPrimaryKey(IPrimaryKeyDefinition key)
        => PrimaryKey = key;
}