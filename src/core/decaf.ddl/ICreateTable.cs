using System;
using System.Linq.Expressions;
using decaf.ddl;

namespace decaf.ddl;

public interface ICreateTable : IExecute
{
    /// <summary>
    /// Create a table from a class.
    /// </summary>
    /// <param name="named">The name of the table.</param>
    /// <typeparam name="T">The type to create as a table.</typeparam>
    /// <returns>(FluentAPI) The ability to continue to create a table.</returns>
    ICreateTable FromType<T>(string named = null);
    
    /// <summary>
    /// Specify the name for the table to be created.
    /// </summary>
    /// <param name="name">The name to use for the table.</param>
    /// <returns>(FluentAPI) The ability to continue to create a table.</returns>
    ICreateTable Named(string name);

    /// <summary>
    /// Create a table with the columns as provided by the array of statements.
    /// </summary>
    /// <param name="columns">The set of columns to create.</param>
    /// <returns>(FluentAPI) The ability to continue to create a table.</returns>
    ICreateTable WithColumns(params Expression<Action<IDdlColumnBuilder>>[] columns);

    /// <summary>
    /// Create a table with the columns with the provided type.
    /// </summary>
    /// <typeparam name="T">The model to use for the table columns.</typeparam>
    /// <returns>(FluentAPI) The ability to continue to create a table.</returns>
    ICreateTable WithColumns<T>();

    /// <summary>
    /// Create an index using the provided name and columns.
    /// </summary>
    /// <param name="name">The name of the index to create.</param>
    /// <param name="columns">The columns to be included in the index.</param>
    /// <returns>(FluentAPI) The ability to continue to create a table.</returns>
    ICreateTable WithIndex(string name, params Expression<Action<IDdlColumnBuilder>>[] columns);
    
    /// <summary>
    /// Create an index using the provided columns.
    /// </summary>
    /// <param name="columns">The columns to be included in the index.</param>
    /// <returns>(FluentAPI) The ability to continue to create a table.</returns>
    ICreateTable WithIndex(params Expression<Action<IDdlColumnBuilder>>[] columns);

    /// <summary>
    /// Add a primary key to the table being created. 
    /// </summary>
    /// <param name="columns"></param>
    /// <returns>(FluentAPI) The ability to continue to create a table.</returns>
    ICreateTable WithPrimaryKey(params Expression<Action<IDdlColumnBuilder>>[] columns);

    /// <summary>
    /// Add a primary key to the table being created.
    /// </summary>
    /// <param name="name">The name of the primary key to be created.</param>
    /// <param name="columns">The columns to include in the primary key.</param>
    /// <returns>(FluentAPI) The ability to continue to create a table.</returns>
    ICreateTable WithPrimaryKey(string name, params Expression<Action<IDdlColumnBuilder>>[] columns);
}