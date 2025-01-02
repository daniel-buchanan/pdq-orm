using decaf.common;
using decaf.state;

namespace decaf
{
	public static class QueryExtensions
	{
		/// <summary>
        /// Begin a delete query, this will allow you to remove records from a table,
        /// view or other schema objects within your database.
        /// </summary>
        /// <param name="query">The query instance that will become a "delete" query.</param>
        /// <returns>(FluentApi) The ability to specify the table, view or other schema item to delete from.</returns>
        /// <example>q.Delete().From("users", "u").Where(b => b.Column("name").Is().StartsWith("bob");</example>
		public static IDelete Delete(this IQueryContainer query)
            => Implementation.Execute.Delete.Create(query.CreateContext<IDeleteQueryContext>(), query);

        /// <summary>
        /// Begin a select query, this will allow you to retrieve records from one or more
        /// tables, perform joins and other complex query semantics to produce the required output.
        /// </summary>
        /// <param name="query">The query instance that will become a "select" query.</param>
        /// <returns>(FluentApi) The ability to specify the base table, view or other schema item to select from.</returns>
        /// <example>q.Select().From("users", "u").Where(b => b.Column("name").Is().EndssWith("bob");</example>
        public static ISelect Select(this IQueryContainer query)
            => Implementation.Execute.Select.Create(query.CreateContext<ISelectQueryContext>(), query);

        /// <summary>
        /// Begin an insert query, this will allow you to insert records into a single table.<br/>
        /// You may have multiple sources for your insert query, whether that be static values,
        /// or from another query itself.
        /// </summary>
        /// <param name="query">The query instance that will become an "insert" query.</param>
        /// <returns>(FluentApi) The ability to specify the base table, view to insert into.</returns>
        /// <example>q.Insert().Into("users", "u"),Value(new { first_name = "Bob", last_name = "Smith" });</example>
        public static IInsert Insert(this IQueryContainer query)
            => Implementation.Execute.Insert.Create(query.CreateContext<IInsertQueryContext>(), query);

        /// <summary>
        /// Begin an update query, this will allow you to update records into a single table.<br/>
        /// You may have multiple sources for your update query, whether that be static values,
        /// or from another query itself.
        /// </summary>
        /// <param name="query">The query instance that will become an "update" query.</param>
        /// <returns>(FluentApi) The ability to specify the base table, view to update values in.</returns>
        /// <example>q.Update().Table("users", "u"),Set(new { first_name = "Bob", last_name = "Smith" }).Where(b => b.Column("id").Is().EqualTo(42);</example>
        public static IUpdate Update(this IQueryContainer query)
            => Implementation.Execute.Update.Create(query.CreateContext<IUpdateQueryContext>(), query);
    }
}

