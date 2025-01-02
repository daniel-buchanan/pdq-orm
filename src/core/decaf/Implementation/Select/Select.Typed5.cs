using System;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;

namespace decaf.Implementation.Execute
{
    internal class SelectTyped<T1, T2, T3, T4, T5>
        : SelectTyped,
        ISelectFromTyped<T1, T2, T3, T4, T5>,
        IGroupByTyped<T1, T2, T3, T4, T5>,
        IGroupByThenTyped<T1, T2, T3, T4, T5>,
        IOrderByThenTyped<T1, T2, T3, T4, T5>
    {
        private SelectTyped(
            ISelectQueryContext context,
            IQueryContainerInternal query)
            : base(context, query)
        {
        }

        public static SelectTyped<T1, T2, T3, T4, T5> Create(
            ISelectQueryContext context,
            IQueryContainerInternal query)
            => new SelectTyped<T1, T2, T3, T4, T5>(context, query);

        /// <inheritdoc/>
        public IGroupByThenTyped<T1, T2, T3, T4, T5> GroupBy(Expression<Func<T1, T2, T3, T4, T5, object>> builder)
        {
            AddGroupBy(builder);
            return this;
        }

        /// <inheritdoc/>
        public IOrderByThenTyped<T1, T2, T3, T4, T5> OrderBy(Expression<Func<T1, T2, T3, T4, T5, object>> builder, SortOrder order = SortOrder.Ascending)
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
        public IGroupByThenTyped<T1, T2, T3, T4, T5> ThenBy(Expression<Func<T1, T2, T3, T4, T5, object>> builder)
            => GroupBy(builder);

        /// <inheritdoc/>
        public IOrderByThenTyped<T1, T2, T3, T4, T5> ThenBy(Expression<Func<T1, T2, T3, T4, T5, object>> builder, SortOrder order = SortOrder.Ascending)
            => OrderBy(builder, order);

        /// <inheritdoc/>
        public IGroupByTyped<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> builder)
        {
            AddWhere(builder);
            return this;
        }

        /// <inheritdoc/>
        IExecuteDynamic ISelectColumnTyped<T1, T2, T3, T4, T5>.SelectDynamic(Expression<Func<T1, T2, T3, T4, T5, dynamic>> expression)
        {
            AddColumns(expression);
            return this;
        }

        /// <inheritdoc/>
        IExecute<TResult> ISelectColumnTyped<T1, T2, T3, T4, T5>.Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> expression)
        {
            AddColumns(expression);
            return Execute<TResult, ISelectQueryContext>.Create(Query, Context);
        }
    }
}

