using System;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;

namespace decaf.Implementation.Execute
{
    internal class UpdateSetFromQueryTyped<TDestination, TSource> :
        UpdateBase,
        IUpdateSetFromQuery<TDestination, TSource>
    {
        private UpdateSetFromQueryTyped(IQueryContainerInternal query, IUpdateQueryContext context)
            : base(query, context)
        {
        }

        public static IUpdateSetFromQuery<TDestination, TSource> Create(IQueryContainerInternal query, IUpdateQueryContext context)
            => new UpdateSetFromQueryTyped<TDestination, TSource>(query, context);

        /// <inheritdoc/>
        public IUpdateSetFromQuery<TDestination, TSource> Output(Expression<Func<TDestination, object>> column)
        {
            var columnToOutput = GetColumnFromExpression(column, Context.Table);
            Context.Output(state.Output.Create(columnToOutput, OutputSources.Updated));
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSetFromQuery<TDestination, TSource> Set(
            Expression<Func<TDestination, object>> columnToUpdate,
            Expression<Func<TSource, object>> sourceColumn)
        {
            var destination = GetColumnFromExpression(columnToUpdate, Context.Table);
            var source = GetColumnFromExpression(sourceColumn, Context.Source);
            Context.Set(state.ValueSources.Update.QueryValueSource.Create(destination, source, Context.Source));
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSetFromQuery<TDestination, TSource> Where(Expression<Func<TDestination, TSource, bool>> expression)
        {
            var where = Context.Helpers().ParseWhere(expression);
            Context.Where(where);
            return this;
        }

        private Column GetColumnFromExpression(Expression expression, IQueryTarget target)
        {
            var internalContext = Context as IQueryContextExtended;
            var columnName = internalContext.ExpressionHelper.GetMemberName(expression);
            return Column.Create(columnName, target);
        }
    }
}

