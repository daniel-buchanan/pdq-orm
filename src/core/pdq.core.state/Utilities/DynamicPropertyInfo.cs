﻿using System;
using System.Linq.Expressions;

namespace pdq.state.Utilities
{
	public class DynamicPropertyInfo
	{
		private DynamicPropertyInfo(
			string name,
			string newName,
			object value,
			Type valueType,
			Type type,
			string alias,
			IValueFunction function)
		{
            Name = name;
			NewName = newName;
			Value = value;
			ValueType = valueType;
			Type = type;
			Alias = alias;
			Function = function;
        }

		public static DynamicPropertyInfo Empty()
			=> new DynamicPropertyInfo(null, null, null, null, null, null, null);

		public static DynamicPropertyInfo Create(
			string name = null,
			string newName = null,
			object value = null,
			Type valueType = null,
			Type type = null,
			string alias = null,
			IValueFunction function = null)
			=> new DynamicPropertyInfo(name, newName, value, valueType, type, alias, function);

		public string Name { get; private set; }

		public string NewName { get; private set; }

		public object Value { get; private set; }

		public Type ValueType { get; private set; }

		public Type Type { get; private set; }

		public string Alias { get; private set; }

		public IValueFunction Function { get; private set; }

        public void SetName(string value) => Name = value;

        public void SetNewName(string value) => NewName = value;

		public void SetValue(object value) => Value = value;

		public void SetValueType(Type type) => ValueType = type;

		public void SetType(Type type) => Type = type;

		public void SetAlias(string alias) => Alias = alias;

		public void SetFunction(IValueFunction function) => Function = function;

		public bool IsEquivalentTo(DynamicPropertyInfo propertyInfo)
        {
			var equal = Name == propertyInfo.Name &&
				NewName == propertyInfo.NewName &&
				Alias == propertyInfo.Alias;

			if (equal) return true;

			return Type == propertyInfo.Type;
        }
	}
}

