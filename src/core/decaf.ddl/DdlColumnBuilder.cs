using System;
using System.Linq.Expressions;
using decaf.common.Utilities.Reflection;
using decaf.state.Ddl.Definitions;

namespace decaf.ddl;

public class DdlColumnBuilder : IDdlColumnBuilder
{
    private readonly IExpressionHelper _expressionHelper;
    
    public DdlColumnBuilder(IExpressionHelper expressionHelper) 
        => _expressionHelper = expressionHelper;

    public string Name { get; private set; }
    public bool Nullable { get; private set; }
    public Type Type { get; private set; }
    
    public IDdlColumnBuilder Named(string name)
    {
        Name = name;
        return this;
    }

    public IDdlColumnBuilder Named<T>()
    {
        Name = typeof(T).Name;
        return this;
    }

    public IDdlColumnBuilder Named<T, TValue>(Expression<Func<T, TValue>> expression)
    {
        Name = _expressionHelper.GetMemberName(expression);
        Type = _expressionHelper.GetMemberType(expression);
        return this;
    }

    public IDdlColumnBuilder IsNullable()
    {
        Nullable = true;
        return this;
    }

    public IDdlColumnBuilder IsNullable(bool nullable)
    {
        Nullable = nullable;
        return this;
    }

    public IDdlColumnBuilder AsType<T>()
    {
        Type = typeof(T);
        return this;
    }

    public IDdlColumnBuilder AsType(Type type)
    {
        Type = type;
        return this;
    }

    public IDdlColumnBuilder AsType<T>(Expression<Func<T, object>> expression)
    {
        Type = _expressionHelper.GetMemberType(expression);
        return this;
    }

    public IColumnDefinition Build()
        => ColumnDefinition.Create(Name, Type, Nullable);
}