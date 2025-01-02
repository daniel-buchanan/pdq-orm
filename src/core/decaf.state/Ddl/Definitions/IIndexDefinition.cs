using System.Collections.Generic;

namespace decaf.state.Ddl.Definitions;

public interface IIndexDefinition
{
    /// <summary>
    /// The name of this Index.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The columns which make up this Index.
    /// </summary>
    IEnumerable<IColumnDefinition> Columns { get; }
}

public interface IIndexDefinition<T> : IIndexDefinition;