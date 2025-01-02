using System;
using System.Collections.Generic;
using System.Linq;
using decaf.common;
using decaf.Exceptions;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("decaf.core.tests")]
namespace decaf.Implementation.Execute
{
    public class WhereBuilder : IWhereBuilderInternal
	{
        private readonly List<IWhere> clauses;
        private readonly IQueryContext queryContext;
        private readonly IClauseHandlingBehaviour clauseHandlingBehaviour;
        private ClauseHandling defaultClauseHandling;

		private WhereBuilder(ClauseHandling clauseHandling, IQueryContext queryContext)
		{
            clauseHandlingBehaviour = ClauseHandlingBehaviour.CreateClauseHandler(this);
            defaultClauseHandling = clauseHandling;
            this.queryContext = queryContext;
            clauses = new List<IWhere>();
		}

        internal static IWhereBuilder Create(DecafOptions options, IQueryContext queryContext)
            => new WhereBuilder(options.DefaultClauseHandling, queryContext);

        private static IWhereBuilder Create(ClauseHandling clauseHandling, IQueryContext queryContext)
            => new WhereBuilder(clauseHandling, queryContext);

        /// <inheritdoc />
        public IWhereBuilder And(Action<IWhereBuilder> builder)
        {
            var b = Create(defaultClauseHandling, queryContext) as IWhereBuilderInternal;
            b.ClauseHandling.DefaultToAnd();
            builder(b);

            clauses.Add(b.GetClauses().First());
            return this;
        }

        /// <inheritdoc />
        public IClauseHandlingBehaviour ClauseHandling => clauseHandlingBehaviour;

        ClauseHandling IWhereBuilderInternal.DefaultClauseHandling => defaultClauseHandling;

        /// <inheritdoc />
        public IColumnWhereBuilder Column(string name, string targetAlias = null)
        {
            var target = string.IsNullOrWhiteSpace(targetAlias) ?
                null :
                queryContext.QueryTargets.FirstOrDefault(t => t.Alias == targetAlias);
            var c = state.Column.Create(name, target);
            return ColumnWhereBuilder.Create(queryContext, this, c);
        }

        /// <inheritdoc />
        public IWhereBuilder Or(Action<IWhereBuilder> builder)
        {
            var b = Create(defaultClauseHandling, queryContext) as IWhereBuilderInternal;
            b.ClauseHandling.DefaultToOr();
            builder(b);

            clauses.Add(b.GetClauses().First());
            return this;
        }

        /// <inheritdoc />
        void IWhereBuilderInternal.AddClause(IWhere item) => clauses.Add(item);

        /// <inheritdoc />
        IEnumerable<IWhere> IWhereBuilderInternal.GetClauses()
        {
            if (defaultClauseHandling == common.ClauseHandling.And)
                return new[] { state.Conditionals.And.Where(clauses) };

            if (defaultClauseHandling == common.ClauseHandling.Or)
                return new[] { state.Conditionals.Or.Where(clauses) };

            throw new WhereBuildFailedException();
        }

        private sealed class ClauseHandlingBehaviour : IClauseHandlingBehaviour
        {
            private readonly WhereBuilder builder;

            private ClauseHandlingBehaviour(WhereBuilder builder)
                => this.builder = builder;

            public static IClauseHandlingBehaviour CreateClauseHandler(WhereBuilder builder)
                => new ClauseHandlingBehaviour(builder);

            /// <inheritdoc />
            public void DefaultToAnd()
                => builder.defaultClauseHandling = common.ClauseHandling.And;

            /// <inheritdoc />
            public void DefaultToOr()
                => builder.defaultClauseHandling = common.ClauseHandling.Or;
        }
    }
}

