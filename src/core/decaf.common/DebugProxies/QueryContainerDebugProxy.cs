using System;

namespace decaf.common.DebugProxies;

internal class QueryContainerDebugProxy(IQueryContainer queryContainer)
{
    /// <summary>
    /// The SQL command that was executed last.
    /// </summary>
    public string LastExecutedSql
    {
        get
        {
            const string notExecuted = "<< Nothing Executed >>";
            if (queryContainer.Context is null) return notExecuted;
            
            var queryContainerInternal = queryContainer as IQueryContainerInternal;
            var sql = queryContainerInternal?.SqlFactory?.ParseTemplate(queryContainer.Context);
            return sql?.Sql ?? notExecuted;
        }
    }

    /// <summary>
    /// The Id of the current query.
    /// </summary>
    public Guid Id => queryContainer.Id;

    /// <summary>
    /// The status of the current query.
    /// </summary>
    public QueryStatus Status => queryContainer.Status;
    
    /// <summary>
    /// The current query context.
    /// </summary>
    public IQueryContext Context => queryContainer.Context;
}