using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using decaf.common;
using decaf.common.Utilities.Reflection;
using decaf.ddl.Utilities;
using decaf.Implementation.Execute;
using decaf.state;
using decaf.state.Ddl.Definitions;

namespace decaf.ddl.Implementation;

public class CreateTable<T> :
        Execute<ICreateTableQueryContext>,
        ICreateTable<T>
{
    private readonly IExpressionHelper expressionHelper;
    private readonly IReflectionHelper reflectionHelper;

    public CreateTable(
        ICreateTableQueryContext context, 
        IQueryContainerInternal query) : 
        base(query, context)
    {
        expressionHelper = context.ToInternal().ExpressionHelper;
        reflectionHelper = context.ToInternal().ReflectionHelper;
        query.SetContext(context);
    }

    private static IColumnDefinition[] GetDefinitions(IEnumerable<Expression<Func<T, object>>> columns)
    {
        return columns
            .Select(ColumnDefinitionBuilder.Build)
            .ToArray();
    }

    public ICreateTable<T> WithName(string name)
    {
        Context.WithName(name);
        return this;
    }

    public ICreateTable<T> WithColumns(params Expression<Func<T, object>>[] columns)
    {
        Context.AddColumns(GetDefinitions(columns));
        return this;
    }

    public ICreateTable<T> WithAllColumns()
    {
        var members = reflectionHelper.GetMemberDetails(typeof(T));
        foreach (var m in members)
        {
            var cb = new DdlColumnBuilder(expressionHelper);
            cb.Named(m.NewName ?? m.Name)
                .AsType(m.ValueType);
            Context.AddColumns(cb.Build());
        }

        return this;
    }

    public ICreateTable<T> WithIndex(params Expression<Func<T, object>>[] columns)
    {
        var tbl = reflectionHelper.GetTableName<T>();
        var defs = GetDefinitions(columns);
        Context.AddIndexes(IndexDefinition.Create(tbl, defs));
        return this;
    }

    public ICreateTable<T> WithIndex(string name, params Expression<Func<T, object>>[] columns)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Index name is required.");
        
        var tbl = reflectionHelper.GetTableName<T>();
        var defs = GetDefinitions(columns);
        Context.AddIndexes(IndexDefinition.Create(name, tbl, defs));
        return this;
    }

    public ICreateTable<T> WithIndexes()
    {
        var helper = new AttributeHelper(reflectionHelper, expressionHelper);
        var indexes = helper.GetIndexes<T>();
        Context.AddIndexes(indexes);

        return this;
    }

    public ICreateTable<T> WithPrimaryKey(params Expression<Func<T, object>>[] columns)
    {
        var defs = columns
            .Select(ColumnDefinitionBuilder.Build)
            .ToArray();
        Context.AddPrimaryKey(PrimaryKeyDefinition.Create<T>(defs));
        return this;
    }

    public ICreateTable<T> WithPrimaryKey(string name, params Expression<Func<T, object>>[] columns)
    {
        var defs = GetDefinitions(columns);
        Context.AddPrimaryKey(PrimaryKeyDefinition.Create(name, defs));
        return this;
    }

    public ICreateTable<T> WithPrimaryKey()
    {
        var helper = new AttributeHelper(reflectionHelper, expressionHelper);
        Context.AddPrimaryKey(helper.GetPrimaryKey<T>());
        return this;
    }
}