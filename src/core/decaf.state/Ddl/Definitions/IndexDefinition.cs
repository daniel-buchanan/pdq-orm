using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace decaf.state.Ddl.Definitions;

public class IndexDefinition : IIndexDefinition
{
    private IndexDefinition(string table, params IColumnDefinition[] columns) : this(string.Empty, table, columns) { }

    private IndexDefinition(string name, string table, params IColumnDefinition[] columns)
    {
        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentNullException(nameof(table));
        
        if (columns == null ||
            !columns.Any())
            throw new ArgumentNullException(nameof(columns));
        
        Name = string.IsNullOrWhiteSpace(name)
            ? GenerateName(columns)
            : name;
        Columns = columns;
        Table = table;
    }

    public static IIndexDefinition Create(string table, params IColumnDefinition[] columns)
        => new IndexDefinition(table, columns);

    public static IIndexDefinition Create(string name, string table, params IColumnDefinition[] columns)
        => new IndexDefinition(name, table, columns);
    
    /// <inheritdoc />
    public string Name { get; }
    
    public string Table { get; }
    
    /// <inheritdoc />
    public IEnumerable<IColumnDefinition> Columns { get; }
    
    private static string GenerateName(IEnumerable<IColumnDefinition> columns)
    {
        var sb = new StringBuilder("idx");
        foreach (var c in columns)
            sb.AppendFormat("_{0}", c.Name);
        return sb.ToString();
    }
}