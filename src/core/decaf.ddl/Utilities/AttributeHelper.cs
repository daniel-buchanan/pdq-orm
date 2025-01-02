using System;
using System.Collections.Generic;
using System.Linq;
using decaf.common.Utilities.Reflection;
using decaf.ddl.Attributes;
using decaf.state.Ddl.Definitions;

namespace decaf.ddl.Utilities;

public class AttributeHelper(IReflectionHelper reflectionHelper, IExpressionHelper expressionHelper)
{
    private IEnumerable<IColumnDefinition> GetComponents<T, TAttribute>() where TAttribute : Attribute
    {
        var attrs = reflectionHelper.GetAttributes<T, TAttribute>();

        var components = new List<IColumnDefinition>();
        foreach (var a in attrs)
        {
            var cb = new DdlColumnBuilder(expressionHelper);
            cb.Named(a.AppliedTo)
                .AsType(a.AppliedType);
            components.Add(cb.Build());
        }

        return components;
    }

    public IPrimaryKeyDefinition GetPrimaryKey<T>()
    {
        var components = GetComponents<T, PrimaryKeyComponentAttribute>();
        return PrimaryKeyDefinition.Create<T>(components.ToArray());
    }
    
    public IPrimaryKeyDefinition GetPrimaryKey<T>(string name)
    {
        var components = GetComponents<T, PrimaryKeyComponentAttribute>();
        return PrimaryKeyDefinition.Create(name, components.ToArray());
    }

    public IIndexDefinition[] GetIndexes<T>()
    {
        var tbl = reflectionHelper.GetTableName<T>();
        var indexes = new List<IIndexDefinition>();
        var attrs = reflectionHelper.GetAttributes<T, IndexAttribute>();
        foreach (var a in attrs)
        {
            if (a.Attribute is not IndexAttribute idx) continue;
            indexes.Add(IndexDefinition.Create(idx.Name, tbl, idx.Columns.ToArray()));
        }

        return indexes.ToArray();
    }
}