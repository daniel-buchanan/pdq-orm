using System;

namespace decaf.state.Ddl.Definitions;

public interface IColumnDefinition
{
    /// <summary>
    /// The name of this Column.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The type of the Column.
    /// </summary>
    Type Type { get; }
    
    /// <summary>
    /// Whether or not this Column is nullable.
    /// </summary>
    bool Nullable { get; }

    /// <summary>
    /// Get the sanitised column name, i.e. no spaces, special characters etc,
    /// </summary>
    /// <returns>The sanitised column name.</returns>
    string GetSanitisedName();
}