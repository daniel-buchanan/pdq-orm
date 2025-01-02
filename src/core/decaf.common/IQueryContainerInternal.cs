using decaf.common.Connections;
using decaf.common.Logging;
using decaf.common.Utilities;

[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("decaf")]
[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("decaf.ddl")]
namespace decaf.common
{
	public interface IQueryContainerInternal : IQueryContainer
	{
		/// <summary>
        /// Get the hash that represents this query.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the hash.</returns>
		string GetHash();

		/// <summary>
        /// Gets the <see cref="IAliasManager"/> for this query.
        /// </summary>
		IAliasManager AliasManager { get; }

		/// <summary>
        /// Gets the <see cref="IUnitOfWork"/> for this query.
        /// </summary>
		IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Gets the <see cref="ISqlFactory"/> for this query.
        /// </summary>
        ISqlFactory SqlFactory { get; }

        /// <summary>
        /// Gets the <see cref="IHashProvider"/> for this query.
        /// </summary>
        IHashProvider HashProvider { get; }

        /// <summary>
        /// Gets the <see cref="ILoggerProxy"/> for this query.
        /// </summary>
        ILoggerProxy Logger { get; }

		/// <summary>
        /// Set the <see cref="IQueryContext"/> associated with this query.
        /// </summary>
        /// <param name="context">The <see cref="IQueryContext"/> to use.</param>
		void SetContext(IQueryContext context);

        /// <summary>
        /// Gets the <see cref="DecafOptions"/> to use for this query.
        /// </summary>
		DecafOptions Options { get; }
    }
}