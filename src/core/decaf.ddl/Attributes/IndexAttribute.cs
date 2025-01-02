using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using decaf.ddl.Utilities;
using decaf.state.Ddl.Definitions;

namespace decaf.ddl.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class IndexAttribute : Attribute
{
    public string Name { get; set; }
    
    public IEnumerable<IColumnDefinition> Columns { get; set; }

    public IndexAttribute() { }

    public IndexAttribute(string name) 
        => Name = name;

    public IndexAttribute(params Expression<Action<IDdlColumnBuilder>>[] columns) 
        => Columns = columns?.Select(ColumnDefinitionBuilder.Build);

    public IndexAttribute(string name, params Expression<Action<IDdlColumnBuilder>>[] columns)
    {
        Name = name;
        Columns = columns?.Select(ColumnDefinitionBuilder.Build);
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class IndexAttribute<T> : IndexAttribute
{
    public IndexAttribute(string name) 
        => Name = name;

    public IndexAttribute(params Expression<Func<T, object>>[] columns) 
        => Columns = columns?.Select(ColumnDefinitionBuilder.Build);

    public IndexAttribute(string name, params Expression<Func<T, object>>[] columns)
    {
        Name = name;
        Columns = columns?.Select(ColumnDefinitionBuilder.Build);
    }
}