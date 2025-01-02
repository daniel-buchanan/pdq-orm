using System;
using System.Linq.Expressions;
using decaf.common.Utilities.Reflection;
using decaf.common.Utilities.Reflection.Dynamic;
using decaf.state.Ddl.Definitions;

namespace decaf.ddl.Utilities;

public static class ColumnDefinitionBuilder
{
    public static IColumnDefinition Build(DynamicColumnInfo col)
        => Build(b => b.Named(col.NewName).AsType(col.ValueType));

    public static IColumnDefinition Build<T>(Expression<Func<T, object>> col)
        => Build(b => b.Named(col).AsType(col));

    public static IColumnDefinition Build(Expression<Action<IDdlColumnBuilder>> expr)
    {
        var reflectionHelper = new ReflectionHelper();
        var expressionHelper = new ExpressionHelper(reflectionHelper);
        var columnBuilder = new DdlColumnBuilder(expressionHelper);
        var func = expr.Compile();
        func(columnBuilder);
        return columnBuilder.Build();
    }
}