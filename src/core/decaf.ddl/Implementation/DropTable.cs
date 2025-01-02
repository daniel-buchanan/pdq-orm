using decaf.common;
using decaf.common.Utilities.Reflection;
using decaf.Implementation.Execute;
using decaf.state;

namespace decaf.ddl.Implementation;

public class DropTable : Execute<IDropTableQueryContext>, IDropTable
{
    private readonly IReflectionHelper reflectionHelper;

    private DropTable(
        IDropTableQueryContext context,
        IQueryContainerInternal query) :
        base(query, context)
    {
        reflectionHelper = context.ToInternal().ReflectionHelper;
        query.SetContext(context);
    }

    public static DropTable Create(
        IDropTableQueryContext context,
        IQueryContainer query)
        => new DropTable(context, query as IQueryContainerInternal);

    public IDropTable FromType<T>(string named = null)
    {
        var tblName = reflectionHelper.GetTableName<T>();
        Context.WithName(!string.IsNullOrWhiteSpace(named) 
            ? named 
            : tblName);
        return this;
    }

    public IDropTable Named(string name)
    {
        Context.WithName(name);
        return this;
    }

    public IDropTable WithCascade()
    {
        Context.WithCascade();
        return this;
    }
}