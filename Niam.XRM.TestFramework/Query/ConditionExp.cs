using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.TestFramework.Query
{
    public static class ConditionExp
    {
        public static bool Equal(Entity entity, ConditionExpression condition)
        {
            var comparatorValue = condition.GetSingleValue();
            var attributeValue = condition.AttributeName == entity.LogicalName + "id"
                ? entity.Id
                : entity.GetAttributeValue<object>(condition.AttributeName);
            if (attributeValue == null) return false;

            if (attributeValue is EntityReference reference)
                return reference.Id.Equals(comparatorValue);

            if (attributeValue is Money money)
                return money.Value.Equals(comparatorValue);

            if (attributeValue is OptionSetValue option)
                return option.Value.Equals(comparatorValue);

            return attributeValue.Equals(comparatorValue);
        }

        public static bool NotEqual(Entity entity, ConditionExpression condition)
            => !Equal(entity, condition);

        public static bool In(Entity entity, ConditionExpression condition)
        {
            if (!condition.Values.Any())
                throw new InvalidOperationException("Condition value must not empty.");

            var haveNullValue = condition.Values.Any(val => val == null);
            if (haveNullValue) throw new InvalidOperationException("Condition value must not null.");

            var attributeValue = condition.AttributeName == entity.LogicalName + "id"
                ? entity.Id
                : entity.GetAttributeValue<object>(condition.AttributeName);
            if (attributeValue == null) return false;

            if (attributeValue is EntityReference reference)
                return condition.Values.Cast<Guid>().Any(val => val == reference.Id);

            if (attributeValue is Money money)
                return condition.Values.Any(val => Convert.ToDecimal(val) == money.Value);

            if (attributeValue is OptionSetValue option)
                return condition.Values.Cast<int>().Any(val => val == option.Value);

            return condition.Values.Any(val => attributeValue.Equals(val));
        }

        public static bool NotIn(Entity entity, ConditionExpression condition)
            => !In(entity, condition);

        public static bool Null(Entity entity, ConditionExpression condition)
            => entity.GetAttributeValue<object>(condition.AttributeName) == null;

        public static bool NotNull(Entity entity, ConditionExpression condition)
            => !Null(entity, condition);

        public static bool EqualUserId(Entity entity, ConditionExpression condition, IQueryParameter queryParameter)
        {
            if (condition.Values.Any())
                throw new InvalidOperationException("Condition values must empty.");

            var attributeValue = condition.AttributeName == entity.LogicalName + "id"
                ? entity.ToEntityReference()
                : entity.GetAttributeValue<EntityReference>(condition.AttributeName);

            return attributeValue != null && attributeValue.Id == queryParameter.UserId;
        }

        public static bool NotEqualUserId(Entity entity, ConditionExpression condition, IQueryParameter queryParameter)
            => !EqualUserId(entity, condition, queryParameter);

        public static bool EqualBusinessId(Entity entity, ConditionExpression condition, IQueryParameter queryParameter)
        {
            if (condition.Values.Any())
                throw new InvalidOperationException("Condition values must empty.");

            var attributeValue = condition.AttributeName == entity.LogicalName + "id"
                ? entity.ToEntityReference()
                : entity.GetAttributeValue<EntityReference>(condition.AttributeName);

            return attributeValue != null && attributeValue.Id == queryParameter.BusinessUnitId;
        }

        public static bool NotEqualBusinessId(Entity entity, ConditionExpression condition, IQueryParameter queryParameter)
            => !EqualBusinessId(entity, condition, queryParameter);

        public static bool On(Entity entity, ConditionExpression condition)
        {
            var comparatorValue = condition.GetSingleValue<DateTime>();
            var attributeValue = entity.GetAttributeValue<DateTime?>(condition.AttributeName);
            return attributeValue != null && attributeValue.Value.Equals(comparatorValue);
        }

        public static bool NotOn(Entity entity, ConditionExpression condition)
            => !On(entity, condition);

        public static bool OnOrBefore(Entity entity, ConditionExpression condition)
        {
            var comparatorValue = condition.GetSingleValue<DateTime>();
            var attributeValue = entity.GetAttributeValue<DateTime?>(condition.AttributeName);
            return attributeValue != null && attributeValue.Value <= comparatorValue;
        }

        public static bool OnOrAfter(Entity entity, ConditionExpression condition)
        {
            var comparatorValue = condition.GetSingleValue<DateTime>();
            var attributeValue = entity.GetAttributeValue<DateTime?>(condition.AttributeName);
            return attributeValue != null && attributeValue.Value >= comparatorValue;
        }

        public static bool GreaterThan(Entity entity, ConditionExpression condition)
        {
            var comparatorValue = condition.GetSingleValue() as IComparable;
            var attributeValue = entity.GetAttributeValue<object>(condition.AttributeName);
            if (attributeValue is EntityReference)
                attributeValue = ((EntityReference) attributeValue).Id;
            else if (attributeValue is Money)
                attributeValue = ((Money) attributeValue).Value;
            else if (attributeValue is OptionSetValue)
                attributeValue = ((OptionSetValue) attributeValue).Value;
            else
                attributeValue = attributeValue as IComparable;
            
            return (comparatorValue != null && attributeValue != null) && (comparatorValue.CompareTo(attributeValue) == -1);
        }

        public static bool GreaterEqual(Entity entity, ConditionExpression condition)
            => GreaterThan(entity, condition) || Equal(entity, condition);

        public static bool LessThan(Entity entity, ConditionExpression condition) 
            => !GreaterThan(entity, condition);

        public static bool LessEqual(Entity entity, ConditionExpression condition)
            => LessThan(entity, condition) || Equal(entity, condition);

        public static bool Between(Entity entity, ConditionExpression condition)
        {
            var greaterEqualCondition = new ConditionExpression(condition.AttributeName, condition.Operator, condition.Values[0]);
            var lessEqualCondition = new ConditionExpression(condition.AttributeName, condition.Operator, condition.Values[1]);
            return GreaterEqual(entity, greaterEqualCondition) && LessEqual(entity, lessEqualCondition);
        }

        public static bool NotBetween(Entity entity, ConditionExpression condition)
            => !Between(entity, condition);

        // Helpers
        public static object GetSingleValue(this ConditionExpression condition)
            => GetSingleValue<object>(condition);

        public static T GetSingleValue<T>(this ConditionExpression condition)
        {
            if (!condition.Values.Any())
                throw new InvalidOperationException("Condition values must not empty.");

            if (condition.Values.Count > 1)
                throw new InvalidOperationException("Condition values must only one value.");

            var comparatorValue = condition.Values[0];
            if (comparatorValue == null) throw new InvalidOperationException("Condition value must not null.");

            return (T) comparatorValue;
        }
    }
}
