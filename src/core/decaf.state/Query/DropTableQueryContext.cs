using System.Collections.Generic;
using decaf.common;
using decaf.common.Utilities;
using decaf.state.Ddl.Definitions;

namespace decaf.state;

internal class DropTableQueryContext : QueryContext, IDropTableQueryContext
{
    public DropTableQueryContext(
        IAliasManager aliasManager,
        IHashProvider hashProvider) :
        base(aliasManager, QueryTypes.DropTable, hashProvider)
    {
    }

    public static IDropTableQueryContext Create(IAliasManager aliasManager, IHashProvider hashProvider)
        => new DropTableQueryContext(aliasManager, hashProvider);

    public string Name { get; private set; }

    public void WithName(string name) => Name = name;

    public bool Cascade { get; private set; }

    public void WithCascade() => Cascade = true;
}