using System;

namespace decaf.common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
	public class TableNameAttribute : Attribute
	{
        public string Name { get; set; }

        public bool CaseSensitive { get; set; }

        public TableNameAttribute() { }

        public TableNameAttribute(string name)
        {
            Name = name;
        }
    }
}

