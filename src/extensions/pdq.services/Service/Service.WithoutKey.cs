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
    internal class Service<TEntity> :
        ExecutionNotifiable,
        IService<TEntity>
        where TEntity : class, IEntity, new()
    {
        private IQuery<TEntity> Query => GetQuery<IQuery<TEntity>>();
        private ICommand<TEntity> Command => GetCommand<ICommand<TEntity>>();

        public Service(
            IQuery<TEntity> query,
            ICommand<TEntity> command)
            : base(query, command) { }

        private Service(IUnitOfWork unitOfWork)
            : base(unitOfWork,
                   Query<TEntity>.Create,
                   Command<TEntity>.Create)
        { }

        public static IService<TEntity> Create(IUnitOfWork unitOfWork)
            => new Service<TEntity>(unitOfWork);

        /// <inheritdoc/>
        public TEntity Add(TEntity toAdd)
            => AddAsync(toAdd).WaitFor();

        /// <inheritdoc/>
        public IEnumerable<TEntity> Add(params TEntity[] toAdd)
            => AddAsync(toAdd?.AsEnumerable()).WaitFor();

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
        public void Delete(Expression<Func<TEntity, bool>> expression)
            => DeleteAsync(expression).WaitFor();

        /// <inheritdoc/>
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
            => FindAsync(expression).WaitFor();

        /// <inheritdoc/>
        public Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            => this.Query.FindAsync(expression, cancellationToken);

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

