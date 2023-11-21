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
    internal class Command<TEntity, TKey> :
        Command<TEntity>,
        ICommand<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        public Command(IPdq pdq) : base(pdq) { }

        private Command(ITransient transient) : base(transient) { }

        public new static ICommand<TEntity, TKey> Create(ITransient transient)
            => new Command<TEntity, TKey>(transient);

        /// <inheritdoc/>
        public override TEntity Add(TEntity toAdd)
            => AddAsync(toAdd).WaitFor();

        /// <inheritdoc/>
        public override IEnumerable<TEntity> Add(params TEntity[] toAdd)
            => AddAsync(toAdd?.AsEnumerable()).WaitFor();

        /// <inheritdoc/>
        public override IEnumerable<TEntity> Add(IEnumerable<TEntity> toAdd)
            => AddAsync(toAdd).WaitFor();

        /// <inheritdoc/>
        public override async Task<TEntity> AddAsync(TEntity toAdd, CancellationToken cancellationToken = default)
        {
            var results = await AddAsync(new[] { toAdd }, cancellationToken);
            return results.FirstOrDefault();
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
        {
            if (items == null) items = new List<TEntity>();
            if (!items.Any())
                return new List<TEntity>();

            var first = items.First();
            return await AddAsync(items, new[] { first.KeyMetadata.Name }, cancellationToken);
        }

        /// <inheritdoc/>
        public void Delete(TKey key) => Delete(new List<TKey> { key });

        /// <inheritdoc/>
        public void Delete(params TKey[] keys) => Delete(keys?.AsEnumerable());

        /// <inheritdoc/>
        public void Delete(IEnumerable<TKey> keys)
            => DeleteAsync(keys).WaitFor();

        /// <inheritdoc/>
        public async Task DeleteAsync(TKey key, CancellationToken cancellationToken = default)
            => await DeleteAsync(new[] { key }, cancellationToken);

        /// <inheritdoc/>
        public async Task DeleteAsync(TKey[] keys, CancellationToken cancellationToken = default)
            => await DeleteAsync(keys?.AsEnumerable(), cancellationToken);

        /// <inheritdoc/>
        public async Task DeleteAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        {
            await DeleteByKeysAsync(keys, (keyBatch, q, b) =>
            {
                GetKeyColumnNames<TEntity, TKey>(q, out var keyName);
                b.Column(keyName).Is().In(keyBatch);
            }, cancellationToken);
        }

        /// <inheritdoc/>
        public void Update(TEntity item)
            => UpdateAsync(item).WaitFor();

        /// <inheritdoc/>
        public void Update(dynamic toUpdate, TKey key)
        {
            var t = UpdateAsync(toUpdate, key);
            t.Wait();
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            var keyValue = item.GetKeyValue();
            await UpdateAsync(item, keyValue, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(dynamic toUpdate, TKey key, CancellationToken cancellationToken = default)
        {
            var temp = new TEntity();
            var parameterExpression = Expression.Parameter(typeof(TEntity), "t");
            var keyConstantExpression = Expression.Constant(key);
            var keyPropertyExpression = Expression.Property(parameterExpression, temp.KeyMetadata.Name);
            var keyEqualsExpression = Expression.Equal(keyPropertyExpression, keyConstantExpression);
            var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(keyEqualsExpression, parameterExpression);

            await UpdateAsync(toUpdate, lambdaExpression, cancellationToken);
        }
    }
}

