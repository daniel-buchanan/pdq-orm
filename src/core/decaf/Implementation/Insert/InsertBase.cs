using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;

namespace decaf.Implementation.Execute
{
    internal abstract class InsertBase : Execute<IInsertQueryContext>
    {
        protected InsertBase(IQueryContainerInternal query, IInsertQueryContext context)
            : base(query, context)
        {
            Query.SetContext(Context);
        }

        protected void FromQuery(Action<ISelect> queryBuilder)
        {
            var context = SelectQueryContext.Create(Query.AliasManager, Query.HashProvider);
            var query = Query.UnitOfWork.GetQuery() as IQueryContainerInternal;
            var select = Select.Create(context, query);

            queryBuilder(select);
            var source = state.ValueSources.Insert.QueryValuesSource.Create(context);
            Context.From(source);
        }

        protected void AddValues<T>(T value)
        {
            var internalContext = Context as IQueryContextExtended;
            var properties = internalContext.ReflectionHelper.GetMemberDetails(value);
            var values = properties.Select(p => internalContext.ReflectionHelper.GetPropertyValue(value, p.NewName));
            var row = values.ToArray();
            Context.Value(row);
        }

        protected void AddValues<T>(IEnumerable<T> values)
        {
            foreach (var v in values) AddValues(v);
        }

        protected void AddColumns(Expression expression)
        {
            var properties = Context.Helpers().GetPropertyInformation(expression);
            foreach (var p in properties)
            {
                var target = Context.QueryTargets.FirstOrDefault(t => t.Alias == p.Alias);
                Context.Column(Column.Create(p.Name, target, p.NewName));
            }
        }
    }
}

