using System;

namespace decaf.common.Utilities.Reflection;

public class AttributeInfo
{
    public SourceType Source { get; private set; }
    public string AppliedTo { get; private set; }
    public Type AppliedType { get; private set; }
    public object Attribute { get; protected set; }
    
    public static AttributeInfo Create(string appliedTo, Type appliedType, SourceType source, object attribute)
        => new()
        {
            AppliedTo = appliedTo,
            AppliedType = appliedType,
            Source = source,
            Attribute = attribute
        };

    public static AttributeInfo<T> Create<T>(string appliedTo, Type appliedType, SourceType source, T attribute)
        where T : Attribute
        => new()
        {
            AppliedTo = appliedTo,
            AppliedType = appliedType,
            Source = source,
            Attribute = attribute
        };
}

public class AttributeInfo<T> : AttributeInfo
    where T : Attribute
{
    public new T Attribute
    {
        get => (T)base.Attribute;
        set => base.Attribute = value;
    }
}

public enum SourceType
{
    Class,
    Property,
    Field,
    Namespace
}