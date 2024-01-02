﻿using System;
using Microsoft.Extensions.DependencyInjection;
using pdq.common;
using pdq.common.Connections;
using pdq.common.Logging;
using pdq.Exceptions;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("pdq.playground")]
namespace pdq
{
    /// <summary>
    /// Set of options for configuring pdq.
    /// </summary>
    public class PdqOptions
    {
        public PdqOptions()
        {
            DefaultLogLevel = LogLevel.Error;
            DefaultClauseHandling = ClauseHandling.And;
            TrackUnitsOfWork = false;
            CloseConnectionOnCommitOrRollback = false;
            LoggerProxyType = typeof(DefaultLogger);
            IncludeHeaderCommentsInSql = true;
        }

        /// <summary>
        /// The default log level to use, this is set to <see cref="LogLevel.Error"/>
        /// by default, unless changed.
        /// </summary>
        public LogLevel DefaultLogLevel { get; private set; }

        /// <summary>
        /// The default where clause handling behaviour, this is set to <see cref="ClauseHandling.And"/>.
        /// If you want to ensure that you always set this explictly, override with <see cref="ClauseHandling.Unspecified"/> and
        /// the <see cref="WhereBuildFailedException"/> will be thrown if you haven't set it.
        /// </summary>
        public ClauseHandling DefaultClauseHandling { get; private set; }

		/// <summary>
        /// Whether or not to track transients as they are used, and disposed.
        /// </summary>
		public bool TrackUnitsOfWork { get; private set; }

        /// <summary>
        /// Whether or not to close the connection on commit or rollback of the transaction.
        /// </summary>
        public bool CloseConnectionOnCommitOrRollback { get; private set; }

        /// <summary>
        /// Whether or not to include header comments in generated SQL.
        /// </summary>
        public bool IncludeHeaderCommentsInSql { get; private set; }
        
        /// <summary>
        /// Whether or not to automatically inject an <see cref="IUnitOfWork"/> as a service.
        /// </summary>
        public bool InjectUnitOfWork { get; private set; }
        
        /// <summary>
        /// The lifetime for the <see cref="IUnitOfWork"/> when injected.
        /// </summary>
        public ServiceLifetime UnitOfWorkLifetime { get; private set; }

        /// <summary>
        /// The type of the logger proxy to use.
        /// </summary>
        internal Type LoggerProxyType { get; private set; }
	}
}

