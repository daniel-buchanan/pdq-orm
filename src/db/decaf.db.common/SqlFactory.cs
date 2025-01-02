using System.Collections.Generic;
using decaf.common.Templates;
using decaf.db.common.Builders;
using decaf.state;

namespace decaf.db.common
{
	public class SqlFactory : state.SqlFactory
	{
        private readonly IBuilderPipeline<ISelectQueryContext> selectBuilder;
        private readonly IBuilderPipeline<IInsertQueryContext> insertBuilder;
        private readonly IBuilderPipeline<IDeleteQueryContext> deleteBuilder;
        private readonly IBuilderPipeline<IUpdateQueryContext> updateBuilder;
        private readonly IBuilderPipeline<ICreateTableQueryContext> createTableBuilder;
        private readonly IBuilderPipeline<IDropTableQueryContext> dropTableBuilder;

        public SqlFactory(
            IBuilderPipeline<ISelectQueryContext> selectBuilder,
            IBuilderPipeline<IDeleteQueryContext> deleteBuilder,
            IBuilderPipeline<IInsertQueryContext> insertBuilder,
            IBuilderPipeline<IUpdateQueryContext> updateBuilder,
            IBuilderPipeline<ICreateTableQueryContext> createTableBuilder = null,
            IBuilderPipeline<IDropTableQueryContext> dropTableBuilder = null)
        {
            this.selectBuilder = selectBuilder;
            this.deleteBuilder = deleteBuilder;
            this.insertBuilder = insertBuilder;
            this.updateBuilder = updateBuilder;
            this.createTableBuilder = createTableBuilder;
            this.dropTableBuilder = dropTableBuilder;
        }

        protected virtual bool IncludeParameterPrefix => false;

        /// <inheritdoc/>
        protected override IDictionary<string, object> ParseParameters(IInsertQueryContext context, bool includePrefix)
            => insertBuilder.GetParameterValues(context, includePrefix);

        /// <inheritdoc/>
        protected override IDictionary<string, object> ParseParameters(IDeleteQueryContext context, bool includePrefix)
            => deleteBuilder.GetParameterValues(context, includePrefix);

        /// <inheritdoc/>
        protected override IDictionary<string, object> ParseParameters(IUpdateQueryContext context, bool includePrefix)
            => updateBuilder.GetParameterValues(context, includePrefix);

        /// <inheritdoc/>
        protected override IDictionary<string, object> ParseParameters(ISelectQueryContext context, bool includePrefix)
            => selectBuilder.GetParameterValues(context, includePrefix);

        /// <inheritdoc/>
        protected override SqlTemplate ParseQuery(IInsertQueryContext context)
            => insertBuilder.Execute(context);

        /// <inheritdoc/>
        protected override SqlTemplate ParseQuery(IDeleteQueryContext context)
            => deleteBuilder.Execute(context);

        /// <inheritdoc/>
        protected override SqlTemplate ParseQuery(IUpdateQueryContext context)
            => updateBuilder.Execute(context);

        /// <inheritdoc/>
        protected override SqlTemplate ParseQuery(ISelectQueryContext context)
            => selectBuilder.Execute(context);

        /// <inheritdoc/>
        protected override SqlTemplate ParseQuery(ICreateTableQueryContext context)
            => createTableBuilder.Execute(context);
        
        /// <inheritdoc />
        protected override SqlTemplate ParseQuery(IDropTableQueryContext context)
            => dropTableBuilder.Execute(context);
    }
}

