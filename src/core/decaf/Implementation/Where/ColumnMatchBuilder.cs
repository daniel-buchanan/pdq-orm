using System.Linq;
using decaf.common;
using decaf.state.Conditionals;

namespace decaf.Implementation.Execute
{
    public class ColumnMatchBuilder : IColumnMatchBuilder
    {
        private readonly IQueryContext queryContext;
        private readonly IWhereBuilderInternal builder;
        private readonly state.Column left;
        private readonly bool notEquals;
        private readonly EqualityOperator @operator;

        private ColumnMatchBuilder(
            IQueryContext queryContext,
            IWhereBuilder builder,
            state.Column left,
            bool notEquals,
            EqualityOperator @operator)
        {
            this.queryContext = queryContext;
            this.builder = builder as IWhereBuilderInternal;
            this.left = left;
            this.notEquals = notEquals;
            this.@operator = @operator;
        }

        internal static IColumnMatchBuilder Create(
            IQueryContext queryContext,
            IWhereBuilder builder,
            state.Column column,
            bool notEquals,
            EqualityOperator @operator)
            => new ColumnMatchBuilder(queryContext, builder, column, notEquals, @operator);

        /// <inheritdoc/>
        public void Column(string name, string targetAlias = null)
        {
            var target = string.IsNullOrWhiteSpace(targetAlias) ?
                null :
                queryContext.QueryTargets.FirstOrDefault(t => t.Alias == targetAlias);
            var right = state.Column.Create(name, target);
            var clause = state.Conditionals.Column.Equals(left, @operator, right);
            if (notEquals) builder.AddClause(Not.This(clause));
            else builder.AddClause(clause);
        }
    }
}

