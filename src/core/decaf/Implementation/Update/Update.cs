using System;
using System.Linq;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;
using decaf.state.ValueSources.Update;

namespace decaf.Implementation.Execute
{
    internal class Update :
        UpdateBase,
        IUpdate,
        IUpdateTable,
        IUpdateSet,
        IUpdateSetFromQuery
    {
        private Update(
            IUpdateQueryContext context,
            IQueryContainerInternal query)
            : base(query, context)
        {
            Context = context;
            Query.SetContext(Context);
        }

        public static Update Create(
            IUpdateQueryContext context,
            IQueryContainer query)
            => new Update(context, query as IQueryContainerInternal);

        /// <inheritdoc/>
        public IUpdateSetFromQuery From(Action<ISelectWithAlias> query)
        {
            FromQuery(query);
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSet Output(string column)
        {
            Context.Output(state.Output.Create(column, OutputSources.Updated));
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSet Set<T>(string column, T value)
        {
            var col = Column.Create(column, Context.Table);
            var valueSource = StaticValueSource.Create(col, value);
            Context.Set(valueSource);
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSet Set(dynamic values)
        {
            SetValues(new[] { values });
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSetFromQuery Set(string columnToUpdate, string sourceColumn)
        {
            var destination = Column.Create(columnToUpdate, Context.Table);
            var source = Column.Create(sourceColumn, Context.Source);
            Context.Set(QueryValueSource.Create(destination, source, Context.Source));
            return this;
        }

        /// <inheritdoc/>
        public IUpdateTable Table(string table, string alias = null)
        {
            var target = state.QueryTargets.TableTarget.Create(table, alias);
            Context.Update(target);
            return this;
        }

        /// <inheritdoc/>
        public IUpdateTable<T> Table<T>()
        {
            var table = Context.Helpers().GetTableName<T>();
            var alias = Query.AliasManager.Add(null, table);
            Context.Update(state.QueryTargets.TableTarget.Create(table, alias));
            return UpdateTyped<T>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public IUpdateTable<T> Table<T>(Expression<Func<T, object>> expression)
        {
            var table = Context.Helpers().GetTableName(expression);
            var alias = Context.Helpers().GetTableAlias(expression);

            var managedTable = Query.AliasManager.GetAssociation(alias) ?? table;
            var managedAlias = Query.AliasManager.Add(alias, table);
            Context.Update(state.QueryTargets.TableTarget.Create(managedTable, managedAlias));

            return UpdateTyped<T>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public IUpdateTable<T> Table<T>(string alias)
        {
            var table = Context.Helpers().GetTableName<T>();
            var managedTable = Query.AliasManager.GetAssociation(alias) ?? table;
            var managedAlias = Query.AliasManager.Add(alias, table);
            Context.Update(state.QueryTargets.TableTarget.Create(managedTable, managedAlias));

            return UpdateTyped<T>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public IUpdateSet Where(Action<IWhereBuilder> builder)
        {
            var whereBuilder = WhereBuilder.Create(Query.Options, Context) as IWhereBuilderInternal;
            builder(whereBuilder);
            Context.Where(whereBuilder.GetClauses().First());
            return this;
        }

        /// <inheritdoc/>
        public IUpdateSet Where(IWhere clause)
        {
            if (clause == null) throw new ArgumentNullException(nameof(clause));
            Context.Where(clause);
            return this;
        }

        /// <inheritdoc/>
        IUpdateSetFromQuery IUpdateSetFromQuery.Output(string column)
        {
            Output(column);
            return this;
        }

        /// <inheritdoc/>
        IUpdateSetFromQuery IUpdateSetFromQuery.Where(Action<IWhereBuilder> builder)
        {
            Where(builder);
            return this;
        }
    }
}

