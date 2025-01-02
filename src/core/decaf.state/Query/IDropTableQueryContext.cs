using System.Collections.Generic;
using decaf.common;
using decaf.state.Ddl.Definitions;

namespace decaf.state;

public interface IDropTableQueryContext : IQueryContext
{
    string Name { get; }

    void WithName(string name);

    bool Cascade { get; }

    void WithCascade();
}