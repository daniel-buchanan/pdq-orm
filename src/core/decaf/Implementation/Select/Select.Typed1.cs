using System;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;

namespace decaf.Implementation.Execute
{
    internal class SelectTyped<T>
        : SelectTyped,
        ISelectFromTyped<T>,
        IGroupByTyped<T>,
        IGroupByThenTyped<T>,
        IOrderByThenTyped<T>
	{
        private SelectTyped(
            ISelectQueryContext context,
            IQueryContainerInternal query)
            : base(context, query)
        {
        }

        public static SelectTyped<T> Create(
            ISelectQueryContext context,
            IQueryContainerInternal query)
            => new SelectTyped<T>(context, query);

        /// <inheritdoc/>
        public ISelectFromTyped<T, T1> From<T1>()
        {
            AddFrom<T1>();
            return SelectTyped<T, T1>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T, T1> From<T1>(Expression<Func<T1, T1>> expression)
        {
            AddFrom(expression);
            return SelectTyped<T, T1>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public IGroupByThenTyped<T> GroupBy(Expression<Func<T, object>> builder)
        {
            AddGroupBy(builder);
            return this;
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T, TDestination> Join<TDestination>(
            Expression<Func<T, TDestination, bool>> joinExpression,
            JoinType type = JoinType.Default)
        {
            AddJoin<T, TDestination>(joinExpression, type);
            return SelectTyped<T, TDestination>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T, TDestination> Join<TDestination>(
            Action<ISelectWithAlias> query,
            Expression<Func<T, TDestination, bool>> joinExpression,
            JoinType type = JoinType.Default)
        {
            AddJoin<T, TDestination>(query, joinExpression, type);
            return SelectTyped<T, TDestination>.Create(Context, Query);
        }

        public ISelectFromTyped<T, TDestination> Join<TDestination>(Func<ISelectWithAlias, Expression<Func<T, TDestination, bool>>> query, JoinType type = JoinType.Default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IOrderByThenTyped<T> OrderBy(Expression<Func<T, object>> builder, SortOrder order = SortOrder.Ascending)
        {
            AddOrderBy(builder, order);
            return this;
        }

        /// <inheritdoc/>
        public IExecute<TResult> SelectAll<TResult>(Expression<Func<TResult, object>> expression)
        {
            AddAllColumns(expression);
            return Execute<TResult, ISelectQueryContext>.Create(Query, Context);
        }

        /// <inheritdoc/>
        public IOrderByThenTyped<T> ThenBy(Expression<Func<T, object>> builder, SortOrder order = SortOrder.Ascending)
            => OrderBy(builder, order);

        /// <inheritdoc/>
        public IGroupByTyped<T> Where(Expression<Func<T, bool>> builder)
        {
            AddWhere(builder);
            return this;
        }

        /// <inheritdoc/>
        IExecuteDynamic ISelectColumnTyped<T>.SelectDynamic(Expression<Func<T, dynamic>> expression)
        {
            AddColumns(expression);
            return this;
        }

        /// <inheritdoc/>
        IExecute<TResult> ISelectColumnTyped<T>.Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            AddColumns(expression);
            return Execute<TResult, ISelectQueryContext>.Create(Query, Context);
        }

        /// <inheritdoc/>
        IGroupByThenTyped<T> IGroupByThenTyped<T>.ThenBy(Expression<Func<T, object>> builder)
            => GroupBy(builder);
    }
}

