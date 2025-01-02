using System;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;

namespace decaf.Implementation.Execute
{
	internal class Delete<T> :
        Execute<IDeleteQueryContext>,
        IDeleteFrom<T>
	{
        private Delete(
            IDeleteQueryContext context,
            IQueryContainerInternal query)
            : base(query, context)
        {
            Context = context;
        }

        public static IDeleteFrom<T> Create(
            IDeleteQueryContext context,
            IQueryContainerInternal query)
            => new Delete<T>(context, query);

        /// <inheritdoc/>
        public IDeleteFrom<T> Output(Expression<Func<T, object>> column)
        {
            var internalContext = Context as IQueryContextExtended;
            var columnName = internalContext.ExpressionHelper.GetMemberName(column);
            var col = Column.Create(columnName, Context.Table);
            Context.Output(state.Output.Create(col, OutputSources.Deleted));
            return this;
        }

        /// <inheritdoc />
        public IDeleteFrom<T> Where(Expression<Func<T, bool>> whereExpression)
        {
            var clause = Context.Helpers().ParseWhere(whereExpression);
            Context.Where(clause);
            return this;
        }
    }
}

