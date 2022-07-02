﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pdq.common.Logging;

namespace pdq.common.Connections
{
	public abstract class ConnectionFactory : IConnectionFactory
	{
        protected readonly ILoggerProxy logger;
        private IDictionary<string, IConnection> connections;

        /// <summary>
        /// Create an instance of a ConnectionFactory.
        /// </summary>
        /// <param name="logger">The logger to Use.</param>
        public ConnectionFactory(ILoggerProxy logger)
        {
            this.connections = new Dictionary<string, IConnection>();
            this.logger = logger;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.connections = null;
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            Dispose();
            return new ValueTask();
        }

        /// <inheritdoc/>
        public IConnection Get(IConnectionDetails connectionDetails)
        {
            var t = GetAsync(connectionDetails);
            t.Wait();
            return t.Result;
        }

        /// <inheritdoc/>
        public async Task<IConnection> GetAsync(IConnectionDetails connectionDetails)
        {
            if (connectionDetails == null)
                throw new ArgumentNullException(nameof(connectionDetails), $"The {nameof(connectionDetails)} cannot be null, it MUST be provided when creating a connection.");

            if (this.connections == null)
                this.connections = new Dictionary<string, IConnection>();
            string hash = null;

            try
            {
                hash = connectionDetails.GetHash();
            }
            catch (Exception e)
            {
                this.logger.Error(e, "Error Getting connection Hash");
                throw;
            }

            if(this.connections.ContainsKey(hash))
            {
                return this.connections[hash];
            }

            try
            {
                var connection = await ConstructConnection(connectionDetails);
                this.connections.Add(hash, connection);

                return connection;
            }
            catch (Exception e)
            {
                this.logger.Error(e, $"An error occurred attempting to get the connection.");
                throw;
            }
        }

        /// <inheritdoc/>
        protected abstract Task<IConnection> ConstructConnection(IConnectionDetails connectionDetails);
    }
}

