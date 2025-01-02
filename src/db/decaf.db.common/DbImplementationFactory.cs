using System;
using decaf.common;
using decaf.common.Connections;
using decaf.common.Templates;
using decaf.db.common.Builders;
using decaf.state;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace decaf.db.common
{
    public abstract class DbImplementationFactory : IDbImplementationFactory
	{
		/// <inheritdoc/>
		public void Configure(IServiceCollection services, IDatabaseOptionsExtensions options)
		{
			ConfigureService<ISqlFactory>(services, SqlFactory, ServiceLifetime.Singleton);
			ConfigureService<IConnectionFactory>(services, ConnectionFactory);
			ConfigureService<ITransactionFactory>(services, TransactionFactory);
			ConfigureService<IParameterManager>(services, ParameterManager);

			services.AddSingleton<IConnectionDetails>(options.GetConnectionDetails);
			services.AddSingleton(options.GetType(), options);
			services.AddSingleton<IDatabaseOptions>(options);
			services.AddSingleton<IDatabaseOptionsExtensions>(options);

            ConfigureService<IValueParser>(services, ValueParser, ServiceLifetime.Singleton);
            ConfigureService<ITypeParser>(services, TypeParser, ServiceLifetime.Singleton);
            ConfigureService<IConstants>(services, Constants, ServiceLifetime.Singleton);
            ConfigureService<IQuotedIdentifierBuilder>(services, QuotedIdentifierBuilder, ServiceLifetime.Singleton);
            ConfigureService<IWhereClauseBuilder>(services, WhereBuilder, ServiceLifetime.Transient);
            ConfigureService<IBuilderPipeline<ISelectQueryContext>>(services, SelectPipeline, ServiceLifetime.Transient);
            ConfigureService<IBuilderPipeline<IDeleteQueryContext>>(services, DeletePipeline, ServiceLifetime.Transient);
            ConfigureService<IBuilderPipeline<IInsertQueryContext>>(services, InsertPipeline, ServiceLifetime.Transient);
            ConfigureService<IBuilderPipeline<IUpdateQueryContext>>(services, UpdatePipeline, ServiceLifetime.Transient);
	        ConfigureService<IBuilderPipeline<ICreateTableQueryContext>>(services, CreateTablePipeline, ServiceLifetime.Transient);
	        ConfigureService<IBuilderPipeline<IDropTableQueryContext>>(services, DropTablePipeline, ServiceLifetime.Transient);
        }

		/// <summary>
		/// Configure a given service of type <see cref="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of the service to configure.</typeparam>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
		/// <param name="fetchType">The <see cref="Func{Type}"/> to provide the implementation type.</param>
		/// <param name="lifetime">The lifetime of the service to register (defaults to <see cref="ServiceLifetime.Scoped"/>).</param>
		/// <param name="instance">The instace for a <see cref="ServiceLifetime.Singleton"/> to be registered.</param>
		private static void ConfigureService<T>(
			IServiceCollection services,
			Func<Type> fetchType,
			ServiceLifetime lifetime = ServiceLifetime.Scoped,
			object instance = null)
		{
            var implementationType = fetchType();
            if(implementationType == null)
	            return;

            ServiceDescriptor sd;
            if (lifetime == ServiceLifetime.Singleton && instance != null)
                sd = new ServiceDescriptor(typeof(T), instance);
            else
                sd = new ServiceDescriptor(typeof(T), implementationType, lifetime);

            services.Replace(sd);
        }

		/// <summary>
		/// Get the type of the <see cref="IConnectionDetails"/> to configure.
		/// </summary>
		/// <returns></returns>
        protected abstract Type ConnectionDetails();

		/// <summary>
		/// Get the type of the <see cref="ISqlFactory"/> to configure.
		/// </summary>
		/// <returns></returns>
		protected virtual Type SqlFactory() => typeof(SqlFactory);

		/// <summary>
		/// Get the tyoe of the <see cref="IConnectionFactory"/> to configure.
		/// </summary>
		/// <returns></returns>
		protected abstract Type ConnectionFactory();

		/// <summary>
		/// Get the type of the <see cref="ITransactionFactory"/> to configure.
		/// </summary>
		/// <returns></returns>
		protected abstract Type TransactionFactory();

		/// <summary>
		/// Get the type of the <see cref="IValueParser"/> to configure.
		/// </summary>
		/// <returns></returns>
        protected abstract Type ValueParser();

		/// <summary>
		/// Get the type of the <see cref="ITypeParser" /> to configure.
		/// </summary>
		/// <returns></returns>
		protected abstract Type TypeParser();

		/// <summary>
		/// Get the type of the <see cref="IConstants"/> to configure.
		/// </summary>
		/// <returns></returns>
        protected virtual Type Constants() => typeof(ANSISQL.Constants);

		/// <summary>
		/// Get the type of the <see cref="IQuotedIdentifierBuilder"/> to configure.
		/// </summary>
		/// <returns></returns>
        protected virtual Type QuotedIdentifierBuilder() => typeof(ANSISQL.QuotedIdentifierBuilder);

		/// <summary>
		/// Get the type of the <see cref="IWhereBuilder"/> to configure.
		/// </summary>
		/// <returns></returns>
        protected abstract Type WhereBuilder();

		/// <summary>
		/// Get the type of the <see cref="IBuilderPipeline{ISelectQueryContext}"/> to configure.
		/// </summary>
		/// <returns></returns>
        protected abstract Type SelectPipeline();

        /// <summary>
        /// Get the type of the <see cref="IBuilderPipeline{IDeleteQueryContext}"/> to configure.
        /// </summary>
        /// <returns></returns>
        protected abstract Type DeletePipeline();

        /// <summary>
        /// Get the type of the <see cref="IBuilderPipeline{IInsertQueryContext}"/> to configure.
        /// </summary>
        /// <returns></returns>
        protected abstract Type InsertPipeline();

        /// <summary>
        /// Get the type of the <see cref="IBuilderPipeline{IUpdateQueryContext}"/> to configure.
        /// </summary>
        /// <returns></returns>
        protected abstract Type UpdatePipeline();

        /// <summary>
        /// Get the type of the <see cref="IParameterManager"/> to configure.
        /// </summary>
        /// <returns></returns>
        protected virtual Type ParameterManager() => typeof(ParameterManager);

        protected abstract Type CreateTablePipeline();
        
        protected abstract Type AlterTablePipeline();
        
        protected abstract Type DropTablePipeline();
	}
}

