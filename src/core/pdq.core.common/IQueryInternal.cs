﻿using System;
using pdq.common.Logging;
using pdq.common.Utilities;

[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("pdq")]
namespace pdq.common
{
	internal interface IQueryInternal : IQuery
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
        /// Gets the <see cref="ITransient"/> for this query.
        /// </summary>
		ITransient Transient { get; }

		/// <summary>
        /// Gets the <see cref="IQueryContext"/> for this query.
        /// </summary>
		IQueryContext Context { get; }

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
        /// Gets the <see cref="PdqOptions"/> to use for this query.
        /// </summary>
		PdqOptions Options { get; }
    }
}