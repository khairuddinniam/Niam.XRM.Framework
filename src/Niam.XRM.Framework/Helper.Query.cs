using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Infrastructure;

namespace Niam.XRM.Framework
{
    public static partial class Helper
    {
        // QueryExpression
        public static FilterExpression AddCondition<T>(this FilterExpression filter,
            Expression<Func<T, object>> attribute,
            ConditionOperator conditionOperator, params object[] values)
            where T : Entity
        {
            filter.AddCondition(Name(attribute), conditionOperator, values);
            return filter;
        }

        public static FilterExpression AddCondition<T>(this FilterExpression filter, string entityAlias,
            Expression<Func<T, object>> attribute,
            ConditionOperator conditionOperator,
            params object[] values)
            where T : Entity
        {
            filter.AddCondition(entityAlias, Name(attribute), conditionOperator, values);
            return filter;
        }

        public static void AddOrder<T>(this QueryExpression query, Expression<Func<T, object>> attribute, 
            OrderType orderType)
            where T : Entity => query.AddOrder(Name(attribute), orderType);

        public static LinkEntity AddLink<TFrom, TTo>(this QueryExpression query,
            Expression<Func<TFrom, object>> fromAttribute,
            Expression<Func<TTo, object>> toAttribute)
            where TFrom : Entity
            where TTo : Entity =>
                query.AddLink(Name<TTo>(), Name(fromAttribute), Name(toAttribute));

        public static LinkEntity AddLink<TFrom, TTo>(this QueryExpression query,
            Expression<Func<TFrom, object>> fromAttribute,
            Expression<Func<TTo, object>> toAttribute,
            JoinOperator joinOperator)
            where TFrom : Entity
            where TTo : Entity =>
                query.AddLink(Name<TTo>(), Name(fromAttribute), Name(toAttribute), joinOperator);

        public static LinkEntity AddLink<TFrom, TTo>(this LinkEntity linkEntity,
            Expression<Func<TFrom, object>> fromAttribute,
            Expression<Func<TTo, object>> toAttribute)
            where TFrom : Entity
            where TTo : Entity =>
                linkEntity.AddLink(Name<TTo>(), Name(fromAttribute), Name(toAttribute));
        
        public static LinkEntity AddLink<TFrom, TTo>(this LinkEntity linkEntity,
            Expression<Func<TFrom, object>> fromAttribute,
            Expression<Func<TTo, object>> toAttribute,
            JoinOperator joinOperator)
            where TFrom : Entity
            where TTo : Entity =>
                linkEntity.AddLink(Name<TTo>(), Name(fromAttribute), Name(toAttribute), joinOperator);

        public static LinkEntity SetColumns(this LinkEntity linkEntity, ColumnSet columns)
        {
            linkEntity.Columns = columns;
            return linkEntity;
        }

        public static LinkEntity SetColumns<TEntity>(
            this LinkEntity linkEntity, params Expression<Func<TEntity, object>>[] attributes) where TEntity : Entity
        {
            return SetColumns(linkEntity, new ColumnSet<TEntity>(attributes));
        }
        
        public static LinkEntity SetAlias(this LinkEntity linkEntity, string alias)
        {
            linkEntity.EntityAlias = alias;
            return linkEntity;
        }

        // QueryByAttribute
        public static void AddAttributeValue<T>(this QueryByAttribute query, Expression<Func<T, object>> attribute, 
            object value) 
            where T : Entity => query.AddAttributeValue(Name(attribute), value);

        public static void AddOrder<T>(this QueryByAttribute query, Expression<Func<T, object>> attribute, 
            OrderType orderType)
            where T : Entity => query.AddOrder(Name(attribute), orderType);

        public static Entity GetAliasedEntity(this Entity entity, string name)
            => GetAliasedEntity<Entity>(entity, name);

        public static T GetAliasedEntity<T>(this Entity entity, string name)
            where T : Entity
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var aliasedName = name + ".";
            var aliasedAttributes = entity.Attributes
                .Where(attr => attr.Key.StartsWith(aliasedName) && attr.Value is AliasedValue)
                .Select(attr =>
                {
                    var attributeName = attr.Key.Substring(aliasedName.Length);
                    var attributeValue = ((AliasedValue)attr.Value).Value;
                    return new KeyValuePair<string, object>(attributeName, attributeValue);
                });

            var aliasedEntity = Instance<T>.Create();
            foreach (var attribute in aliasedAttributes)
                aliasedEntity.Set(attribute.Key, attribute.Value);

            return aliasedEntity;
        }
    }
}
