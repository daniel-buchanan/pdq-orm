using System;
using System.Linq;
using decaf.common;
using decaf.state;
using decaf.state.QueryTargets;

namespace decaf.Implementation.Execute
{
    internal class Join : IJoinFrom, IJoinTo, IJoinConditions
    {
        private readonly ISelectFrom selectFrom;
        private readonly IQueryContextExtended context;
        private readonly DecafOptions options;
        private readonly IQueryContainerInternal query;
        private readonly JoinType joinType;

        private IQueryTarget left;
        private IQueryTarget right;

        private Join(
            ISelectFrom selectFrom,
            IQueryContext context,
            DecafOptions options,
            IQueryContainerInternal query,
            JoinType joinType)
        {
            this.selectFrom = selectFrom;
            this.context = context as IQueryContextExtended;
            this.options = options;
            this.query = query;
            this.joinType = joinType;
        }

        public static Join Create(
            ISelectFrom selectFrom,
            IQueryContext context,
            DecafOptions options,
            IQueryContainerInternal query,
            JoinType joinType) => new Join(selectFrom, context, options, query, joinType);

        /// <inheritdoc/>
        public IJoinTo From(string name, string alias, string schema = null)
        {
            var managedTable = context.AliasManager.GetAssociation(alias) ?? name;
            var managedAlias = context.AliasManager.Add(alias, managedTable);
            var existingTarget = context.QueryTargets.FirstOrDefault(t => t.Alias == managedAlias);
            if(existingTarget == null)
            {
                existingTarget = TableTarget.Create(managedTable, managedAlias, schema);
                context.AddQueryTarget(existingTarget);
            }
            left = existingTarget;
            return this;
        }

        /// <inheritdoc/>
        public ISelectFrom On(Action<IWhereBuilder> builder)
        {
            var b = WhereBuilder.Create(options, context) as IWhereBuilderInternal;
            builder(b);

            var selectContext = context as ISelectQueryContext;
            var conditions = b.GetClauses().First();
            var join = state.Join.Create(left, right, joinType, conditions);
            selectContext.Join(join);

            return selectFrom;
        }

        /// <inheritdoc/>
        public IJoinConditions To(string name, string alias, string schema = null)
        {
            var managedTable = context.AliasManager.GetAssociation(alias) ?? name;
            var managedAlias = context.AliasManager.Add(alias, managedTable);
            var existingTarget = context.QueryTargets.FirstOrDefault(t => t.Alias == managedAlias);
            if (existingTarget == null)
            {
                existingTarget = TableTarget.Create(managedTable, managedAlias, schema);
                context.AddQueryTarget(existingTarget);
            }
            right = existingTarget;
            return this;
        }

        /// <inheritdoc/>
        public IJoinConditions To(Action<ISelectWithAlias> query)
        {
            var selectContext = SelectQueryContext.Create(context.AliasManager, this.query.HashProvider);
            var selectQuery = QueryContainer.Create(this.query) as IQueryContainerInternal;
            var select = Select.Create(selectContext, selectQuery);
            query(select);
            
            right = SelectQueryTarget.Create(selectContext, select.Alias);
            return this;
        }
    }
}

