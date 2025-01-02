using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;

namespace decaf.Implementation.Execute
{
	internal class Insert<T> :
        InsertBase,
		IInsertInto<T>,
		IInsertValues<T>
	{
        private Insert(IQueryContainerInternal query, IInsertQueryContext context)
            : base(query, context) { }

        public static Insert<T> Create(IInsertQueryContext context, IQueryContainerInternal query)
            => new Insert<T>(query, context);

        /// <inheritdoc/>
        public IInsertValues<T> Columns(Expression<Func<T, dynamic>> columns)
        {
            AddColumns(columns);
            return this;
        }

        /// <inheritdoc/>
        public IInsertValues<T> From(Action<ISelect> query)
        {
            FromQuery(query);
            return this;
        }

        /// <inheritdoc/>
        public IInsertValues<T> Output(Expression<Func<T, object>> column)
        {
            var internalContext = Context as IQueryContextExtended;
            var columnName = internalContext.ExpressionHelper.GetMemberName(column);
            var col = Column.Create(columnName, Context.Target);
            Context.Output(state.Output.Create(col, OutputSources.Inserted));
            return this;
        }

        /// <inheritdoc/>
        public IInsertValues<T> Value(T value)
        {
            AddValues(value);
            return this;
        }

        /// <inheritdoc/>
        public IInsertValues<T> Values(IEnumerable<T> values)
        {
            AddValues(values);
            return this;
        }
    }
}

