using System;

namespace decaf.ddl.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class PrimaryKeyComponentAttribute : Attribute { }