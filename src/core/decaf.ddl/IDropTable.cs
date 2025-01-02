using System;
using System.Linq.Expressions;
using decaf.ddl;

namespace decaf.ddl;

public interface IDropTable : IExecute
{
    /// <summary>
    /// Drop a table using a class.
    /// </summary>
    /// <param name="named">The name of the table.</param>
    /// <typeparam name="T">The type of the table to drop.</typeparam>
    /// <returns>(FluentAPI) The ability to continue to create a table.</returns>
    IDropTable FromType<T>(string named = null);

    /// <summary>
    /// Specify the name for the table to be dropped.
    /// </summary>
    /// <param name="name">The name to use for the table.</param>
    /// <returns>(FluentAPI) The ability to continue to drop a table.</returns>
    IDropTable Named(string name);

    /// <summary>
    /// Whether or not this drop table statement should cascade to dependent objects.
    /// </summary>
    /// <returns>(FluentAPI) The ability to continue dropping a table.</returns>
    IDropTable WithCascade();
}