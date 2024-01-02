﻿using System;
using pdq.common.Connections;
using pdq.common.Logging;
using pdq.common.Utilities;

namespace pdq.common
{
	public class QueryContainer : IQueryContainerInternal
	{
		private readonly ILoggerProxy logger;
		private readonly IUnitOfWorkInternal unitOfWork;
		private readonly IAliasManager aliasManager;
		private readonly PdqOptions options;
		private readonly IHashProvider hashProvider;
		private IQueryContext context;

		private QueryContainer(
			IUnitOfWork unitOfWork,
			ILoggerProxy logger,
			IHashProvider hashProvider,
			PdqOptions options)
		{
			this.logger = logger;
			this.unitOfWork = unitOfWork as IUnitOfWorkInternal;
			this.aliasManager = AliasManager.Create();
			this.options = options;
			this.hashProvider = hashProvider;

			Id = Guid.NewGuid();
			Status = QueryStatus.Empty;

			this.logger.Debug($"Query({Id}) :: Created as {Status}");
		}

		/// <inheritdoc/>
		public Guid Id { get; private set; }

		/// <inheritdoc/>
		public QueryStatus Status { get; private set; }

		/// <inheritdoc/>
		IAliasManager IQueryContainerInternal.AliasManager => this.aliasManager;

		/// <inheritdoc/>
		IUnitOfWork IQueryContainerInternal.UnitOfWork => this.unitOfWork;

		/// <inheritdoc/>
		public IQueryContext Context => this.context;

		/// <inheritdoc/>
		public void Discard() => this.Dispose();

		/// <inheritdoc/>
		IHashProvider IQueryContainerInternal.HashProvider => this.hashProvider;

		/// <inheritdoc/>
		PdqOptions IQueryContainerInternal.Options => this.options;

		/// <inheritdoc/>
		public ISqlFactory SqlFactory => this.unitOfWork.SqlFactory;

		/// <inheritdoc/>
		public ILoggerProxy Logger => this.logger;

		/// <inheritdoc/>
		string IQueryContainerInternal.GetHash() => this.context.GetHash();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <param name="logger"></param>
		/// <param name="unitOfWork"></param>
		/// <returns></returns>
		public static IQueryContainer Create(
			IUnitOfWork unitOfWork,
			ILoggerProxy logger,
			IHashProvider hashProvider,
			PdqOptions options)
			=> new QueryContainer(unitOfWork, logger, hashProvider, options);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="existing"></param>
		/// <returns></returns>
		internal static IQueryContainer Create(IQueryContainerInternal existing)
			=> new QueryContainer(existing.UnitOfWork, existing.Logger, existing.HashProvider, existing.Options);

		/// <inheritdoc/>
		void IQueryContainerInternal.SetContext(IQueryContext context) => this.context = context;

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc/>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;
			this.logger.Debug($"Query({Id} :: Disposing({disposing})");
			this.aliasManager.Dispose();
			this.unitOfWork.NotifyQueryDisposed(Id);
		}
	}
}

