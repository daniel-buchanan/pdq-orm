using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using decaf.common;
using decaf.common.Utilities.Reflection;
using decaf.common.Utilities.Reflection.Dynamic;
using decaf.state;

namespace decaf.Implementation.Execute
{
    internal abstract class UpdateBase : Execute<IUpdateQueryContext>
    {
        protected UpdateBase(IQueryContainerInternal query, IUpdateQueryContext context)
            : base(query, context)
        {
            Query.SetContext(Context);
        }

        protected void FromQuery(Action<ISelectWithAlias> queryBuilder)
        {
            var context = SelectQueryContext.Create(Query.AliasManager, Query.HashProvider);
            var query = Query.UnitOfWork.GetQuery() as IQueryContainerInternal;
            var select = Select.Create(context, query) as ISelectWithAlias;

            queryBuilder(select);
            var source = state.QueryTargets.SelectQueryTarget.Create(context, select.Alias);
            Context.From(source);
        }

        protected void SetValues<T>(IEnumerable<T> values)
        {
            foreach(var v in values) SetValues(v);
        }

        private void SetValues<T>(T value)
        {
            var internalContext = Context as IQueryContextExtended;
            var paramExpression = Expression.Parameter(typeof(T), "p");
            var constExpression = Expression.Constant(value);
            var valueExpression = Expression.Lambda(constExpression, paramExpression);

            var props = internalContext.DynamicExpressionHelper
                .GetProperties(valueExpression, internalContext)
                .ToList();
            
            for(var i = 0; i < props.Count; i += 1)
            {
                var p = props[i];
                if (PropertyIsKey(value, p)) continue;
                
                var column = Column.Create(p.Name, Context.Table);
                var v = p.Value;
                var valueType = p.ValueType;
                var defaultValue = DefaultValueHelper.Get(valueType);
                if (v?.Equals(defaultValue) == true ||
                    (v == null && defaultValue == null))
                    continue;

                var source = state.ValueSources.Update.StaticValueSource.Create(column, valueType, v);
                Context.Set(source);
            }
        }

        private static bool PropertyIsKey<T>(T value, DynamicColumnInfo info)
        {
            var metadata = value.GetPropertyValue("KeyMetadata");
            if (metadata == null) return false;

            if(metadata.GetType().Name == "KeyMetadata`1")
            {
                var keyName = metadata.GetPropertyValue("Name") as string;
                return keyName == info.Name;
            }
            else if(metadata.GetType().Name == "CompositeKey")
            {
                var componentOne = metadata.GetPropertyValue("ComponentOne");
                var componentTwo = metadata.GetPropertyValue("ComponentTwo");
                var valueOne = componentOne.GetPropertyValue("Name") as string;
                var valueTwo = componentTwo.GetPropertyValue("Name") as string;
                return info.Name == valueOne || info.Name == valueTwo;
            }
            else if(metadata.GetType().Name == "CompositeKeyTriple")
            {
                var componentOne = metadata.GetPropertyValue("ComponentOne");
                var componentTwo = metadata.GetPropertyValue("ComponentTwo");
                var componentThree = metadata.GetPropertyValue("ComponentThree");
                var valueOne = componentOne.GetPropertyValue("Name") as string;
                var valueTwo = componentTwo.GetPropertyValue("Name") as string;
                var valueThree = componentThree.GetPropertyValue("Name") as string;
                return info.Name == valueOne ||
                    info.Name == valueTwo ||
                    info.Name == valueThree;
            }

            return false;
        }
    }
}

