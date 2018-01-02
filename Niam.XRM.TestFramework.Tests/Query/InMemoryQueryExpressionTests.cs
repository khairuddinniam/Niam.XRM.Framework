using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Xunit;

namespace Niam.XRM.TestFramework.Tests.Query
{
    public class InMemoryQueryExpressionTests : TestBase
    {
        [Fact]
        public void Can_filter_columns()
        {
            var reference = new EntityReference("xts_related", Guid.NewGuid());
            var money = new Money(100m);
            var option = new OptionSetValue(123);

            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            entity.Set(e => e.xts_relatedid, reference);
            entity.Set(e => e.xts_money, money);
            entity.Set(e => e.AttributeWithCaseChar, option);
            Db["ENTITY"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<xts_entity>(e => e.xts_money)
            };
            query.Criteria.AddCondition(Helper.Name<xts_entity>(e => e.Id), ConditionOperator.Equal, entity.Id);
            var results = Test.Service.RetrieveMultiple(query);
            var retrieved = results.Entities[0].ToEntity<xts_entity>();
            Assert.Equal(entity.ToEntityReference(), retrieved.ToEntityReference());
            Assert.False(retrieved.ContainsAny(e => e.xts_relatedid));
            Assert.False(retrieved.ContainsAny(e => e.AttributeWithCaseChar));
            Assert.Equal(money, retrieved.Get(e => e.xts_money));
        }

        [Fact]
        public void Can_differentiate_entity()
        {
            var reference = new EntityReference("xts_related", Guid.NewGuid());

            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            entity.Set(e => e.xts_attribute, reference);
            Db["ENTITY-001"] = entity;

            var other = new Entity("other")
            {
                Id = Guid.NewGuid(),
                ["xts_attribute"] = reference
            };
            Db["ENTITY-002"] = other;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_attribute, ConditionOperator.Equal, reference.Id);
            
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Single(results.Entities);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        [Fact]
        public void Can_limit_result_using_topcount()
        {
            var reference = new EntityReference("xts_related", Guid.NewGuid());

            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            entity.Set(e => e.xts_attribute, reference);
            Db["ENTITY-001"] = entity;

            var other = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            other.Set(e => e.xts_attribute, reference);
            Db["ENTITY-002"] = other;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_attribute, ConditionOperator.Equal, reference.Id);

            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(2, results.Entities.Count);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
            Assert.Equal(other.ToEntityReference(), results.Entities[1].ToEntityReference());

            query.TopCount = 1;
            results = Test.Service.RetrieveMultiple(query);
            Assert.Single(results.Entities);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        [Fact]
        public void Can_paging()
        {
            var reference = new EntityReference("xts_related", Guid.NewGuid());

            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            entity.Set(e => e.xts_attribute, reference);
            Db["ENTITY-001"] = entity;

            var other = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            other.Set(e => e.xts_attribute, reference);
            Db["ENTITY-002"] = other;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_attribute, ConditionOperator.Equal, reference.Id);

            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(2, results.Entities.Count);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
            Assert.Equal(other.ToEntityReference(), results.Entities[1].ToEntityReference());

            query.PageInfo = new PagingInfo
            {
                Count = 1,
                PageNumber = 1
            };
            results = Test.Service.RetrieveMultiple(query);
            Assert.Single(results.Entities);
            Assert.True(results.MoreRecords);
            Assert.NotNull(results.PagingCookie);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());

            query.PageInfo.PageNumber++;
            query.PageInfo.PagingCookie = results.PagingCookie;
            results = Test.Service.RetrieveMultiple(query);
            Assert.Single(results.Entities);
            Assert.False(results.MoreRecords);
            Assert.NotNull(results.PagingCookie);
            Assert.Equal(other.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        [Theory]
        [MemberData(nameof(GetCompareEqualityData))]
        public void Can_compare_condition_equal(Entity entity, string attributeName, object value)
        {
            Db["ENTITY"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition(attributeName, ConditionOperator.Equal, value);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        [Theory]
        [MemberData(nameof(GetCompareEqualityData))]
        public void Can_compare_condition_not_equal(Entity entity, string attributeName, object value)
        {
            Db["ENTITY"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition(attributeName, ConditionOperator.NotEqual, value);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.False(results.Entities.Any());
        }

        public static IEnumerable<object[]> GetCompareEqualityData()
        {
            var reference = new EntityReference("xts_related", Guid.NewGuid());
            var money = new Money(100m);
            var option = new OptionSetValue(123);

            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            entity.Set(e => e.xts_relatedid, reference);
            entity.Set(e => e.xts_money, money);
            entity.Set(e => e.AttributeWithCaseChar, option);

            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.Id), entity.Id };
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.xts_relatedid), reference.Id };
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.xts_money), money.Value };
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.AttributeWithCaseChar), option.Value };
        }

        [Theory]
        [MemberData(nameof(GetCompareInData))]
        public void Can_compare_condition_in(xts_entity entity, string attributeName, object[] values)
        {
            var reference = new EntityReference("xts_related", Guid.NewGuid());
            var money = new Money(300m);
            var option = new OptionSetValue(789);
            entity.Set(e => e.xts_relatedid, reference);
            entity.Set(e => e.xts_money, money);
            entity.Set(e => e.AttributeWithCaseChar, option);

            Db["ENTITY"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition(attributeName, ConditionOperator.NotIn, values);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        [Theory]
        [MemberData(nameof(GetCompareInData))]
        public void Can_compare_condition_not_in(Entity entity, string attributeName, object[] values)
        {
            Db["ENTITY"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition(attributeName, ConditionOperator.In, values);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        public static IEnumerable<object[]> GetCompareInData()
        {
            var reference1 = new EntityReference("xts_related", Guid.NewGuid());
            var reference2 = new EntityReference("xts_related", Guid.NewGuid());
            var money1 = new Money(100m);
            var money2 = new Money(200m);
            var option1 = new OptionSetValue(123);
            var option2 = new OptionSetValue(456);
            var entity = new xts_entity { Id = Guid.NewGuid() };

            entity.Set(e => e.xts_relatedid, reference1);            
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.xts_relatedid), new object[] { reference1.Id, reference2.Id } };
            entity.Set(e => e.xts_relatedid, reference2);
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.xts_relatedid), new object[] { reference1.Id, reference2.Id } };

            entity.Set(e => e.xts_money, money1);
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.xts_money), new object[] { money1.Value, money2.Value } };
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.xts_money), new object[] { 100, 200 } };
            entity.Set(e => e.xts_money, money2);
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.xts_money), new object[] { money1.Value, money2.Value } };
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.xts_money), new object[] { 100, 200 } };

            entity.Set(e => e.AttributeWithCaseChar, option1);
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.AttributeWithCaseChar), new object[] { option1.Value, option2.Value } };
            entity.Set(e => e.AttributeWithCaseChar, option2);
            yield return new object[] { entity, Helper.Name<xts_entity>(e => e.AttributeWithCaseChar), new object[] { option1.Value, option2.Value } };
        }

        [Fact]
        public void Can_compare_condition_null()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };

            Db["ENTITY"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_relatedid, ConditionOperator.NotNull);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.False(results.Entities.Any());

            query.Criteria.AddCondition<xts_entity>(e => e.xts_money, ConditionOperator.NotNull);
            results = Test.Service.RetrieveMultiple(query);
            Assert.False(results.Entities.Any());

            query.Criteria.AddCondition<xts_entity>(e => e.AttributeWithCaseChar, ConditionOperator.NotNull);
            results = Test.Service.RetrieveMultiple(query);
            Assert.False(results.Entities.Any());
        }

        [Fact]
        public void Can_compare_condition_not_null()
        {
            var reference = new EntityReference("xts_related", Guid.NewGuid());
            var money = new Money(300m);
            var option = new OptionSetValue(789);

            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_relatedid, reference);
            entity.Set(e => e.xts_money, money);
            entity.Set(e => e.AttributeWithCaseChar, option);

            Db["ENTITY"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_relatedid, ConditionOperator.NotNull);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());

            query.Criteria.AddCondition<xts_entity>(e => e.xts_money, ConditionOperator.NotNull);
            results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());

            query.Criteria.AddCondition<xts_entity>(e => e.AttributeWithCaseChar, ConditionOperator.NotNull);
            results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        [Fact]
        public void Can_compare_condition_equal_userid()
        {
            var userId = Guid.NewGuid();
            Test.QueryParameter.UserId = userId;

            var relatedRef = new EntityReference("systemuser", userId);
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_relatedid, relatedRef);
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_relatedid, ConditionOperator.EqualUserId);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        [Fact]
        public void Can_compare_condition_not_equal_userid()
        {
            var userId = Guid.NewGuid();
            Test.QueryParameter.UserId = userId;

            var relatedRef = new EntityReference("systemuser", userId);
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_relatedid, relatedRef);
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_relatedid, ConditionOperator.NotEqualUserId);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.False(results.Entities.Any());
        }

        [Fact]
        public void Can_compare_condition_equal_businessunitid()
        {
            var businessUnitId = Guid.NewGuid();
            Test.QueryParameter.BusinessUnitId = businessUnitId;

            var relatedRef = new EntityReference("businessunit", businessUnitId);
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_relatedid, relatedRef);
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_relatedid, ConditionOperator.EqualBusinessId);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        [Fact]
        public void Can_compare_condition_not_equal_businessunitid()
        {
            var businessUnitId = Guid.NewGuid();
            Test.QueryParameter.BusinessUnitId = businessUnitId;

            var relatedRef = new EntityReference("businessunit", businessUnitId);
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_relatedid, relatedRef);
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_relatedid, ConditionOperator.NotEqualBusinessId);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.False(results.Entities.Any());
        }

        [Fact]
        public void Can_compare_condition_on()
        {
            var date = new DateTime(2016, 11, 23, 0, 0, 0, DateTimeKind.Utc);
            var compareDate = new DateTime(2016, 11, 23, 0, 0, 0, DateTimeKind.Utc);
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_date, date);
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_date, ConditionOperator.On, compareDate);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        [Fact]
        public void Can_compare_condition_not_on()
        {
            var date = new DateTime(2016, 11, 23, 0, 0, 0, DateTimeKind.Utc);
            var compareDate = new DateTime(2016, 11, 23, 0, 0, 0, DateTimeKind.Utc);
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_date, date);
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_date, ConditionOperator.NotOn, compareDate);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.False(results.Entities.Any());
        }

        [Theory]
        [MemberData(nameof(GetOnOrBeforeData))]
        public void Can_compare_condition_on_or_before(DateTime date)
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_date, date);
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_date, ConditionOperator.OnOrBefore, new DateTime(2016, 11, 23, 0, 0, 0, DateTimeKind.Utc));
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        public static IEnumerable<object[]> GetOnOrBeforeData()
        {
            yield return new object[] { new DateTime(2016, 11, 23, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { new DateTime(2016, 11, 22, 0, 0, 0, DateTimeKind.Utc) };
        }

        [Theory]
        [MemberData(nameof(GetOnOrAfterData))]
        public void Can_compare_condition_on_or_after(DateTime date)
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_date, date);
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition<xts_entity>(e => e.xts_date, ConditionOperator.OnOrAfter, new DateTime(2016, 11, 23, 0, 0, 0, DateTimeKind.Utc));
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        public static IEnumerable<object[]> GetOnOrAfterData()
        {
            yield return new object[] { new DateTime(2016, 11, 24, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { new DateTime(2016, 11, 23, 0, 0, 0, DateTimeKind.Utc) };
        }

        [Theory]
        [MemberData(nameof(GetGreaterThanData))]
        public void Can_compare_condition_greater_than(object attributeValue, object comparerValue)
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid(),
                ["xts_attribute"] = attributeValue
            };
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition("xts_attribute", ConditionOperator.GreaterThan, comparerValue);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        public static IEnumerable<object[]> GetGreaterThanData()
        {
            yield return new object[] { 2, 1 };
            yield return new object[] { 4.5, 4.4 };
            yield return new object[] { new Money(100m), 50m };
            yield return new object[] { new OptionSetValue(12), 11 };
            yield return new object[] { new EntityReference("entity", Guid.NewGuid()), Guid.Empty };
        }

        [Theory]
        [MemberData(nameof(GetLessThanData))]
        public void Can_compare_condition_less_than(object attributeValue, object comparerValue)
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid(),
                ["xts_attribute"] = attributeValue
            };
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition("xts_attribute", ConditionOperator.LessThan, comparerValue);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        public static IEnumerable<object[]> GetLessThanData()
        {
            yield return new object[] { 2, 3 };
            yield return new object[] { 4.5, 4.6 };
            yield return new object[] { new Money(100m), 100.1m };
            yield return new object[] { new OptionSetValue(12), 13 };
            yield return new object[] { new EntityReference("entity", Guid.Empty), Guid.NewGuid() };
        }

        [Theory]
        [MemberData(nameof(GetGreaterEqualData))]
        public void Can_compare_condition_greater_equal(object attributeValue, object comparerValue)
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid(),
                ["xts_attribute"] = attributeValue
            };
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition("xts_attribute", ConditionOperator.GreaterEqual, comparerValue);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        public static IEnumerable<object[]> GetGreaterEqualData()
        {
            yield return new object[] { 2, 1 };
            yield return new object[] { 2, 2 };
            yield return new object[] { 4.5, 4.4 };
            yield return new object[] { 4.5, 4.5 };
            yield return new object[] { new Money(100m), 50m };
            yield return new object[] { new Money(100m), 100m };
            yield return new object[] { new OptionSetValue(12), 11 };
            yield return new object[] { new OptionSetValue(12), 12 };
            yield return new object[] { new EntityReference("entity", Guid.NewGuid()), Guid.Empty };
            var id = Guid.NewGuid();
            yield return new object[] { new EntityReference("entity", id), id };
        }

        [Theory]
        [MemberData(nameof(GetLessEqualData))]
        public void Can_compare_condition_less_equal(object attributeValue, object comparerValue)
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid(),
                ["xts_attribute"] = attributeValue
            };
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition("xts_attribute", ConditionOperator.LessEqual, comparerValue);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        public static IEnumerable<object[]> GetLessEqualData()
        {
            yield return new object[] { 2, 3 };
            yield return new object[] { 2, 2 };
            yield return new object[] { 4.5, 4.6 };
            yield return new object[] { 4.5, 4.5 };
            yield return new object[] { new Money(100m), 100.1m };
            yield return new object[] { new Money(100m), 100m };
            yield return new object[] { new OptionSetValue(12), 13 };
            yield return new object[] { new OptionSetValue(12), 12 };
            yield return new object[] { new EntityReference("entity", Guid.Empty), Guid.NewGuid() };
            var id = Guid.NewGuid();
            yield return new object[] { new EntityReference("entity", id), id };
        }

        [Theory]
        [MemberData(nameof(GetBetweenData))]
        public void Can_compare_condition_between(object attributeValue, object lowerBoundary, object upperBoundary)
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid(),
                ["xts_attribute"] = attributeValue
            };
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition("xts_attribute", ConditionOperator.Between, lowerBoundary, upperBoundary);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        public static IEnumerable<object[]> GetBetweenData()
        {
            yield return new object[] { 2, 1, 3 };
            yield return new object[] { 2, 2, 3 };
            yield return new object[] { 2, 1, 2 };

            yield return new object[] { 4.5, 4.4, 4.6 };
            yield return new object[] { 4.5, 4.5, 4.6 };
            yield return new object[] { 4.5, 4.4, 4.5 };

            yield return new object[] { new Money(100m), 50m, 150m };
            yield return new object[] { new Money(100m), 100m, 150m };
            yield return new object[] { new Money(100m), 50m, 100m };

            yield return new object[] { new OptionSetValue(12), 11, 13 };
            yield return new object[] { new OptionSetValue(12), 12, 13 };
            yield return new object[] { new OptionSetValue(12), 11, 12 };
        }

        [Theory]
        [MemberData(nameof(GetNotBetweenData))]
        public void Can_compare_condition_not_between(object attributeValue, object lowerBoundary, object upperBoundary)
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid(),
                ["xts_attribute"] = attributeValue
            };
            Db["ENTITY-001"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition("xts_attribute", ConditionOperator.NotBetween, lowerBoundary, upperBoundary);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        public static IEnumerable<object[]> GetNotBetweenData()
        {
            yield return new object[] { 3, 1, 2 };
            yield return new object[] { 3, 4, 5 };

            yield return new object[] { 4.5, 4.3, 4.4 };
            yield return new object[] { 4.5, 4.6, 4.7 };

            yield return new object[] { new Money(100m), 50m, 99m };
            yield return new object[] { new Money(100m), 101m, 150m };

            yield return new object[] { new OptionSetValue(12), 10, 11 };
            yield return new object[] { new OptionSetValue(12), 13, 14 };
        }

        [Fact]
        public void Can_compare_filter_criteria_and()
        {
            var reference = new EntityReference("xts_related", Guid.NewGuid());
            var money = new Money(100m);
            var option = new OptionSetValue(123);

            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            entity.Set(e => e.xts_relatedid, reference);
            entity.Set(e => e.xts_money, money);
            entity.Set(e => e.AttributeWithCaseChar, option);
            Db["ENTITY"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = { FilterOperator = LogicalOperator.And }
            };
            query.Criteria.AddCondition<xts_entity>(e => e.Id, ConditionOperator.Equal, entity.Id);
            query.Criteria.AddCondition<xts_entity>(e => e.xts_money, ConditionOperator.NotEqual, 100m);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.False(results.Entities.Any());

            query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = { FilterOperator = LogicalOperator.And }
            };
            query.Criteria.AddCondition<xts_entity>(e => e.Id, ConditionOperator.Equal, entity.Id);
            query.Criteria.AddCondition<xts_entity>(e => e.xts_money, ConditionOperator.Equal, 100m);
            results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }

        [Fact]
        public void Can_compare_filter_criteria_or()
        {
            var reference = new EntityReference("xts_related", Guid.NewGuid());
            var money = new Money(100m);
            var option = new OptionSetValue(123);

            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            entity.Set(e => e.xts_relatedid, reference);
            entity.Set(e => e.xts_money, money);
            entity.Set(e => e.AttributeWithCaseChar, option);
            Db["ENTITY"] = entity;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = { FilterOperator = LogicalOperator.Or }
            };
            query.Criteria.AddCondition<xts_entity>(e => e.Id, ConditionOperator.Equal, entity.Id);
            query.Criteria.AddCondition<xts_entity>(e => e.xts_money, ConditionOperator.Equal, 200m);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());

            query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = { FilterOperator = LogicalOperator.Or }
            };
            query.Criteria.AddCondition<xts_entity>(e => e.Id, ConditionOperator.Equal, Guid.NewGuid());
            query.Criteria.AddCondition<xts_entity>(e => e.xts_money, ConditionOperator.Equal, 100m);
            results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());

            query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = { FilterOperator = LogicalOperator.Or }
            };
            query.Criteria.AddCondition<xts_entity>(e => e.Id, ConditionOperator.Equal, Guid.NewGuid());
            query.Criteria.AddCondition<xts_entity>(e => e.xts_money, ConditionOperator.Equal, 200m);
            results = Test.Service.RetrieveMultiple(query);
            Assert.False(results.Entities.Any());
        }

        [Fact]
        public void Can_order_by_date()
        {
            var firstDate = new DateTime(2017, 3, 2);
            var secondDate = new DateTime(2017, 3, 3);

            var first = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            first.Set(e => e.xts_date, firstDate);
            Db["FIRST"] = first;

            var second = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            second.Set(e => e.xts_date, secondDate);
            Db["SECOND"] = second;

            var query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.AddOrder<xts_entity>(e => e.xts_date, OrderType.Descending);

            var results = Test.Service.RetrieveMultiple(query).Entities;
            Assert.Equal(2, results.Count);
            Assert.Equal(second.ToEntityReference(), results[0].ToEntityReference());
            Assert.Equal(first.ToEntityReference(), results[1].ToEntityReference());

            query = new QueryExpression(xts_entity.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(true)
            };
            query.AddOrder<xts_entity>(e => e.xts_date, OrderType.Descending);

            results = Test.Service.RetrieveMultiple(query).Entities;
            Assert.Single(results);
            Assert.Equal(second.ToEntityReference(), results[0].ToEntityReference());
        }
    }
}
