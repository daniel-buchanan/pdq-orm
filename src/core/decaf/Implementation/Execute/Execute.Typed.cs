using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using decaf.common;
using decaf.common.Utilities;

namespace decaf.Implementation.Execute
{
    internal class Execute<TResult, TContext> : Execute<TContext>, IExecute<TResult>
        where TContext: IQueryContext
	{
		protected Execute(
            IQueryContainerInternal query,
            TContext context)
            : base(query, context) { }

        public static IExecute<TResult> Create(
            IQueryContainerInternal query,
            TContext context)
            => new Execute<TResult, TContext>(query, context);

        /// <inheritdoc/>
        public new IEnumerable<TResult> AsEnumerable() => AsEnumerableAsync<TResult>().WaitFor();

        /// <inheritdoc/>
        public new Task<IEnumerable<TResult>> AsEnumerableAsync(CancellationToken cancellationToken = default)
            => AsEnumerableAsync<TResult>(cancellationToken);

        /// <inheritdoc/>
        public new TResult First() => FirstAsync().WaitFor();

        /// <inheritdoc/>
        public new Task<TResult> FirstAsync(CancellationToken cancellationToken = default)
            => FirstAsync<TResult>(cancellationToken);

        /// <inheritdoc/>
        public new TResult FirstOrDefault() => FirstOrDefaultAsync().WaitFor();

        /// <inheritdoc/>
        public new Task<TResult> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
            => FirstOrDefaultAsync<TResult>(cancellationToken);

        /// <inheritdoc/>
        public new TResult Single() => SingleAsync().WaitFor();

        /// <inheritdoc/>
        public new Task<TResult> SingleAsync(CancellationToken cancellationToken = default)
            => SingleAsync<TResult>(cancellationToken);

        /// <inheritdoc/>
        public new TResult SingleOrDefault() => SingleOrDefaultAsync().WaitFor();

        /// <inheritdoc/>
        public new Task<TResult> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
            => SingleOrDefaultAsync<TResult>(cancellationToken);

        /// <inheritdoc/>
        public new IList<TResult> ToList() => ToListAsync().WaitFor();

        /// <inheritdoc/>
        public new Task<IList<TResult>> ToListAsync(CancellationToken cancellationToken = default)
            => ToListAsync<TResult>(cancellationToken);

        /// <inheritdoc/>
        void IExecute<TResult>.Execute() => ExecuteAsync().WaitFor();
    }
}

