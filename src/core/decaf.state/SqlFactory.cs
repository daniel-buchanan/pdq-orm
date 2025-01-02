using System.Collections.Generic;
using System.Linq;
using decaf.common;
using decaf.common.Exceptions;
using decaf.common.Templates;
using decaf.common.Utilities;

namespace decaf.state;

public abstract class SqlFactory : ISqlFactory
{
    private readonly Dictionary<string, SqlTemplate> cache = new();

    /// <inheritdoc/>
    public SqlTemplate ParseTemplate(IQueryContext context)
    {
        SqlTemplate template;
        var key = context.GetHash();
        var existing = cache.TryGetValue(key, out template);

        if (existing) return template;

        if (context is ISelectQueryContext selectContext)
            template = ParseQuery(selectContext);
        if (context is IDeleteQueryContext deleteContext)
            template = ParseQuery(deleteContext);
        if (context is IUpdateQueryContext updateContext)
            template = ParseQuery(updateContext);
        if (context is IInsertQueryContext insertContext)
            template = ParseQuery(insertContext);
        if (context is ICreateTableQueryContext createTableContext)
            template = ParseQuery(createTableContext);
        if (context is IDropTableQueryContext dropTableContext)
            template = ParseQuery(dropTableContext);

        if (template == null) return null;
        cache.Add(key, template);
        return template;
    }

    /// <inheritdoc/>
    public object ParseParameters(IQueryContext context, SqlTemplate template, bool includePrefix = true)
    {
        IDictionary<string, object> values = null;

        if (context is ISelectQueryContext selectContext)
            values = ParseParameters(selectContext, includePrefix);
        if (context is IDeleteQueryContext deleteContext)
            values = ParseParameters(deleteContext, includePrefix);
        if (context is IUpdateQueryContext updateContext)
            values = ParseParameters(updateContext, includePrefix);
        if (context is IInsertQueryContext insertContext)
            values = ParseParameters(insertContext, includePrefix);

        if (values == null) return new { };

        if (!CheckParameters(values.Keys.ToArray(), template))
            throw new SqlTemplateMismatchException("Provided template, does not match provided parameters.");

        return MapDictionary(values);
    }

    /// <summary>
    /// Parse a select query into a SQL Template
    /// </summary>
    /// <param name="context">The context to parse.</param>
    /// <returns>A SQL Template ready to execute.</returns>
    protected abstract SqlTemplate ParseQuery(ISelectQueryContext context);

    /// <summary>
    /// Parse a delete query into a SQL Template
    /// </summary>
    /// <param name="context">The context to parse.</param>
    /// <returns>A SQL Template ready to execute.</returns>
    protected abstract SqlTemplate ParseQuery(IDeleteQueryContext context);

    /// <summary>
    /// Parse an update query into a SQL Template
    /// </summary>
    /// <param name="context">The context to parse.</param>
    /// <returns>A SQL Template ready to execute.</returns>
    protected abstract SqlTemplate ParseQuery(IUpdateQueryContext context);

    /// <summary>
    /// Parse an insert query into a SQL Template
    /// </summary>
    /// <param name="context">The context to parse.</param>
    /// <returns>A SQL Template ready to execute.</returns>
    protected abstract SqlTemplate ParseQuery(IInsertQueryContext context);

    /// <summary>
    /// Parse a create table query into a sql template.
    /// </summary>
    /// <param name="context">The context to parse</param>
    /// <returns>A SQL template, ready to execute</returns>
    protected abstract SqlTemplate ParseQuery(ICreateTableQueryContext context);
    
    /// <summary>
    /// Parse a drop table query into a sql template.
    /// </summary>
    /// <param name="context">The context to parse</param>
    /// <returns>A SQL template, ready to execute</returns>
    protected abstract SqlTemplate ParseQuery(IDropTableQueryContext context);

    /// <summary>
    /// Parse the parameters from a select context, with their values.
    /// </summary>
    /// <param name="context">The context to parse.</param>
    /// <param name="includePrefix">Whether or not to include the prefix for parameters.</param>
    /// <returns>A dictionary containing the parameters.</returns>
    protected abstract IDictionary<string, object> ParseParameters(ISelectQueryContext context, bool includePrefix);

    /// <summary>
    /// Parse the parameters from a delete context, with their values.
    /// </summary>
    /// <param name="context">The context to parse.</param>
    /// <param name="includePrefix">Whether or not to include the prefix for parameters.</param>
    /// <returns>A dictionary containing the parameters.</returns>
    protected abstract IDictionary<string, object> ParseParameters(IDeleteQueryContext context, bool includePrefix);

    /// <summary>
    /// Parse the parameters from an update context, with their values.
    /// </summary>
    /// <param name="context">The context to parse.</param>
    /// <param name="includePrefix">Whether or not to include the prefix for parameters.</param>
    /// <returns>A dictionary containing the parameters.</returns>
    protected abstract IDictionary<string, object> ParseParameters(IUpdateQueryContext context, bool includePrefix);

    /// <summary>
    /// Parse the parameters from an insert context, with their values.
    /// </summary>
    /// <param name="context">The context to parse.</param>
    /// <param name="includePrefix">Whether or not to include the prefix for parameters.</param>
    /// <returns>A dictionary containing the parameters.</returns>
    protected abstract IDictionary<string, object> ParseParameters(IInsertQueryContext context, bool includePrefix);

    /// <summary>
    /// Map dictionary values to anonymous objects.
    /// </summary>
    /// <param name="dict">The dictionary to parse.</param>
    /// <returns>An anonymous object with the properties in the source dictionary.</returns>
    private static object MapDictionary(IDictionary<string, object> dict)
        => ParameterMapper.Map(dict);

    /// <summary>
    /// Check the provided parameters against the template to ensure they are correct.
    /// </summary>
    /// <param name="parameterNames">THe names of the parameters.</param>
    /// <param name="template">An existing SQL Template to check against.</param>
    /// <returns>True if the parameters are valid, False if they aren't.</returns>
    private static bool CheckParameters(string[] parameterNames, SqlTemplate template)
    {
        if (parameterNames.Length != (template.Parameters?.Count() ?? 0))
            return false;

        var result = true;
        for(var i = 0; i < parameterNames.Length; i++)
        {
            var existing = template.Parameters.FirstOrDefault(p => p.Name.EndsWith(parameterNames[i]));
            result &= existing != null;
        }

        return result;
    }
}