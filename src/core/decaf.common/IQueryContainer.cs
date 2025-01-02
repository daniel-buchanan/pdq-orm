using System;
using System.Diagnostics;
using decaf.common.DebugProxies;

namespace decaf.common
{
	/// <summary>
    /// <see cref="IQuery"/> represents an individual query that should be executed.
    /// It may be one of the following kinds:<br/>
    /// - Select<br/>
    /// - Delete<br/>
    /// - Update<br/>
    /// - Insert
    /// </summary>
	public interface IQueryContainer : IDisposable
	{
        /// <summary>
        /// The Id of the query, this is uniquely generated for each query and
        /// cannot be changed.
        /// </summary>
		Guid Id { get; }

        /// <summary>
        /// The status of the query, this will change as it is built and/or executed.
        /// </summary>
		QueryStatus Status { get; }

        /// <summary>
        /// Gets the <see cref="IQueryContext"/> for this query.
        /// </summary>
		IQueryContext Context { get; }

        /// <summary>
        /// Discard the current query.
        /// </summary>
        void Discard();
	}
}

