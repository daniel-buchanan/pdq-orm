using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using decaf.common;
using decaf.common.Utilities;
using decaf.state;
using decaf.state.Conditionals;
using decaf.state.QueryTargets;

[assembly: InternalsVisibleTo("decaf.core.tests")]
namespace decaf.Implementation.Execute
{
    internal class Select :
        SelectCommon,
        ISelectWithAlias,
        ISelectFrom,
        IOrderByThen,
        IGroupBy,
        IGroupByThen
	{
        private Select(
            ISelectQueryContext context,
            IQueryContainerInternal query)
            : base(context, query) { }

        public static Select Create(
            ISelectQueryContext context,
            IQueryContainer query)
            => new Select(context, query as IQueryContainerInternal);
        
        internal ISelectQueryContext GetContext() => Context;

        /// <inheritdoc/>
        public string Alias { get; private set; }

        /// <inheritdoc/>
        public IJoinFrom Join()
            => Execute.Join.Create(this, Context, Options, Query, JoinType.Default);

        /// <inheritdoc/>
        public IJoinFrom InnerJoin()
            => Execute.Join.Create(this, Context, Options, Query, JoinType.Inner);

        /// <inheritdoc/>
        public IJoinFrom LeftJoin()
            => Execute.Join.Create(this, Context, Options, Query, JoinType.Left);

        /// <inheritdoc/>
        public IJoinFrom RightJoin()
            => Execute.Join.Create(this, Context, Options, Query, JoinType.Right);

        /// <inheritdoc/>
        public IGroupBy Where(Action<IWhereBuilder> builder)
        {
            var b = WhereBuilder.Create(Options, Context) as IWhereBuilderInternal;
            builder(b);
            var clause = b!.GetClauses().First();
            if((clause is And ||
               clause is Or) &&
               clause.Children.Count == 1)
            {
                clause = clause.Children.First();
            }

            Context.Where(clause);
            return this;
        }

        /// <inheritdoc/>
        public ISelectFrom From(
            string table,
            string alias,
            string schema = null)
        {
            var managedTable = Query.AliasManager.GetAssociation(alias) ?? table;
            var managedAlias = Query.AliasManager.Add(alias, managedTable);
            Context.From(TableTarget.Create(managedTable, managedAlias, schema));
            return this;
        }

        /// <inheritdoc/>
        public ISelectFrom From(Action<ISelect> query, string alias)
        {
            var selectContext = SelectQueryContext.Create(AliasManager.Create(), HashProvider.Create());
            var select = Create(selectContext, Query);
            query(select);

            Query.AliasManager.Add(alias, "query");
            var builtQuery = SelectQueryTarget.Create(select.Context, alias);
            Context.From(builtQuery);

            return this;
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T> From<T>()
        {
            AddFrom<T>();
            return SelectTyped<T>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T> From<T>(Expression<Func<T, T>> aliasExpression)
        {
            AddFrom(aliasExpression);
            return SelectTyped<T>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T> From<T>(Action<ISelectWithAlias> query, string alias)
        {
            var selectContext = SelectQueryContext.Create(AliasManager.Create(), Query.HashProvider);
            var selectQuery = QueryContainer.Create(Query) as IQueryContainerInternal;
            var select = Create(selectContext, selectQuery);
            query(select);

            var target = SelectQueryTarget.Create(selectContext, select.Alias ?? alias);

            var table = Context.Helpers().GetTableName<T>();
            Query.AliasManager.Add(table, select.Alias ?? alias);

            Context.From(target);
            return SelectTyped<T>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public ISelectWithAlias KnownAs(string alias)
        {
            Alias = alias;
            return this;
        }

        /// <inheritdoc/>
        public IOrderByThen OrderBy(string column, string tableAlias, SortOrder orderBy)
        {
            var managedTable = Query.AliasManager.GetAssociation(tableAlias);
            var managedAlias = Query.AliasManager.Add(tableAlias, managedTable);
            Context.OrderBy(state.OrderBy.Create(column, TableTarget.Create(managedTable, managedAlias), orderBy));
            return this;
        }

        /// <inheritdoc/>
        public IGroupByThen GroupBy(string column, string tableAlias)
        {
            var managedTable = Query.AliasManager.GetAssociation(tableAlias);
            var managedAlias = Query.AliasManager.Add(tableAlias, managedTable);
            Context.GroupBy(state.GroupBy.Create(column, TableTarget.Create(managedTable, managedAlias)));
            return this;
        }

        /// <inheritdoc/>
        public IOrderByThen ThenBy(string column, string tableAlias, SortOrder orderBy)
            => OrderBy(column, tableAlias, orderBy);

        /// <inheritdoc/>
        public IGroupByThen ThenBy(string column, string tableAlias)
            => GroupBy(column, tableAlias);

        /// <inheritdoc/>
        IExecuteDynamic ISelectColumn.SelectDynamic(Expression<Func<ISelectColumnBuilder, dynamic>> expression)
        {
            AddColumns(expression);
            return this;
        }

        /// <inheritdoc/>
        IExecute<TResult> ISelectColumn.Select<TResult>(Expression<Func<ISelectColumnBuilder, TResult>> expression)
        {
            AddColumns(expression);
            return Execute<TResult, ISelectQueryContext>.Create(Query, Context);
        }

        /// <inheritdoc/>
        public IExecute<TResult> SelectAll<TResult>(string alias)
        {
            AddAllColumns<TResult>(alias);
            return Execute<TResult, ISelectQueryContext>.Create(Query, Context);
        }
    }
}

