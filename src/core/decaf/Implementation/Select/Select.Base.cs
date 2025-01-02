using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using decaf.common;
using decaf.common.Exceptions;
using decaf.common.Utilities.Reflection.Dynamic;
using decaf.Exceptions;
using decaf.state;

namespace decaf.Implementation.Execute
{
	internal abstract class SelectCommon : Execute<ISelectQueryContext>
	{
        protected readonly DecafOptions Options;
        protected new readonly ISelectQueryContext Context;

        protected SelectCommon(
            ISelectQueryContext context,
            IQueryContainer query)
            : base((IQueryContainerInternal)query, context)
        {
            Options = (query as IQueryContainerInternal).Options;
            Context = context;
            Query.SetContext(Context);
        }

        protected void AddFrom<T>(Expression<Func<T, T>> expression = null)
        {
            string managedTable, managedAlias;

            if (expression is null)
            {
                managedTable = Context.Helpers().GetTableName<T>();
                managedAlias = Query.AliasManager.Add(null, managedTable);
            }
            else
            {
                var table = Context.Helpers().GetTableName(expression);
                var alias = Context.Helpers().GetTableAlias(expression);
                managedTable = Query.AliasManager.GetAssociation(alias) ?? table;
                managedAlias = Query.AliasManager.Add(alias, table);
            }

            Context.From(state.QueryTargets.TableTarget.Create(managedTable, managedAlias));
        }

        protected void AddColumns(Expression expression)
        {
            var properties = Context.Helpers().GetPropertyInformation(expression);
            foreach (var p in properties)
            {
                var target = GetQueryTargetByAlias(p.Alias);
                if (target == null) target = GetQueryTargetByType(p.Type);
                
                Context.Select(Column.Create(p.Name, target, p.NewName));
            }
        }

        private IQueryTarget GetQueryTargetByType(Type type)
        {
            var table = Context.Helpers().GetTableName(type);
            return GetQueryTargetByTable(table);
        }

        protected IQueryTarget GetQueryTargetByTable(string table)
        {
            var alias = Query.AliasManager.FindByAssociation(table).FirstOrDefault();
            if (alias == null) throw new TableNotFoundException(table);

            return GetQueryTargetByAlias(alias.Name);
        }

        private IQueryTarget GetQueryTargetByAlias(string alias)
        {
            var managedAlias = Query.AliasManager.GetAssociation(alias);
            if (string.IsNullOrWhiteSpace(managedAlias))
                throw new TableNotFoundException(alias ?? "\"No ALIAS Provided\"");

            var target = Context.QueryTargets.FirstOrDefault(t => t.Alias == alias);
            if (target == null) throw new TableNotFoundException(null, managedAlias);
            return target;
        }

        protected void AddAllColumns<T>(Expression<Func<T, object>> expression)
        {
            var param = Context.Helpers().GetTableAlias(expression);
            AddAllColumns<T>(param);
        }

        protected void AddAllColumns<T>(string alias)
        {
            var entityType = typeof(T);
            var internalContext = Context as IQueryContextExtended;
            
            var members = new List<PropertyInfo>();
            var arguments = new List<Expression>();
            var parameterExpression = Expression.Parameter(typeof(ISelectColumnBuilder), "b");
            var properties = internalContext?.ReflectionHelper.GetMemberDetails(entityType, QueryTypes.Select);
            var emptyCtor = new DynamicConstructorInfo(properties, typeof(T));
            var isMethod = typeof(ISelectColumnBuilder).GetMethods().FirstOrDefault(m => m.IsGenericMethod && m.GetParameters().Count() == 2);

            if(isMethod == null)
                throw new ShouldNeverOccurException("The method .Is<T>(column, alias) could not be found on `ISelectColumnBuilder`, this should never happen!");

            if (properties is null)
                throw new ShouldNeverOccurException($"No public properties found for type {typeof(T).Name}.");

            foreach (var p in properties)
            {
                var typedIsMethod = isMethod.MakeGenericMethod(p.ValueType);
                var nameExpression = Expression.Constant(p.NewName);
                var aliasExpression = Expression.Constant(alias);
                var methodCallExpression = Expression.Call(parameterExpression, typedIsMethod, nameExpression, aliasExpression);
                var member = new DynamicPropertyInfo(p.Name, p.ValueType, typeof(T));
                members.Add(member);
                arguments.Add(methodCallExpression);
            }

            var bodyExpression = Expression.New(emptyCtor, arguments, members);
            var lambdaExpression = (Expression<Func<ISelectColumnBuilder, T>>)Expression.Lambda(bodyExpression, parameterExpression);
            AddColumns(lambdaExpression);
        }
    }
}

