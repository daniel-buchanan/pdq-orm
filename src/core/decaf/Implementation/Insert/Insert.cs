using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using decaf.common;
using decaf.state;

namespace decaf.Implementation.Execute
{
    internal class Insert :
        InsertBase,
        IInsert,
        IInsertInto,
        IInsertValues
    {
        private Insert(IQueryContainerInternal query, IInsertQueryContext context)
            : base(query, context) { }

        public static Insert Create(IInsertQueryContext context, IQueryContainer query)
            => new Insert(query as IQueryContainerInternal, context);

        /// <inheritdoc/>
        public IInsertValues Columns(Expression<Func<IInsertColumnBuilder, dynamic>> columns)
        {
            AddColumns(columns);
            return this;
        }

        /// <inheritdoc/>
        public IInsertValues From(Action<ISelect> query)
        {
            FromQuery(query);
            return this;
        }

        /// <inheritdoc/>
        public IInsertInto Into(string table, string alias = null)
        {
            var managedTable = Query.AliasManager.GetAssociation(alias) ?? table;
            var managedAlias = Query.AliasManager.Add(alias, managedTable);
            Context.Into(state.QueryTargets.TableTarget.Create(managedTable, managedAlias));
            return this;
        }

        /// <inheritdoc/>
        public IInsertInto<T> Into<T>()
        {
            var table = Context.Helpers().GetTableName<T>();
            var alias = Query.AliasManager.Add(null, table);
            Context.Into(state.QueryTargets.TableTarget.Create(table, alias));

            return Insert<T>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public IInsertInto<T> Into<T>(Expression<Func<T, object>> expression)
        {
            var table = Context.Helpers().GetTableName(expression);
            var alias = Context.Helpers().GetTableAlias(expression);

            var managedTable = Query.AliasManager.GetAssociation(alias) ?? table;
            var managedAlias = Query.AliasManager.Add(alias, table);

            Context.Into(state.QueryTargets.TableTarget.Create(managedTable, managedAlias));
            return Insert<T>.Create(Context, Query);
        }

        /// <inheritdoc/>
        public IInsertValues Output(string column)
        {
            var col = Column.Create(column, Context.Target);
            Context.Output(state.Output.Create(col, OutputSources.Inserted));
            return this;
        }

        /// <inheritdoc/>
        public IInsertValues Value(dynamic value) => Values(new[] { value });

        /// <inheritdoc/>
        public IInsertValues Values(IEnumerable<dynamic> values)
        {
            AddValues(values);
            return this;
        }
    }
}

