using System;
using System.Collections.Generic;

namespace decaf.state.Ddl.Definitions;

public class PrimaryKeyDefinition : IPrimaryKeyDefinition
{
    protected PrimaryKeyDefinition(string name, params IColumnDefinition[] columns)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));
        
        Columns = columns;
        Name = name;
    }

    public static IPrimaryKeyDefinition Create<T>(params IColumnDefinition[] columns)
        => new PrimaryKeyDefinition<T>(columns);

    public static IPrimaryKeyDefinition Create(string name, params IColumnDefinition[] columns)
        => new PrimaryKeyDefinition(name, columns);
    
    /// <inheritdoc />
    public string Name { get; }
    
    /// <inheritdoc />
    public IEnumerable<IColumnDefinition> Columns { get; }
}

public class PrimaryKeyDefinition<T>(params IColumnDefinition[] columns)
    : PrimaryKeyDefinition("pk_" + typeof(T).Name, columns), IPrimaryKeyDefinition<T>;