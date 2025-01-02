using System;
using System.Linq.Expressions;

namespace decaf.ddl;

public interface IDdlColumnBuilder
{
    string Name { get; }
    bool Nullable { get; }
    Type Type { get; }
    
    IDdlColumnBuilder Named(string name);
    
    IDdlColumnBuilder Named<T>();

    IDdlColumnBuilder Named<T, TValue>(Expression<Func<T, TValue>> expression);

    IDdlColumnBuilder IsNullable();

    IDdlColumnBuilder IsNullable(bool nullable);

    IDdlColumnBuilder AsType<T>();

    IDdlColumnBuilder AsType(Type type);

    IDdlColumnBuilder AsType<T>(Expression<Func<T, object>> expression);
}