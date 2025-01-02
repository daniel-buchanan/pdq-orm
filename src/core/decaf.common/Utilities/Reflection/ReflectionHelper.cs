using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using decaf.common.Utilities.Reflection.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using decaf.common.Attributes;

namespace decaf.common.Utilities.Reflection
{
    public sealed class ReflectionHelper : IReflectionHelper
    {
        public object GetPropertyValue(object o, string member)
        {
            // check inputs
            if (o == null) throw new ArgumentNullException(nameof(o));
            if (member == null) throw new ArgumentNullException(nameof(member));

            // get the type of the object
            var scope = o.GetType();

            // convert to metadata provider
            var provider = o as IDynamicMetaObjectProvider;

            // if object is a provider
            if (provider != null)
            {
                // check object contains requested property
                if (!((IDictionary<string, object>)provider).ContainsKey(member))
                {
                    throw new ArgumentException($"'{member}' could not be found in this object.");
                }

                // get the parameter
                var param = Expression.Parameter(typeof(object));

                // get the meta data object
                var mobj = provider.GetMetaObject(param);

                // get the binder
                var binder = (GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, member, scope, new[] { CSharpArgumentInfo.Create(0, null) });

                // bind the member
                var ret = mobj.BindGetMember(binder);

                // create expression
                var final = Expression.Block(
                    Expression.Label(CallSiteBinder.UpdateLabel),
                    ret.Expression
                );

                // create lambda
                var lambda = Expression.Lambda(final, param);

                // compile lambda
                var del = lambda.Compile();

                // invoke lambda and return
                return del.DynamicInvoke(o);
            }

            // get member type
            var memberObj = o.GetType().GetProperty(member, BindingFlags.Public | BindingFlags.Instance);

            if (memberObj == null)
            {
                throw new ArgumentException($"'{member}' could not be found in this object.");
            }

            // get value from member
            var value = memberObj.GetValue(o, null);

            // check if value is null
            if (value != null)
            {
                // get the underlying type
                var underlyingType = memberObj.PropertyType;

                // check if property is nullable
                var isNullable = IsNullableType(underlyingType);

                // if its nullable
                if (isNullable)
                {
                    // get underlying top
                    underlyingType = Nullable.GetUnderlyingType(underlyingType);

                    // if there isn't one, re-set
                    if (underlyingType == null) underlyingType = memberObj.PropertyType;
                }

                // if underlying type is object
                if (underlyingType == typeof(object) || underlyingType == typeof(Object))
                {
                    // get the value type
                    underlyingType = value.GetType();
                }

                // convert the value to the underlying type
                value = Convert.ChangeType(value, underlyingType);
            }

            // return value
            return value;
        }

        public Type GetMemberType(object o, string member)
        {
            // get member type
            var memberType = o.GetType();

            // get the property
            var memberObj = memberType.GetProperty(member, BindingFlags.Public | BindingFlags.Instance);

            // get property type
            return memberObj.PropertyType;
        }

        public T GetInstanceObject<T>(object obj, string name, Type toCreate, object[] instanceParams)
        {
            // get the type of the parameter
            var ptype = GetMemberType(obj, name);

            //add the model type for the type def of the repository
            Type[] args = { ptype };

            //get the generic type definition for the model
            var constructedField = toCreate.MakeGenericType(args);

            //create instance of the repository, typed for the model
            var instanceField = Activator.CreateInstance(constructedField, instanceParams);

            // return instance
            return (T)instanceField;
        }

        public object GetInstanceObject(Type toCreate, Type typeParam, object[] instanceParams)
        {
            // create arguments array
            Type[] args = { typeParam };

            // create generic type
            var constructedField = toCreate.MakeGenericType(args);

            // get instance
            var instance = Activator.CreateInstance(constructedField, instanceParams);

            // return instance
            return instance;
        }

        public List<DynamicColumnInfo> GetMemberDetails(dynamic toUse, QueryTypes cmdType = QueryTypes.None)
        {
            // create list
            var tList = new List<DynamicColumnInfo>();

            // convert to metadata provider
            var tTarget = toUse as IDynamicMetaObjectProvider;

            // get member names from provider
            if (tTarget != null)
                tList.AddRange(tTarget.GetMetaObject(Expression.Constant(tTarget)).GetDynamicMemberNames().Select(m => DynamicColumnInfo.Create(name: m, newName: m)));
            // get from normal type
            else
                tList.AddRange(GetMemberDetails((Type)toUse.GetType(), cmdType));

            // return list of members
            return tList;
        }

        public List<DynamicColumnInfo> GetMemberDetails(Type toUse, QueryTypes queryType = QueryTypes.None)
        {
            var fieldNames = new List<DynamicColumnInfo>();

            // get object type and properties
            var objType = toUse;
            var properties = objType.GetProperties();

            //igore any properties which have ignore set

            properties = (from p in properties
                          where p.GetCustomAttributes(typeof(IgnoreColumnForAttribute), true)
                                 .Any(a =>
                                 {
                                     var attr = (IgnoreColumnForAttribute)a;
                                     return !attr.QueryType.HasFlag(queryType) ||
                                        attr.QueryType == QueryTypes.None;
                                 }) ||
                                 !p.GetCustomAttributes().Any()
                          select p).ToArray();

            // iternate through properties
            foreach (var p in properties)
            {
                // get field name
                var name = GetFieldName(p);

                // add to set
                fieldNames.Add(DynamicColumnInfo.Create(
                    name: name,
                    newName: p.Name,
                    valueType: p.PropertyType));
            }

            return fieldNames;
        }

        public string GetTableName<T>()
        {
            // get type
            var tp = typeof(T);

            // get table name
            return GetTableName(tp);
        }

        public string GetTableName(Type tp)
        {
            // get attributes
            var attributes = tp.GetCustomAttributes(typeof(TableNameAttribute), true);

            // there should only be one
            var attr = attributes.FirstOrDefault();

            // if we have an attribute, return specified name
            if (attr != null)
            {
                var tableAttr = (TableNameAttribute)attr;
                if (tableAttr.CaseSensitive) return $"\"{tableAttr.Name}\"";
                return tableAttr.Name;
            }

            // return class name
            return tp.Name;
        }

        public string GetFieldName(PropertyInfo field, QueryTypes queryType = QueryTypes.None)
        {
            // get rename attributes
            var attributes = field.GetCustomAttributes();

            // there should only be one
            var renameAttr = attributes.FirstOrDefault(a => a is RenameColumnAttribute) as RenameColumnAttribute;
            var ignoreAttr = attributes.FirstOrDefault(a => a is IgnoreColumnForAttribute) as IgnoreColumnForAttribute;

            if (ignoreAttr?.QueryType.HasFlag(queryType) == true) return null;

            // if we have an attribute, return specified name
            if (renameAttr != null) return renameAttr.Name;

            // return property name
            return field.Name;
        }

        public string GetFieldName(MemberInfo field)
        {
            // get rename attributes
            var attributes = field.GetCustomAttributes(typeof(RenameColumnAttribute), false);

            // there should only be one
            var attr = attributes.FirstOrDefault();

            // if we have an attribute, return specified name
            if (attr != null) return ((RenameColumnAttribute)attr).Name;

            // return member name
            return field.Name;
        }

        public bool IsNullableType(Type toCheck)
        {
            // determine kind of type
            bool genericType = toCheck.IsGenericType && toCheck.GetGenericTypeDefinition() == typeof(Nullable<>);
            bool primitiveOrValueType = toCheck.IsPrimitive || toCheck.IsValueType;

            // if it is a generic type, it is nullable, if it is primitive, it isnt
            return genericType || !primitiveOrValueType;
        }

        public bool IsNullableType<T>()
        {
            // determine if nullable
            return IsNullableType(typeof(T));
        }

        public IEnumerable<AttributeInfo> GetAttributes<TSource, TAttribute>() where TAttribute : Attribute
        {
            var attributes = new List<AttributeInfo>();
            var classType = typeof(TSource);
            var classAttrs = classType.GetCustomAttributes(inherit: true, attributeType: typeof(TAttribute));
            var classAttributes = classAttrs
                .Select(a => AttributeInfo.Create(classType.Name, classType, SourceType.Class, a));
            var properties = classType.GetProperties();
            var propertyAttributes = properties.SelectMany(p =>
            {
                var attrs = p.GetCustomAttributes(typeof(TAttribute));
                return attrs.Select(a => AttributeInfo.Create(p.Name, p.PropertyType, SourceType.Property, a));
            });
            attributes.AddRange(classAttributes);
            attributes.AddRange(propertyAttributes);
            return attributes;
        }

        public Type GetUnderlyingType<T>(T someType)
        {
            return GetUnderlyingType<T>();
        }

        public Type GetUnderlyingType(Type t)
        {
            // check if type is nullable
            var isNullable = IsNullableType(t);

            // default to provided type
            var underlyingType = t;

            // if type is nullable
            if (isNullable)
            {
                // get underlying type value
                underlyingType = Nullable.GetUnderlyingType(underlyingType);
            }

            // if no underlying type, return provided
            return underlyingType ?? t;
        }

        public Type GetUnderlyingType<T>()
        {
            // get underlying type
            return GetUnderlyingType(typeof(T));
        }

        public bool DetermineIfAnonymous(Type type)
        {
            // check for properties of anon types
            var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            var nameContainsAnonymousType = type.FullName.Contains("AnonymousType");

            // check if both are true
            var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            // return if anon
            return isAnonymousType;
        }
    }
}

