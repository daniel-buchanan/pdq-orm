using System;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;

namespace decaf.Implementation.Execute
{
    internal class SelectTyped<T1, T2>
        : SelectTyped,
        ISelectFromTyped<T1, T2>,
        IGroupByTyped<T1, T2>,
        IGroupByThenTyped<T1, T2>,
        IOrderByThenTyped<T1, T2>
	{
        private SelectTyped(
            ISelectQueryContext context,
            IQueryContainerInternal query)
            : base(context, query)
        {
        }

        public static SelectTyped<T1, T2> Create(
            ISelectQueryContext context,
            IQueryContainerInternal query)
            => new SelectTyped<T1, T2>(context, query);

        /// <inheritdoc/>
        public ISelectFromTyped<T1, T2, T3> From<T3>()
        {
            AddFrom<T3>();
            return SelectTyped<T1, T2, T3>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T1, T2, T3> From<T3>(Expression<Func<T3, T3>> expression)
        {
            AddFrom(expression);
            return SelectTyped<T1, T2, T3>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public IGroupByThenTyped<T1, T2> GroupBy(Expression<Func<T1, T2, object>> builder)
        {
            AddGroupBy(builder);
            return this;
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T1, T2, TDestination> Join<T, TDestination>(Expression<Func<T, TDestination, bool>> joinExpression, JoinType type = JoinType.Default) where T : T1
        {
            AddJoin<T, TDestination>(joinExpression, type);
            return SelectTyped<T1, T2, TDestination>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T1, T2, TDestination> Join<TDestination>(Expression<Func<T2, TDestination, bool>> joinExpression, JoinType type = JoinType.Default)
        {
            AddJoin<T2, TDestination>(joinExpression, type);
            return SelectTyped<T1, T2, TDestination>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T1, T2, TDestination> Join<TDestination>(Action<ISelectWithAlias> query, Expression<Func<T2, TDestination, bool>> joinExpression, JoinType type = JoinType.Default)
        {
            AddJoin<T2, TDestination>(query, joinExpression, type);
            return SelectTyped<T1, T2, TDestination>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public IOrderByThenTyped<T1, T2> OrderBy(Expression<Func<T1, T2, object>> builder, SortOrder order = SortOrder.Ascending)
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
        public IGroupByThenTyped<T1, T2> ThenBy(Expression<Func<T1, T2, object>> builder)
            => GroupBy(builder);

        /// <inheritdoc/>
        public IOrderByThenTyped<T1, T2> ThenBy(Expression<Func<T1, T2, object>> builder, SortOrder order = SortOrder.Ascending)
            => OrderBy(builder, order);

        /// <inheritdoc/>
        public IGroupByTyped<T1, T2> Where(Expression<Func<T1, T2, bool>> builder)
        {
            AddWhere(builder);
            return this;
        }

        /// <inheritdoc/>
        IExecuteDynamic ISelectColumnTyped<T1, T2>.SelectDynamic(Expression<Func<T1, T2, dynamic>> expression)
        {
            AddColumns(expression);
            return this;
        }

        /// <inheritdoc/>
        IExecute<TResult> ISelectColumnTyped<T1, T2>.Select<TResult>(Expression<Func<T1, T2, TResult>> expression)
        {
            AddColumns(expression);
            return Execute<TResult, ISelectQueryContext>.Create(Query, Context);
        }
    }
}

