using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using decaf.common;
using decaf.common.Exceptions;
using decaf.common.Utilities.Reflection;
using decaf.ddl.Utilities;
using decaf.Implementation.Execute;
using decaf.state;
using decaf.state.Ddl.Definitions;

namespace decaf.ddl.Implementation;

public class CreateTable : Execute<ICreateTableQueryContext>, ICreateTable
{
    private const string TableNameRequiredForIndexReason = "Table Name is required before creating indexes";
    private readonly IExpressionHelper expressionHelper;
    private readonly IReflectionHelper reflectionHelper;

    private CreateTable(
        ICreateTableQueryContext context,
        IQueryContainerInternal query) :
        base(query, context)
    {
        expressionHelper = context.ToInternal().ExpressionHelper;
        reflectionHelper = context.ToInternal().ReflectionHelper;
        query.SetContext(context);
    }

    public static CreateTable Create(
        ICreateTableQueryContext context,
        IQueryContainer query)
        => new CreateTable(context, query as IQueryContainerInternal);

    public static CreateTable<T> Create<T>(
        ICreateTableQueryContext context,
        IQueryContainer query)
    {
        var n = new CreateTable<T>(context, query as IQueryContainerInternal);
        n.WithName(typeof(T).Name);
        return n;
    }

    public ICreateTable FromType<T>(string named = null)
    {
        var tbl = Create<T>(Context, Query);
        if (string.IsNullOrWhiteSpace(named)) named = typeof(T).Name;
        tbl.WithName(named);
        tbl.WithAllColumns();
        tbl.WithPrimaryKey();
        tbl.WithIndexes();
        return this;
    }

    public ICreateTable Named(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Table name is required.");
        
        Context.WithName(name);
        return this;
    }

    public ICreateTable WithColumns(params Expression<Action<IDdlColumnBuilder>>[] columns)
    {
        foreach (var c in columns)
        {
            var columnBuilder = new DdlColumnBuilder(expressionHelper);
            var func = c.Compile();
            func(columnBuilder);
            Context.AddColumns(columnBuilder.Build());
        }

        return this;
    }

    public ICreateTable WithColumns<T>()
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

    public ICreateTable WithIndex(string name, params Expression<Action<IDdlColumnBuilder>>[] columns)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Index name is required.");
        
        if(string.IsNullOrWhiteSpace(Context.Name))
            throw new PropertyNotProvidedException(nameof(Context.Name), TableNameRequiredForIndexReason);
        
        var defs = columns.Select(ColumnDefinitionBuilder.Build).ToArray();
        Context.AddIndexes(IndexDefinition.Create(name, Context.Name, defs));
        return this;
    }

    public ICreateTable WithIndex(params Expression<Action<IDdlColumnBuilder>>[] columns)
    {
        if(string.IsNullOrWhiteSpace(Context.Name))
            throw new PropertyNotProvidedException(nameof(Context.Name), TableNameRequiredForIndexReason);
        
        var defs = columns.Select(ColumnDefinitionBuilder.Build).ToArray();
        Context.AddIndexes(IndexDefinition.Create(null, Context.Name, defs));
        return this;
    }

    public ICreateTable WithPrimaryKey(params Expression<Action<IDdlColumnBuilder>>[] columns)
    {
        if(string.IsNullOrWhiteSpace(Context.Name))
            throw new PropertyNotProvidedException(nameof(Context.Name), TableNameRequiredForIndexReason);
        
        var cols = new List<IColumnDefinition>();
        foreach (var c in columns)
        {
            var builder = new DdlColumnBuilder(expressionHelper);
            var func = c.Compile();
            func(builder);
            cols.Add(builder.Build());
        }

        var pk = PrimaryKeyDefinition.Create(Context.Name, cols.ToArray());
        Context.AddPrimaryKey(pk);

        return this;
    }

    public ICreateTable WithPrimaryKey(string name, params Expression<Action<IDdlColumnBuilder>>[] columns)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Primary Key name is required.");
        
        if(string.IsNullOrWhiteSpace(Context.Name))
            throw new PropertyNotProvidedException(nameof(Context.Name), "Table Name is required before creating indexes");
        
        var defs = columns.Select(ColumnDefinitionBuilder.Build).ToArray();
        Context.AddPrimaryKey(PrimaryKeyDefinition.Create(name, defs.ToArray()));
        return this;
    }
}