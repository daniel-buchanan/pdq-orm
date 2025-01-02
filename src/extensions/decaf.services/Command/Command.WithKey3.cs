using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using decaf.common;
using decaf.common.Connections;
using decaf.common.Utilities;

namespace decaf.services
{
    internal class Command<TEntity, TKey1, TKey2, TKey3> :
        Command<TEntity>,
        ICommand<TEntity, TKey1, TKey2, TKey3>
        where TEntity : class, IEntity<TKey1, TKey2, TKey3>, new()
    {
        public Command(IDecaf decaf, ISqlFactory sqlFactory) : base(decaf, sqlFactory) { }

        private Command(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public new static ICommand<TEntity, TKey1, TKey2, TKey3> Create(IUnitOfWork unitOfWork)
            => new Command<TEntity, TKey1, TKey2, TKey3>(unitOfWork);

        /// <inheritdoc/>
        public override IEnumerable<TEntity> Add(IEnumerable<TEntity> toAdd)
        {
            var items = toAdd?.ToList() ?? new List<TEntity>();
            if (!items.Any())
                return new List<TEntity>();

            var first = items[0];
            return AddAsync(items, new[]
            {
                first.KeyMetadata.ComponentOne.Name,
                first.KeyMetadata.ComponentTwo.Name,
                first.KeyMetadata.ComponentThree.Name
            }).WaitFor();
        }

        /// <inheritdoc/>
        public void Delete(TKey1 key1, TKey2 key2, TKey3 key3)
            => Delete(new List<CompositeKeyValue<TKey1, TKey2, TKey3>> { new CompositeKeyValue<TKey1, TKey2, TKey3>(key1, key2, key3) });

        /// <inheritdoc/>
        public void Delete(params ICompositeKeyValue<TKey1, TKey2, TKey3>[] keys)
            => Delete(keys?.AsEnumerable());

        /// <inheritdoc/>
        public void Delete(IEnumerable<ICompositeKeyValue<TKey1, TKey2, TKey3>> keys)
            => DeleteAsync(keys).WaitFor();

        /// <inheritdoc/>
        public async Task DeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = default)
            => await DeleteAsync(new[] { CompositeKeyValue.Create(key1, key2, key3) }, cancellationToken);

        /// <inheritdoc/>
        public async Task DeleteAsync(ICompositeKeyValue<TKey1, TKey2, TKey3>[] keys, CancellationToken cancellationToken = default)
            => await DeleteAsync(keys?.AsEnumerable(), cancellationToken);

        /// <inheritdoc/>
        public async Task DeleteAsync(IEnumerable<ICompositeKeyValue<TKey1, TKey2, TKey3>> keys, CancellationToken cancellationToken = default)
        {
            await DeleteByKeysAsync(keys, (keyBatch, q, b) =>
            {
                GetKeyColumnNames(q, out var key1Name, out var key2Name, out var key3Name);
                b.ClauseHandling.DefaultToOr();
                foreach (var k in keyBatch)
                {
                    b.And(a =>
                    {
                        a.Column(key1Name).Is().EqualTo(k.ComponentOne);
                        a.Column(key2Name).Is().EqualTo(k.ComponentTwo);
                        a.Column(key3Name).Is().EqualTo(k.ComponentThree);
                    });
                }
            }, cancellationToken);
        }

        /// <inheritdoc/>
        public void Update(dynamic toUpdate, TKey1 key1, TKey2 key2, TKey3 key3)
            => UpdateAsync(toUpdate, key1, key2, key3).WaitFor();

        /// <inheritdoc/>
        public void Update(TEntity toUpdate)
            => UpdateAsync(toUpdate).WaitFor();

        /// <inheritdoc/>
        public async Task UpdateAsync(TEntity toUpdate, CancellationToken cancellationToken = default)
        {
            var key = toUpdate.GetKeyValue();
            await UpdateAsync(toUpdate, key.ComponentOne, key.ComponentTwo, key.ComponentThree, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(
            dynamic toUpdate,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            CancellationToken cancellationToken = default)
        {
            var temp = new TEntity();
            var parameterExpression = Expression.Parameter(typeof(TEntity), "t");
            var keyOneConstantExpression = Expression.Constant(key1);
            var keyTwoConstantExpression = Expression.Constant(key2);
            var keyThreeConstantExpression = Expression.Constant(key3);
            var keyOnePropertyExpression = Expression.Property(parameterExpression, temp.KeyMetadata.ComponentOne.Name);
            var keyOneEqualsExpression = Expression.Equal(keyOnePropertyExpression, keyOneConstantExpression);
            var keyTwoPropertyExpression = Expression.Property(parameterExpression, temp.KeyMetadata.ComponentTwo.Name);
            var keyTwoEqualsExpression = Expression.Equal(keyTwoPropertyExpression, keyTwoConstantExpression);
            var keyThreePropertyExpression = Expression.Property(parameterExpression, temp.KeyMetadata.ComponentThree.Name);
            var keyThreeEqualsExpression = Expression.Equal(keyThreePropertyExpression, keyThreeConstantExpression);
            var andExpression = Expression.AndAlso(keyOneEqualsExpression, keyTwoEqualsExpression);
            var nestedAndExpression = Expression.AndAlso(andExpression, keyThreeEqualsExpression);
            var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(nestedAndExpression, parameterExpression);

            await UpdateAsync(toUpdate, lambdaExpression, cancellationToken);
        }
        
        /// <summary>
        /// Get the column names for an Entity.<br/>
        /// Where it has a composite key with two values.
        /// </summary>
        /// <param name="q">The <see cref="IQuery"/> instance to work with.</param>
        /// <param name="key1Name">The name of the first Key component.</param>
        /// <param name="key2Name">The name of the second Key component.</param>
        /// <param name="key3Name">The name of the third Key component.</param>
        private static void GetKeyColumnNames(IQueryContainer q, out string key1Name, out string key2Name, out string key3Name)
        {
            var tmp = new TEntity();
            key1Name = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata.ComponentOne);
            key2Name = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata.ComponentTwo);
            key3Name = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata.ComponentThree);
        }
    }
}

