using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using decaf.common;
using decaf.common.Utilities;

namespace decaf.Implementation.Execute
{
    public abstract class Execute<TContext> :
        ExecuteBase<TContext>,
        IExecute
        where TContext: IQueryContext
	{
		protected Execute(
            IQueryContainerInternal query,
            TContext context)
            : base(query, context, query.SqlFactory) { }

        /// <inheritdoc/>
        public IExecuteDynamic Dynamic() => this;

        /// <inheritdoc/>
        public IEnumerable<T> AsEnumerable<T>()
            => AsEnumerableAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> AsEnumerableAsync<T>(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.QueryAsync<T>(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public IEnumerable<dynamic> AsEnumerable()
            => AsEnumerableAsync().WaitFor();

        /// <inheritdoc/>
        public async Task<IEnumerable<dynamic>> AsEnumerableAsync(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.QueryAsync(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public T First<T>()
            => FirstAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<T> FirstAsync<T>(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.QueryFirstAsync<T>(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public dynamic First()
            => FirstAsync().WaitFor();

        /// <inheritdoc/>
        public async Task<dynamic> FirstAsync(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.QueryFirstAsync(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public T FirstOrDefault<T>()
            => FirstOrDefaultAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.QueryFirstOrDefaultAsync<T>(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public dynamic FirstOrDefault()
            => FirstOrDefaultAsync().WaitFor();

        /// <inheritdoc/>
        public async Task<dynamic> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.QueryFirstOrDefaultAsync(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public T Single<T>()
            => SingleAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<T> SingleAsync<T>(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.QuerySingleAsync<T>(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public dynamic Single()
            => SingleAsync().WaitFor();

        /// <inheritdoc/>
        public async Task<dynamic> SingleAsync(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.QuerySingleAsync(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public T SingleOrDefault<T>()
            => SingleOrDefaultAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.QuerySingleOrDefaultAsync<T>(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public dynamic SingleOrDefault()
            => SingleOrDefaultAsync().WaitFor();

        /// <inheritdoc/>
        public async Task<dynamic> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.QuerySingleOrDefaultAsync(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public IList<T> ToList<T>()
            => ToListAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<IList<T>> ToListAsync<T>(CancellationToken cancellationToken = default)
            => (await AsEnumerableAsync<T>(cancellationToken)).ToList();

        /// <inheritdoc/>
        public IList<dynamic> ToList()
            => ToListAsync().WaitFor();

        /// <inheritdoc/>
        public async Task<IList<dynamic>> ToListAsync(CancellationToken cancellationToken = default)
            => (await AsEnumerableAsync(cancellationToken)).ToList();

        /// <inheritdoc/>
        public IExecute<T> Typed<T>()
            => Execute<T, TContext>.Create(Query, Context);

        /// <inheritdoc/>
        void IExecute.Execute()
            => ExecuteAsync().WaitFor();

        /// <inheritdoc/>
        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
            => await ExecuteAsync((s, p, c, t, _) => c.ExecuteAsync(s, p, t), cancellationToken);

        /// <inheritdoc/>
        public new string GetSql()
            => base.GetSql();

    }
}

