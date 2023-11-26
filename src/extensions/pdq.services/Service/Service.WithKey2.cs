﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using pdq.common.Connections;
using pdq.common.Utilities;

namespace pdq.services
{
    internal class Service<TEntity, TKey1, TKey2> :
        ExecutionNotifiable,
        IService<TEntity, TKey1, TKey2>
        where TEntity : class, IEntity<TKey1, TKey2>, new()
    {
        private IQuery<TEntity, TKey1, TKey2> Query => GetQuery<IQuery<TEntity, TKey1, TKey2>>();
        private ICommand<TEntity, TKey1, TKey2> Command => GetCommand<ICommand<TEntity, TKey1, TKey2>>();

        public Service(
            IQuery<TEntity, TKey1, TKey2> query,
            ICommand<TEntity, TKey1, TKey2> command)
            : base(query, command) { }

        private Service(ITransient transient)
            : base(transient,
                   Query<TEntity, TKey1, TKey2>.Create,
                   Command<TEntity, TKey1, TKey2>.Create)
        { }

        public static IService<TEntity, TKey1, TKey2> Create(ITransient transient)
            => new Service<TEntity, TKey1, TKey2>(transient);

        /// <inheritdoc/>
        public TEntity Add(TEntity toAdd)
            => AddAsync(toAdd).WaitFor();

        /// <inheritdoc/>
        public IEnumerable<TEntity> Add(params TEntity[] toAdd)
            => AddAsync(toAdd).WaitFor();

        /// <inheritdoc/>
        public IEnumerable<TEntity> Add(IEnumerable<TEntity> toAdd)
            => AddAsync(toAdd).WaitFor();

        /// <inheritdoc/>
        public IEnumerable<TEntity> All()
            => AllAsync().WaitFor();

        /// <inheritdoc/>
        public Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken = default)
            => this.Query.AllAsync(cancellationToken);

        /// <inheritdoc/>
        public void Delete(TKey1 key1, TKey2 key2)
            => DeleteAsync(key1, key2).WaitFor();

        /// <inheritdoc/>
        public void Delete(params ICompositeKeyValue<TKey1, TKey2>[] keys)
            => DeleteAsync(keys?.AsEnumerable()).WaitFor();

        /// <inheritdoc/>
        public void Delete(IEnumerable<ICompositeKeyValue<TKey1, TKey2>> keys)
            => DeleteAsync(keys).WaitFor();

        /// <inheritdoc/>
        public void Delete(Expression<Func<TEntity, bool>> expression)
            => DeleteAsync(expression).WaitFor();

        /// <inheritdoc/>
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
            => FindAsync(expression).WaitFor();

        /// <inheritdoc/>
        public Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            => this.Query.FindAsync(expression, cancellationToken);

        /// <inheritdoc/>
        public TEntity Get(TKey1 key1, TKey2 key2)
            => GetAsync(key1, key2).WaitFor();

        /// <inheritdoc/>
        public IEnumerable<TEntity> Get(params ICompositeKeyValue<TKey1, TKey2>[] keys)
            => GetAsync(keys?.AsEnumerable()).WaitFor();

        /// <inheritdoc/>
        public IEnumerable<TEntity> Get(IEnumerable<ICompositeKeyValue<TKey1, TKey2>> keys)
            => GetAsync(keys).WaitFor();

        /// <inheritdoc/>
        public Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = default)
            => this.Query.GetAsync(key1, key2, cancellationToken);

        /// <inheritdoc/>
        public Task<IEnumerable<TEntity>> GetAsync(ICompositeKeyValue<TKey1, TKey2>[] keys, CancellationToken cancellationToken = default)
            => this.Query.GetAsync(keys, cancellationToken);

        /// <inheritdoc/>
        public Task<IEnumerable<TEntity>> GetAsync(IEnumerable<ICompositeKeyValue<TKey1, TKey2>> keys, CancellationToken cancellationToken = default)
            => this.Query.GetAsync(keys, cancellationToken);

        /// <inheritdoc/>
        public void Update(dynamic toUpdate, TKey1 key1, TKey2 key2)
        {
            var t = UpdateAsync(toUpdate, key1, key2);
            t.Wait();
        }

        /// <inheritdoc/>
        public void Update(TEntity toUpdate, Expression<Func<TEntity, bool>> expression)
            => UpdateAsync(toUpdate, expression).WaitFor();

        /// <inheritdoc/>
        public void Update(dynamic toUpdate, Expression<Func<TEntity, bool>> expression)
        {
            var t = UpdateAsync(toUpdate, expression);
            t.Wait();
        }

        /// <inheritdoc/>
        public void Update(TEntity toUpdate)
            => UpdateAsync(toUpdate).WaitFor();

        /// <inheritdoc/>
        public async Task UpdateAsync(TEntity toUpdate, CancellationToken cancellationToken = default)
            => await this.Command.UpdateAsync(toUpdate, cancellationToken);

        /// <inheritdoc/>
        public async Task UpdateAsync(dynamic toUpdate, TKey1 key1, TKey2 key2, CancellationToken cancellationToken = default)
            => await this.Command.UpdateAsync(toUpdate, key1, key2, cancellationToken);

        /// <inheritdoc/>
        public async Task DeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = default)
            => await this.Command.DeleteAsync(key1, key2, cancellationToken);

        /// <inheritdoc/>
        public async Task DeleteAsync(ICompositeKeyValue<TKey1, TKey2>[] keys, CancellationToken cancellationToken = default)
            => await this.Command.DeleteAsync(keys, cancellationToken);

        /// <inheritdoc/>
        public async Task DeleteAsync(IEnumerable<ICompositeKeyValue<TKey1, TKey2>> keys, CancellationToken cancellationToken = default)
            => await this.Command.DeleteAsync(keys, cancellationToken);

        /// <inheritdoc/>
        public async Task<TEntity> AddAsync(TEntity toAdd, CancellationToken cancellationToken = default)
            => await this.Command.AddAsync(toAdd, cancellationToken);

        /// <inheritdoc/>
        public async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
            => await this.Command.AddAsync(items, cancellationToken);

        /// <inheritdoc/>
        public async Task UpdateAsync(TEntity toUpdate, Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            => await this.Command.UpdateAsync(toUpdate, expression, cancellationToken);

        /// <inheritdoc/>
        public async Task UpdateAsync(dynamic toUpdate, Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            => await this.Command.UpdateAsync(toUpdate, expression, cancellationToken);

        /// <inheritdoc/>
        public async Task DeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            => await this.Command.DeleteAsync(expression, cancellationToken);
    }
}