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
    internal class Command<TEntity, TKey1, TKey2> :
        Command<TEntity>,
        ICommand<TEntity, TKey1, TKey2>
        where TEntity : class, IEntity<TKey1, TKey2>, new()
    {
        public Command(IDecaf decaf, ISqlFactory sqlFactory) : base(decaf, sqlFactory) { }

        private Command(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public new static ICommand<TEntity, TKey1, TKey2> Create(IUnitOfWork unitOfWork)
            => new Command<TEntity, TKey1, TKey2>(unitOfWork);

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
                GetKeyColumnNames(q, out var keyColumnOne, out var keyColumnTwo);
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
        
        /// <summary>
        /// Get the column names for an Entity.<br/>
        /// Where it has a composite key with two values.
        /// </summary>
        /// <typeparam name="TEntity">The Entity type to work with.</typeparam>
        /// <typeparam name="TKey1">The type of the first component of the key.</typeparam>
        /// <typeparam name="TKey2">The type of the second component of the key.</typeparam>
        /// <param name="q">The <see cref="IQuery"/> instance to work with.</param>
        /// <param name="key1Name">The name of the first Key comoponent.</param>
        /// <param name="key2Name">The name of the second Key compononent.</param>
        private static void GetKeyColumnNames(IQueryContainer q, out string key1Name, out string key2Name)
        {
            var tmp = new TEntity();
            key1Name = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata.ComponentOne);
            key2Name = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata.ComponentTwo);
        }
    }
}

