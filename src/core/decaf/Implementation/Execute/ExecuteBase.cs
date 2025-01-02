using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using decaf.common;
using decaf.common.Connections;
using decaf.common.Utilities;

namespace decaf.Implementation.Execute
{
    public abstract class ExecuteBase<TContext> :
        IGetSql
        where TContext: IQueryContext
	{
        protected readonly IQueryContainerInternal Query;
        protected TContext Context;
        protected readonly ISqlFactory SqlFactory;

		protected ExecuteBase(
            IQueryContainer query,
            TContext context,
            ISqlFactory sqlFactory)
		{
            Query = query as IQueryContainerInternal;
            Context = context;
            SqlFactory = sqlFactory;
		}

        protected IDbTransaction GetTransaction()
        {
            var internalTransient = Query.UnitOfWork as IUnitOfWorkExtended;
            return internalTransient?.Transaction.GetUnderlyingTransaction();
        }

        protected IDbConnection GetConnection()
        {
            var internalTransient = Query.UnitOfWork as IUnitOfWorkExtended;
            return internalTransient?.Connection.GetUnderlyingConnection();
        }

        /// <summary>
        /// Execute a function on a DBTransaction
        /// </summary>
        /// <typeparam name="T">The return type for the function.</typeparam>
        /// <param name="func">The function to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the function</returns>
        protected async Task<T> ExecuteAsync<T>(
            Func<string, object, IDbConnection, IDbTransaction, CancellationToken, Task<T>> func,
            CancellationToken cancellationToken = default)
        {
            var template = SqlFactory.ParseTemplate(Context);
            var parameters = SqlFactory.ParseParameters(Context, template, includePrefix: false);
            var connection = GetConnection();
            var transaction = GetTransaction();

            return await func(template.Sql, parameters, connection, transaction, cancellationToken);
        }

        /// <inheritdoc/>
        public string GetSql()
        {
            var template = SqlFactory.ParseTemplate(Context);
            return template.Sql;
        }

        /// <inheritdoc/>
        public Dictionary<string, object> GetSqlParameters()
        {
            var template = SqlFactory.ParseTemplate(Context);
            var parameters = SqlFactory.ParseParameters(Context, template, includePrefix: false) as ExpandoObject;
            return ParameterMapper.Unmap(parameters);
        }
    }
}

