using System;

namespace decaf.state.Ddl.Definitions;

public class ColumnDefinition : IColumnDefinition
{
    public static IColumnDefinition Create(string name, Type type = null, bool nullable = false)
        => new ColumnDefinition(name, type ?? typeof(object), nullable);

    private ColumnDefinition(string name, Type type, bool nullable)
    {
        Name = name;
        Type = type;
        Nullable = nullable;
    }
    
    /// <inheritdoc />
    public string Name { get; }
    
    /// <inheritdoc />
    public Type Type { get; }
    
    /// <inheritdoc />
    public bool Nullable { get; }

    public string GetSanitisedName()
    {
        throw new NotImplementedException();
    }
}