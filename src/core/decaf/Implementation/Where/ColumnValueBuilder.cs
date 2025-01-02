using System.Collections.Generic;
using decaf.common;
using decaf.common.ValueFunctions;
using decaf.state.Conditionals;

namespace decaf.Implementation.Execute
{
	public class ColumnValueBuilder : IColumnValueBuilder
	{
        private readonly IQueryContext queryContext;
        private readonly IWhereBuilderInternal builder;
        private readonly state.Column column;
        private readonly bool notEquals;

		private ColumnValueBuilder(
            IQueryContext queryContext,
            IWhereBuilder builder,
            state.Column column,
            bool notEquals)
		{
            this.queryContext = queryContext;
            this.builder = builder as IWhereBuilderInternal;
            this.column = column;
            this.notEquals = notEquals;
		}

        internal static IColumnValueBuilder Create(
            IQueryContext queryContext,
            IWhereBuilder builder,
            state.Column column,
            bool notEquals)
            => new ColumnValueBuilder(queryContext, builder, column, notEquals);

        /// <inheritdoc />
        public void EndsWith<T>(T value)
            => AddClause(EqualityOperator.EndsWith, StringEndsWith.Create(value as string), value);

        /// <inheritdoc />
        public IColumnMatchBuilder EqualTo()
            => ColumnMatchBuilder.Create(queryContext, builder, column, notEquals, EqualityOperator.Equals);

        /// <inheritdoc />
        public void EqualTo<T>(T value)
            => AddClause(EqualityOperator.Equals, null, value);

        /// <inheritdoc />
        public IColumnMatchBuilder GreaterThan()
            => ColumnMatchBuilder.Create(queryContext, builder, column, notEquals, EqualityOperator.GreaterThan);

        /// <inheritdoc />
        public void GreaterThan<T>(T value)
            => AddClause(EqualityOperator.GreaterThan, null, value);

        /// <inheritdoc />
        public IColumnMatchBuilder GreaterThanOrEqualTo()
            => ColumnMatchBuilder.Create(queryContext, builder, column, notEquals, EqualityOperator.GreaterThanOrEqualTo);

        /// <inheritdoc />
        public void GreaterThanOrEqualTo<T>(T value)
            => AddClause(EqualityOperator.GreaterThanOrEqualTo, null, value);

        /// <inheritdoc />
        public void In<T>(params T[] values)
            => AddClause(values);

        /// <inheritdoc />
        public void In<T>(IEnumerable<T> values)
            => AddClause(values);

        /// <inheritdoc />
        public void Between<T>(T start, T end)
        {
            var between = state.Conditionals.Between.Values(column, start, end);
            if (notEquals) builder.AddClause(Not.This(between));
            else builder.AddClause(between);
        }

        /// <inheritdoc />
        public IColumnMatchBuilder LessThan()
            => ColumnMatchBuilder.Create(queryContext, builder, column, notEquals, EqualityOperator.LessThan);

        /// <inheritdoc />
        public void LessThan<T>(T value)
            => AddClause(EqualityOperator.LessThan, null, value);

        /// <inheritdoc />
        public IColumnMatchBuilder LessThanOrEqualTo()
            => ColumnMatchBuilder.Create(queryContext, builder, column, notEquals, EqualityOperator.LessThanOrEqualTo);

        /// <inheritdoc />
        public void LessThanOrEqualTo<T>(T value)
            => AddClause(EqualityOperator.LessThanOrEqualTo, null, value);

        /// <inheritdoc />
        public void Like<T>(T value)
            => AddClause(EqualityOperator.Like, StringContains.Create(value as string), value);

        /// <inheritdoc />
        public void Null() => AddClause<object>(EqualityOperator.Equals, null, null);

        /// <inheritdoc />
        public void NullOrWhitespace()
        {
            var nullClause = new Column<object>(column, EqualityOperator.Equals, null);
            var whitespaceClause = new Column<string>(column, EqualityOperator.Equals, Trim.Create(), string.Empty);
            var orClause = Or.Where(nullClause, whitespaceClause);
            if (notEquals) builder.AddClause(Not.This(orClause));
            else builder.AddClause(orClause);
        }

        /// <inheritdoc />
        public void StartsWith<T>(T value)
            => AddClause(EqualityOperator.StartsWith, StringStartsWith.Create(value as string), value);

        private void AddClause<T>(EqualityOperator op, IValueFunction function, T value)
        {
            var col = new Column<T>(column, op, function, value);
            if (notEquals) builder.AddClause(Not.This(col));
            else builder.AddClause(col);
        }

        private void AddClause<T>(IEnumerable<T> values)
        {
            var col = Values.In(column, values);
            if (notEquals) builder.AddClause(Not.This(col));
            else builder.AddClause(col);
        }
    }
}

