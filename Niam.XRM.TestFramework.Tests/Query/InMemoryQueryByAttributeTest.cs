using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Xunit;

namespace Niam.XRM.TestFramework.Tests.Query
{
    public class InMemoryQueryByAttributeTest : TestBase
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

            var query = new QueryByAttribute(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<xts_entity>(e => e.xts_money)
            };
            query.AddAttributeValue(Helper.Name<xts_entity>(e => e.Id), entity.Id);
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

            var query = new QueryByAttribute(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.AddAttributeValue<xts_entity>(e => e.xts_attribute, reference.Id);

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

            var query = new QueryByAttribute(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.AddAttributeValue<xts_entity>(e => e.xts_attribute, reference.Id);

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

            var query = new QueryByAttribute(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.AddAttributeValue<xts_entity>(e => e.xts_attribute, reference.Id);

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
        public void Can_compare(Entity entity, string attributeName, object value)
        {
            Db["ENTITY"] = entity;

            var query = new QueryByAttribute(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.AddAttributeValue(attributeName, value);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
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

        [Fact]
        public void Can_compare_multi_conditions()
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

            var query = new QueryByAttribute(xts_entity.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            query.AddAttributeValue<xts_entity>(e => e.Id, entity.Id);
            query.AddAttributeValue<xts_entity>(e => e.xts_money, 100m);
            var results = Test.Service.RetrieveMultiple(query);
            Assert.Equal(entity.ToEntityReference(), results.Entities[0].ToEntityReference());
        }
    }
}
