using decaf.common;

namespace decaf.Implementation.Execute
{
    public class ColumnWhereBuilder : IColumnWhereBuilder
    {
        private readonly IQueryContext queryContext;
        private readonly IWhereBuilder builder;
        private readonly state.Column column;

        private ColumnWhereBuilder(
            IQueryContext queryContext,
            IWhereBuilder builder,
            state.Column column)
        {
            this.queryContext = queryContext;
            this.builder = builder;
            this.column = column;
        }

        internal static IColumnWhereBuilder Create(
            IQueryContext queryContext,
            IWhereBuilder builder,
            state.Column column)
            => new ColumnWhereBuilder(queryContext, builder, column);

        /// <inheritdoc />
        public IColumnValueBuilder Is() => ColumnValueBuilder.Create(queryContext, builder, column, false);

        /// <inheritdoc />
        public IColumnValueBuilder IsNot() => ColumnValueBuilder.Create(queryContext, builder, column, true);
    }
}

