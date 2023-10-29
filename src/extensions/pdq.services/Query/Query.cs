﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using pdq.common;
using pdq.common.Connections;
using pdq.common.Utilities;
using pdq.state;

namespace pdq.services
{
    internal class Query<TEntity> :
        Service,
        IQuery<TEntity>
        where TEntity : class, IEntity, new()
    {
        const string TableAlias = "t";

        public event EventHandler<PreExecutionEventArgs> OnBeforeExecution
        {
            add => base.preExecution += value;
            remove => base.preExecution -= value;
        }

        public Query(IPdq pdq) : base(pdq) { }

        protected Query(ITransient transient) : base(transient) { }

        public static IQuery<TEntity> Create(ITransient transient) => new Query<TEntity>(transient);

        /// <inheritdoc/>
        public IEnumerable<TEntity> All()
            => AllAsync().WaitFor();

        /// <inheritdoc/>
        public async Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken = default)
        {
            return await ExecuteQueryAsync(async (q, c) =>
            {
                var sel = q.Select()
                    .From<TEntity>(t => t)
                    .SelectAll<TEntity>(t => t);
                NotifyPreExecution(this, q);
                return await sel.AsEnumerableAsync(c);
            }, cancellationToken);
        }

        /// <inheritdoc/>
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
            => FindAsync(expression).WaitFor();

        /// <inheritdoc/>
        public async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> expression,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteQueryAsync(async (q, c) =>
            {
                var sel = q.Select()
                    .From<TEntity>(t => t)
                    .Where(expression)
                    .SelectAll<TEntity>(t => t);
                NotifyPreExecution(this, q);
                return await sel.AsEnumerableAsync(c);
            }, cancellationToken);
        }

        protected IEnumerable<TEntity> GetByKeys<TKey>(
            IEnumerable<TKey> keys,
            Action<IEnumerable<TKey>, IQueryContainer, IWhereBuilder> filter)
        {
            var numKeys = keys?.Count() ?? 0;
            if (numKeys == 0) return Enumerable.Empty<TEntity>();

            var t = this.GetTransient();
            const int take = 100;
            var skip = 0;
            var results = new List<TEntity>();

            do
            {
                var keyBatch = keys.Skip(skip).Take(take);

                using (var q = t.Query())
                {
                    var select = q.Select();
                    var table = q.Context.Helpers().GetTableName<TEntity>();

                    var selected = select.From(table, TableAlias)
                        .Where(b => filter(keyBatch, q, b))
                        .SelectAll<TEntity>(TableAlias);

                    NotifyPreExecution(this, q);
                    var batchResults = selected.AsEnumerable();

                    results.AddRange(batchResults);
                }

                skip += take;
            } while (skip < numKeys);

            if (this.disposeOnExit) t.Dispose();

            return results;
        }
    }
}

