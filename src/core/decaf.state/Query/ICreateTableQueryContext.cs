using System.Collections.Generic;
using decaf.common;
using decaf.state.Ddl.Definitions;

namespace decaf.state;

public interface ICreateTableQueryContext : IQueryContext
{
    string Name { get; }

    void WithName(string name);
    
    IEnumerable<IColumnDefinition> Columns { get; }

    void AddColumns(params IColumnDefinition[] cols);
    
    IEnumerable<IIndexDefinition> Indexes { get; }

    void AddIndexes(params IIndexDefinition[] indexes);
    
    IPrimaryKeyDefinition PrimaryKey { get; }

    void AddPrimaryKey(IPrimaryKeyDefinition key);
}