using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework
{
    public static partial class Helper
    {
        public static TV Get<T, TV>(this IEntityGetter<T> accessor, Expression<Func<T, TV>> attribute)
            where T : Entity => accessor != null ? accessor.Get<TV>(Name(attribute)) : default(TV);

        public static decimal GetValue<T>(this IEntityGetter<T> accessor, Expression<Func<T, Money>> moneyAttribute, decimal defaultValue = 0m)
            where T : Entity => Get(accessor, moneyAttribute)?.GetValueOrDefault(defaultValue) ?? defaultValue;

        public static TV GetValue<T, TV>(this IEntityGetter<T> accessor, Expression<Func<T, TV?>> attribute, TV defaultValue = default(TV))
            where T : Entity
            where TV : struct
            => Get(accessor, attribute).GetValueOrDefault(defaultValue);

        public static TV GetAliasedValue<TV>(this IEntityGetter<Entity> accessor, string attributeName)
            => (TV) accessor.Get<AliasedValue>(attributeName)?.Value;

        public static string GetFormattedValue<T>(this IEntityGetter<T> accessor, Expression<Func<T, object>> attribute)
            where T : Entity
            => accessor.GetFormattedValue(Name(attribute));

        public static void SetFormattedValue<T>(
            this IEntitySetter<T> accessor, Expression<Func<T, object>> attribute, string formattedValue)
            where T : Entity
            => accessor.SetFormattedValue(Name(attribute), formattedValue);

        public static void Set<T, TV>(this IEntitySetter<T> accessor, Expression<Func<T, TV>> attribute, IAttributeValueProvider<T, TV> attributeValueProvider)
            where T : Entity => accessor.Set(Name(attribute), attributeValueProvider.GetValueFor(attribute));

        public static void Set<T, TV>(this IEntitySetter<T> accessor, Expression<Func<T, TV>> attribute, IValueProvider<TV> valueProvider)
            where T : Entity => accessor.Set(Name(attribute), valueProvider.GetValue());

        public static void Set<T, TV>(this IEntitySetter<T> accessor, Expression<Func<T, TV?>> attribute, IValueProvider<TV> valueProvider)
            where T : Entity
            where TV : struct => accessor.Set(Name(attribute), valueProvider.GetValue());

        public static void Set<T, TV>(this IEntitySetter<T> accessor, Expression<Func<T, TV>> attribute, TV value)
            where T : Entity => accessor.Set(GetMemberInfo(attribute), value);

        public static void Set<T>(this IEntitySetter<T> accessor, Expression<Func<T, OptionSetValue>> attribute, Enum value)
            where T : Entity => Set(accessor, attribute, value.ToOptionSetValue());

        public static void Set<T>(this IEntitySetter<T> accessor, Expression<Func<T, OptionSetValue>> attribute, int value)
            where T : Entity => Set(accessor, attribute, new OptionSetValue(value));

        public static void Set<T>(this IEntitySetter<T> accessor, Expression<Func<T, Money>> attribute, decimal value)
            where T : Entity => Set(accessor, attribute, new Money(value));

        public static void Set<T>(this IEntitySetter<T> accessor, Expression<Func<T, Money>> attribute, IValueProvider<decimal> valueProvider)
            where T : Entity => Set(accessor, attribute, valueProvider.GetValue());

        public static void Set<T>(this IEntitySetter<T> accessor, Expression<Func<T, EntityCollection>> attribute,
            EntityReference valueRef, params EntityReference[] otherValueRefs)
            where T : Entity => Set(accessor, attribute, new[] { valueRef }.Concat(otherValueRefs));

        public static void Set<T>(this IEntitySetter<T> accessor, Expression<Func<T, EntityCollection>> attribute,
            IEnumerable<EntityReference> valueRefs)
            where T : Entity
        {
            var activityParties = valueRefs
                .Select(partyRef => new Entity("activityparty") { Attributes = { ["partyid"] = partyRef } })
                .ToList();
            var collection = new EntityCollection(activityParties);
            accessor.Set(attribute, collection);
        }

        public static bool Equal<T, TV>(this IEntityGetter<T> accessor, Expression<Func<T, TV>> attribute, TV comparisonValue)
            where T : Entity => Equal(Get(accessor, attribute), comparisonValue);

        public static bool Equal<T, TV>(this IEntityGetter<T> accessor, string attribute, TV comparisonValue)
            where T : Entity => Equal(accessor.Get<TV>(attribute), comparisonValue);

        public static bool Equal<T>(this IEntityGetter<T> accessor, Expression<Func<T, OptionSetValue>> attribute, Enum option)
            where T : Entity => Equal(Get(accessor, attribute), option.ToOptionSetValue());

        public static bool EqualsAny<T>(this IEntityGetter<T> accessor, Expression<Func<T, OptionSetValue>> attribute, Enum firstOption, params Enum[] otherOptions)
            where T : Entity => EqualsAny(Get(accessor, attribute), firstOption, otherOptions);
    }
}
