using System;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;

namespace decaf.Implementation.Execute
{
    internal class UpdateTyped<T> :
        UpdateBase,
        IUpdateTable<T>,
        IUpdateSet<T>
    {
        private UpdateTyped(
            IUpdateQueryContext context,
            IQueryContainerInternal query)
            : base(query, context)
        {
        }

        public static UpdateTyped<T> Create(
            IUpdateQueryContext context,
            IQueryContainerInternal query)
            => new UpdateTyped<T>(context, query);

        /// <inheritdoc/>
        public IUpdateSetFromQuery<T, TSource> From<TSource>(Action<ISelectWithAlias> query)
        {
            FromQuery(query);
            return UpdateSetFromQueryTyped<T, TSource>.Create(Query, Context);
        }

        /// <inheritdoc/>
        public IUpdateSet<T> Output(Expression<Func<T, object>> column)
        {
            var columnToOutput = GetDestinationColumn(column);
            Context.Output(state.Output.Create(columnToOutput, OutputSources.Updated));
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSet<T> Set<TValue>(Expression<Func<T, TValue>> column, TValue value)
        {
            var col = GetDestinationColumn(column);
            var source = state.ValueSources.Update.StaticValueSource.Create<TValue>(col, value);
            Context.Set(source);
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSet<T> Set(T values)
        {
            SetValues(new[] { values });
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSet<T> Set(dynamic values)
        {
            SetValues(new[] { values });
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSet<T> Where(Expression<Func<T, bool>> expression)
        {
            var where = Context.Helpers().ParseWhere(expression);
            Context.Where(where);
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSet<T> Where(IWhere clause)
        {
            Context.Where(clause);
            return this;
        }

        private Column GetDestinationColumn(Expression expression)
        {
            var internalContext = Context as IQueryContextExtended;
            var columnName = internalContext.ExpressionHelper.GetMemberName(expression);
            return Column.Create(columnName, Context.Table);
        }
    }
}

