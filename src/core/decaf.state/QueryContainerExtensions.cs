using decaf.common;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("decaf")]
namespace decaf.state
{
	public static class QueryFrameworkExtensions
	{
		/// <summary>
        /// Create an <see cref="IQueryContext"/> from an <see cref="IQuery"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IQueryContext"/> to create.</typeparam>
        /// <param name="query">Self.</param>
        /// <returns>A newly created <see cref="IQueryContext"/>.</returns>
		public static T CreateContext<T>(this IQueryContainer query) where T: IQueryContext
        {
			var internalQuery = query as IQueryContainerInternal;
			var queryContextType = typeof(T);

			if (internalQuery == null) return default(T);

			if(queryContextType.IsAssignableFrom(typeof(ISelectQueryContext)))
				return (T)SelectQueryContext.Create(internalQuery.AliasManager, internalQuery.HashProvider);

			if (queryContextType.IsAssignableFrom(typeof(IDeleteQueryContext)))
				return (T)DeleteQueryContext.Create(internalQuery.AliasManager, internalQuery.HashProvider);

			if (queryContextType.IsAssignableFrom(typeof(IInsertQueryContext)))
				return (T)InsertQueryContext.Create(internalQuery.AliasManager, internalQuery.HashProvider);

			if (queryContextType.IsAssignableFrom(typeof(IUpdateQueryContext)))
				return (T)UpdateQueryContext.Create(internalQuery.AliasManager, internalQuery.HashProvider);

			if (queryContextType.IsAssignableFrom(typeof(ICreateTableQueryContext)))
				return (T)CreateTableQueryContext.Create(internalQuery.AliasManager, internalQuery.HashProvider);

			if (queryContextType.IsAssignableFrom(typeof(IDropTableQueryContext)))
				return (T)DropTableQueryContext.Create(internalQuery.AliasManager, internalQuery.HashProvider);

			return default(T);
        }
	}
}

