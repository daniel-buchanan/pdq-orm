﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace pdq.common.Connections
{
	public interface IConnectionFactory : IDisposable
	{
		IConnection GetConnection(IConnectionDetails connectionDetails);

		Task<IConnection> GetConnectionAsync(IConnectionDetails connectionDetails, CancellationToken cancellationToken = default);
	}
}

