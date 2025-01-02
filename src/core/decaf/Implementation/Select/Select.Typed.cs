using System;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;
using decaf.state.QueryTargets;

namespace decaf.Implementation.Execute
{
    internal abstract class SelectTyped : SelectCommon
	{
        protected SelectTyped(
            ISelectQueryContext context,
            IQueryContainerInternal query)
            : base(context, query)
        {
        }

        protected void AddJoin<T1, T2>(Expression expression, JoinType type)
        {
            var conditions = Context.Helpers().ParseJoin(expression);
            if (conditions == null)
                conditions = Context.Helpers().ParseWhere(expression);
            var left = GetQueryTarget<T1>();
            var right = GetQueryTarget<T2>();

            Context.Join(state.Join.Create(left, right, type, conditions));
        }

        protected void AddJoin<T1, T2>(Action<ISelectWithAlias> query, Expression expression, JoinType type)
        {
            var select = Select.Create(Context, Query);
            query(select);

            var table = Context.Helpers().GetTableName<T2>();
            var alias = Query.AliasManager.Add(select.Alias, table);

            var left = GetQueryTarget<T1>();
            var right = SelectQueryTarget.Create(select.GetContext(), alias);

            var conditions = Context.Helpers().ParseJoin(expression);
            if (conditions == null)
                conditions = Context.Helpers().ParseWhere(expression);

            Context.Join(state.Join.Create(left, right, type, conditions));
        }

        protected void AddWhere(Expression expression)
        {
            var clause = Context.Helpers().ParseWhere(expression);
            Context.Where(clause);
        }

        protected void AddGroupBy(Expression expression)
        {
            var column = Context.Helpers().GetColumnName(expression);
            var target = GetQueryTargetFromExpression(expression);
            Context.GroupBy(GroupBy.Create(column, target));
        }

        protected void AddOrderBy(Expression expression, SortOrder order)
        {
            var column = Context.Helpers().GetColumnName(expression);
            var target = GetQueryTargetFromExpression(expression);
            Context.OrderBy(OrderBy.Create(column, target, order));
        }

        private IQueryTarget GetQueryTargetFromExpression(Expression expression)
        {
            var table = Context.Helpers().GetTableName(expression);
            return GetQueryTargetByTable(table);
        }

        protected IQueryTarget GetQueryTarget<T>()
        {
            var table = Context.Helpers().GetTableName<T>();
            return GetQueryTargetByTable(table);
        }
    }
}

