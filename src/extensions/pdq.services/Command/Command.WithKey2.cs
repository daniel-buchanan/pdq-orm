﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using pdq.common;
using pdq.common.Connections;
using pdq.common.Utilities;

namespace pdq.services
{
    internal class Command<TEntity, TKey1, TKey2> :
        Command<TEntity>,
        ICommand<TEntity, TKey1, TKey2>
        where TEntity : class, IEntity<TKey1, TKey2>, new()
    {
        public Command(IPdq pdq) : base(pdq) { }

        private Command(ITransient transient) : base(transient) { }

        public new static ICommand<TEntity, TKey1, TKey2> Create(ITransient transient)
            => new Command<TEntity, TKey1, TKey2>(transient);

        /// <inheritdoc/>
        public override IEnumerable<TEntity> Add(IEnumerable<TEntity> toAdd)
        {
            var items = toAdd?.ToList() ?? new List<TEntity>();
            if (!items.Any())
                return new List<TEntity>();

            var first = items[0];
            return AddAsync(items, new[] { first.KeyMetadata.ComponentOne.Name, first.KeyMetadata.ComponentTwo.Name }).WaitFor();
        }

        /// <inheritdoc/>
        public void Delete(TKey1 key1, TKey2 key2) 
            => Delete(new List<CompositeKeyValue<TKey1, TKey2>> { new CompositeKeyValue<TKey1, TKey2>(key1, key2) });

        /// <inheritdoc/>
        public void Delete(params ICompositeKeyValue<TKey1, TKey2>[] keys) 
            => Delete(keys?.AsEnumerable());

        /// <inheritdoc/>
        public void Delete(IEnumerable<ICompositeKeyValue<TKey1, TKey2>> keys)
            => DeleteAsync(keys).WaitFor();

        /// <inheritdoc/>
        public async Task DeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = default)
            => await DeleteAsync(new[] { CompositeKeyValue.Create(key1, key2) }, cancellationToken);

        /// <inheritdoc/>
        public async Task DeleteAsync(ICompositeKeyValue<TKey1, TKey2>[] keys, CancellationToken cancellationToken = default)
            => await DeleteAsync(keys?.AsEnumerable(), cancellationToken);

        /// <inheritdoc/>
        public async Task DeleteAsync(
            IEnumerable<ICompositeKeyValue<TKey1, TKey2>> keys,
            CancellationToken cancellationToken = default)
        {
            await DeleteByKeysAsync(keys, (keyBatch, q, b) =>
            {
                b.ClauseHandling.DefaultToOr();
                GetKeyColumnNames<TEntity, TKey1, TKey2>(q, out var keyColumnOne, out var keyColumnTwo);
                foreach (var k in keyBatch)
                {
                    b.And(a =>
                    {
                        a.Column(keyColumnOne).Is().EqualTo(k.ComponentOne);
                        a.Column(keyColumnTwo).Is().EqualTo(k.ComponentTwo);
                    });
                }
            }, cancellationToken);
        }

        /// <inheritdoc/>
        public void Update(dynamic toUpdate, TKey1 key1, TKey2 key2)
            => UpdateAsync(toUpdate, key1, key2).WaitFor();

        /// <inheritdoc/>
        public void Update(TEntity toUpdate)
            => UpdateAsync(toUpdate).WaitFor();

        /// <inheritdoc/>
        public async Task UpdateAsync(TEntity toUpdate, CancellationToken cancellationToken = default)
        {
            var key = toUpdate.GetKeyValue();
            await UpdateAsync(toUpdate, key.ComponentOne, key.ComponentTwo, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(dynamic toUpdate, TKey1 key1, TKey2 key2, CancellationToken cancellationToken = default)
        {
            var temp = new TEntity();
            var parameterExpression = Expression.Parameter(typeof(TEntity), "t");
            var keyOneConstantExpression = Expression.Constant(key1);
            var keyTwoConstantExpression = Expression.Constant(key2);
            var keyOnePropertyExpression = Expression.Property(parameterExpression, temp.KeyMetadata.ComponentOne.Name);
            var keyOneEqualsExpression = Expression.Equal(keyOnePropertyExpression, keyOneConstantExpression);
            var keyTwoPropertyExpression = Expression.Property(parameterExpression, temp.KeyMetadata.ComponentTwo.Name);
            var keyTwoEqualsExpression = Expression.Equal(keyTwoPropertyExpression, keyTwoConstantExpression);
            var andExpression = Expression.AndAlso(keyOneEqualsExpression, keyTwoEqualsExpression);
            var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(andExpression, parameterExpression);

            await UpdateAsync(toUpdate, lambdaExpression, cancellationToken);
        }
    }
}

