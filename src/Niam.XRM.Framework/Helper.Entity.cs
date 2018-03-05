using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework
{
    public static partial class Helper
    {
        public static EntityAccessor<T> ToEntityAccessor<T>(this Entity entity)
            where T : Entity => ToEntityAccessor(entity.ToEntity<T>());

        public static EntityAccessor<T> ToEntityAccessor<T>(this T entity)
            where T : Entity => new EntityAccessor<T>(entity);

        public static TV Get<T, TV>(this T entity, Expression<Func<T, TV>> attribute)
            where T : Entity => Get<TV>(entity, Name(attribute));

        public static TV Get<TV>(this Entity entity, string attributeName) 
            => entity.GetAttributeValue<TV>(attributeName);

        public static decimal GetValue<T>(this T entity, Expression<Func<T, Money>> moneyAttribute, decimal defaultValue = 0m)
            where T : Entity => Get(entity, moneyAttribute).GetValueOrDefault(defaultValue);

        public static int? GetValue<T>(this T entity, Expression<Func<T, OptionSetValue>> optionAttribute, int? defaultValue = null)
            where T : Entity => Get(entity, optionAttribute)?.Value ?? defaultValue;

        public static TV GetValue<T, TV>(this T entity, Expression<Func<T, TV?>> attribute, TV defaultValue = default(TV))
            where T : Entity
            where TV : struct
            => Get(entity, attribute).GetValueOrDefault(defaultValue);

        public static string GetFormattedValue(this Entity entity, string attributeName)
            => entity.FormattedValues.TryGetValue(attributeName, out string value) ? value : null;

        public static string GetFormattedValue<T>(this T entity, Expression<Func<T, object>> attribute)
            where T : Entity
            => GetFormattedValue(entity, Name(attribute));

        public static void SetFormattedValue(this Entity entity, string attributeName, string formattedValue)
            => entity.FormattedValues[attributeName] = formattedValue;

        public static void SetFormattedValue<T>(
            this T entity, Expression<Func<T, object>> attribute, string formattedValue)
            where T : Entity
            => SetFormattedValue(entity, Name(attribute), formattedValue);

        public static void Set<T, TV>(this T entity, Expression<Func<T, TV>> attribute, IAttributeValueProvider<T, TV> attributeValueProvider)
            where T : Entity => Set(entity, attribute, attributeValueProvider.GetValueFor(attribute));

        public static void Set<T, TV>(this T entity, Expression<Func<T, TV>> attribute, IValueProvider<TV> valueProvider)
            where T : Entity => Set(entity, attribute, valueProvider.GetValue());

        public static void Set<T, TV>(this T entity, Expression<Func<T, TV>> attribute, IAttributeValueProvider valueProvider)
            where T : Entity => Set(entity, attribute, (TV) valueProvider.GetValueFor(Name<T>(), Name(attribute)));

        public static void Set<T, TV>(this T entity, Expression<Func<T, TV?>> attribute, IValueProvider<TV> valueProvider)
            where T : Entity
            where TV : struct => Set(entity, attribute, valueProvider.GetValue());

        public static void Set<T, TV>(this T entity, Expression<Func<T, TV>> attribute, TV value)
            where T : Entity => Set(entity, GetMemberInfo(attribute), value);

        public static void Set<T>(this T entity, Expression<Func<T, OptionSetValue>> attribute, Enum value)
            where T : Entity => Set(entity, attribute, value.ToOptionSetValue());

        public static void Set<T>(this T entity, Expression<Func<T, OptionSetValue>> attribute, int? value)
            where T : Entity => Set(entity, attribute, value.HasValue ? new OptionSetValue(value.Value) : null);

        public static void Set<T>(this T entity, Expression<Func<T, Money>> attribute, decimal value)
            where T : Entity => Set(entity, attribute, new Money(value));

        public static void Set<T>(this T entity, Expression<Func<T, Money>> attribute, IValueProvider<decimal> valueProvider)
            where T : Entity => Set(entity, attribute, valueProvider.GetValue());

        public static void Set<T>(this T entity, Expression<Func<T, EntityCollection>> attribute,
            EntityReference valueRef, params EntityReference[] otherValueRefs)
            where T : Entity => Set(entity, attribute, new[] { valueRef }.Concat(otherValueRefs));

        public static void Set<T>(this T entity, Expression<Func<T, EntityCollection>> attribute,
            IEnumerable<EntityReference> valueRefs)
            where T : Entity
        {
            var activityParties = valueRefs
                .Select(partyRef => new Entity("activityparty") { Attributes = { ["partyid"] = partyRef } })
                .ToList();
            var collection = new EntityCollection(activityParties);
            entity.Set(attribute, collection);
        }

        public static void Set(this Entity entity, MemberInfo memberInfo, object value)
        {
            var info = Info(entity.GetType());
            if (info.IsCrmSvcUtilGenerated)
            {
                info.GetPropertyInfo(memberInfo.Name).SetValue(entity, value);
            }
            else
            {
                var attributeName = info.GetAttributeName(memberInfo.Name);
                Set(entity, attributeName, value);
            }
        }

        public static void Set(this Entity entity, string attributeName, object value)
        {
            entity[attributeName] = value;

            var isPrimaryField = (entity.LogicalName + "id") == attributeName;
            if (isPrimaryField)
                entity.Id = (Guid) value;
        }

        public static void Set(this Entity entity, string attributeName, IAttributeValueProvider attributeValueProvider)
            => Set(entity, attributeName, attributeValueProvider.GetValueFor(entity.LogicalName, attributeName));

        public static void Set(this Entity entity, string attributeName, IValueProvider valueProvider)
            => Set(entity, attributeName, valueProvider.GetValue());

        public static void Set<TV>(this Entity entity, string attributeName, IValueProvider<TV> valueProvider)
            => Set(entity, attributeName, valueProvider.GetValue());

        public static bool Remove<T>(this T entity, Expression<Func<T, object>> attribute)
            where T : Entity => Remove(entity, Name(attribute));

        public static bool Remove(this Entity entity, string attribute) => entity.Attributes.Remove(attribute);

        public static bool Equal<T, TV>(this T entity, Expression<Func<T, TV>> attribute, TV comparisonValue)
            where T : Entity => Equal(Get(entity, attribute), comparisonValue);

        public static bool Equal<T>(this T entity, Expression<Func<T, OptionSetValue>> attribute, Enum option)
            where T : Entity => Equal(Get(entity, attribute), option.ToOptionSetValue());

        public static bool EqualsAny<T>(this T entity, Expression<Func<T, OptionSetValue>> attribute, Enum firstOption, params Enum[] otherOptions)
            where T : Entity => EqualsAny(Get(entity, attribute), firstOption, otherOptions);

        // Get entity logical name.
        public static string Name<T>()
            where T : Entity => EntityCache<T>.Info.LogicalName;

        public static string Name(Type entityType) => Info(entityType).LogicalName;

        // Get attribute logical name.
        public static string Name<T>(this T entity, Expression<Func<T, object>> attribute)
            where T : Entity => Name(attribute);

        public static string Name<T, TV>(this T entity, Expression<Func<T, TV>> attribute)
            where T : Entity => Name(attribute);

        // Get attribute logical name.
        public static string Name<T>(Expression<Func<T, object>> attribute)
            where T : Entity => GetAttributeName<T>(attribute);

        public static string Name<T, TV>(Expression<Func<T, TV>> attribute)
            where T : Entity => GetAttributeName<T>(attribute);

        private static string GetAttributeName<T>(LambdaExpression attribute)
            where T : Entity
        {
            var memberInfo = GetMemberInfo(attribute);
            return Info<T>().GetAttributeName(memberInfo.Name);
        }

        // Get entity primary name attribute.
        public static string PrimaryAttribute<T>()
            where T : Entity => Info<T>().PrimaryNameAttribute;

        public static IEntityInfo Info<T>()
            where T : Entity => EntityCache<T>.Info;

        public static IEntityInfo Info(string entityName)
            => EntityCache.TryGetValue(entityName, out var value) ? value : null;

        public static IEntityInfo Info(Type entityType)
            => EntityCache.GetOrAddInfo(entityType);

        public static bool ContainsAny<T>(
            this T entity, 
            Expression<Func<T, object>> attribute, 
            params Expression<Func<T, object>>[] otherAttributes)
            where T : Entity
        {
            var attributeName = Name(attribute);
            var otherAttributeNames = otherAttributes.Select(Name).ToArray();
            return entity.ContainsAny(attributeName, otherAttributeNames);
        }

        public static bool ContainsAny<T>(this T entity, string attribute, params string[] otherAttributes) 
            where T : Entity => entity.Contains(attribute) || otherAttributes.Any(entity.Contains);
    }
}
