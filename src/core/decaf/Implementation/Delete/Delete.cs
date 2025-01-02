using System;
using System.Linq;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;

namespace decaf.Implementation.Execute
{
	internal class Delete :
        Execute<IDeleteQueryContext>,
        IDelete,
        IDeleteFrom
	{
        private Delete(
            IDeleteQueryContext context,
            IQueryContainerInternal query)
            : base(query, context)
        {
            Context = context;
            Query.SetContext(Context);
        }

        public static Delete Create(
            IDeleteQueryContext context,
            IQueryContainer query)
            => new Delete(context, query as IQueryContainerInternal);

        /// <inheritdoc />
        public IDeleteFrom From(string name, string alias = null, string schema = null)
        {
            var managedTable = Query.AliasManager.GetAssociation(alias) ?? name;
            var managedAlias = Query.AliasManager.Add(alias, name);
            Context.From(state.QueryTargets.TableTarget.Create(managedTable, managedAlias, schema));
            return this;
        }

        /// <inheritdoc />
        public IDeleteFrom<T> From<T>(Expression<Func<T, T>> aliasExpression)
        {
            var table = Context.Helpers().GetTableName(aliasExpression);
            var alias = Context.Helpers().GetTableAlias(aliasExpression);

            var managedTable = Query.AliasManager.GetAssociation(alias) ?? table;
            var managedAlias = Query.AliasManager.Add(alias, table);

            Context.From(state.QueryTargets.TableTarget.Create(managedTable, managedAlias));
            return Delete<T>.Create(Context, Query);
        }

        /// <inheritdoc />
        public IDeleteFrom<T> From<T>()
        {
            var table = Context.Helpers().GetTableName<T>();
            var alias = Query.AliasManager.Add(null, table);
            Context.From(state.QueryTargets.TableTarget.Create(table, alias));
            return Delete<T>.Create(Context, Query);
        }

        /// <inheritdoc />
        public IDeleteFrom Output(string column)
        {
            var col = Column.Create(column, Context.Table);
            Context.Output(state.Output.Create(col, OutputSources.Deleted));
            return this;
        }

        /// <inheritdoc />
        public IDeleteFrom Where(Action<IWhereBuilder> builder)
        {
            var b = WhereBuilder.Create(Query.Options, Context) as IWhereBuilderInternal;
            builder(b);
            var clause = b.GetClauses().First();
            if ((clause is state.Conditionals.And ||
               clause is state.Conditionals.Or) &&
               clause.Children.Count == 1)
            {
                clause = clause.Children.First();
            }

            Context.Where(clause);
            return this;
        }
    }
}

