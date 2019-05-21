using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class HelperQueryTests
    {
        [Fact]
        public void Can_add_condition()
        {
            var filter = new FilterExpression();
            filter.AddCondition<xts_entity>(e => e.xts_money, ConditionOperator.Equal, 200m);

            var condition = filter.Conditions[0];
            Assert.Null(condition.EntityName);
            Assert.Equal("xts_money", condition.AttributeName);
            Assert.Equal(new object[] { 200m }, condition.Values.ToArray());
        }

        [Fact]
        public void Can_add_condition_alias()
        {
            var filter = new FilterExpression();
            filter.AddCondition<xts_entity>("alias", e => e.xts_money, ConditionOperator.Equal, 200m);

            var condition = filter.Conditions[0];
            Assert.Equal("alias", condition.EntityName);
            Assert.Equal("xts_money", condition.AttributeName);
            Assert.Equal(new object[] { 200m }, condition.Values.ToArray());
        }

        [Fact]
        public void Can_add_attribute_value()
        {
            var query = new QueryByAttribute(xts_entity.EntityLogicalName);
            query.AddAttributeValue<xts_entity>(e => e.xts_money, 200m);

            Assert.Equal("xts_money", query.Attributes[0]);
            Assert.Equal(200m, query.Values[0]);
        }

        [Fact]
        public void Can_add_order_on_query_expression()
        {
            var query = new QueryExpression(xts_entity.EntityLogicalName);
            query.AddOrder<xts_entity>(e => e.xts_attribute, OrderType.Descending);

            Assert.Equal("xts_attribute", query.Orders[0].AttributeName);
            Assert.Equal(OrderType.Descending, query.Orders[0].OrderType);
        }

        [Fact]
        public void Can_add_order_on_query_by_attribute()
        {
            var query = new QueryByAttribute(xts_entity.EntityLogicalName);
            query.AddOrder<xts_entity>(e => e.xts_attribute, OrderType.Descending);

            Assert.Equal("xts_attribute", query.Orders[0].AttributeName);
            Assert.Equal(OrderType.Descending, query.Orders[0].OrderType);
        }

        [Fact]
        public void Can_add_link_generic_from_to_on_query_expression()
        {
            var query = new QueryExpression(xts_entity.EntityLogicalName);
            query.AddLink<xts_entity, xts_relatedentity>(e => e.xts_attribute, e => e.xts_relatedid);

            var linkEntity = query.LinkEntities[0];
            Assert.Equal("xts_entity", linkEntity.LinkFromEntityName);
            Assert.Equal("xts_relatedentity", linkEntity.LinkToEntityName);
            Assert.Equal("xts_attribute", linkEntity.LinkFromAttributeName);
            Assert.Equal("xts_relatedid", linkEntity.LinkToAttributeName);
        }

        [Fact]
        public void Can_add_link_generic_from_to_join_operator_on_query_expression()
        {
            var query = new QueryExpression(xts_entity.EntityLogicalName);
            query.AddLink<xts_entity, xts_relatedentity>(e => e.xts_attribute, e => e.xts_relatedid, JoinOperator.LeftOuter);

            var linkEntity = query.LinkEntities[0];
            Assert.Equal("xts_entity", linkEntity.LinkFromEntityName);
            Assert.Equal("xts_relatedentity", linkEntity.LinkToEntityName);
            Assert.Equal("xts_attribute", linkEntity.LinkFromAttributeName);
            Assert.Equal("xts_relatedid", linkEntity.LinkToAttributeName);
            Assert.Equal(JoinOperator.LeftOuter, linkEntity.JoinOperator);
        }

        [Fact]
        public void Can_add_link_generic_from_to_on_link_entity()
        {
            var linkEntity = new LinkEntity
            {
                LinkFromEntityName = xts_entity.EntityLogicalName
            };
            linkEntity.AddLink<xts_entity, xts_relatedentity>(e => e.xts_attribute, e => e.xts_relatedid);

            var linkedLinkEntity = linkEntity.LinkEntities[0];
            Assert.Equal("xts_entity", linkedLinkEntity.LinkFromEntityName);
            Assert.Equal("xts_relatedentity", linkedLinkEntity.LinkToEntityName);
            Assert.Equal("xts_attribute", linkedLinkEntity.LinkFromAttributeName);
            Assert.Equal("xts_relatedid", linkedLinkEntity.LinkToAttributeName);
        }

        [Fact]
        public void Can_add_link_generic_from_to_join_operator_on_link_entity()
        {
            var linkEntity = new LinkEntity
            {
                LinkFromEntityName = xts_entity.EntityLogicalName
            };
            linkEntity.AddLink<xts_entity, xts_relatedentity>(e => e.xts_attribute, e => e.xts_relatedid, JoinOperator.LeftOuter);

            var linkedLinkEntity = linkEntity.LinkEntities[0];
            Assert.Equal("xts_entity", linkedLinkEntity.LinkFromEntityName);
            Assert.Equal("xts_relatedentity", linkedLinkEntity.LinkToEntityName);
            Assert.Equal("xts_attribute", linkedLinkEntity.LinkFromAttributeName);
            Assert.Equal("xts_relatedid", linkedLinkEntity.LinkToAttributeName);
            Assert.Equal(JoinOperator.LeftOuter, linkedLinkEntity.JoinOperator);
        }

        [Fact]
        public void Can_get_aliased_entity()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_int, 123);
            entity.Set(e => e.xts_money, 200m);
            entity["rel.xts_money"] = new AliasedValue(
                "xts_relatedentity",
                "xts_money",
                new Money(450m)
            );
            entity["rel.xts_column"] = new AliasedValue(
                "xts_relatedentity",
                "xts_column",
                "MY-ALIASED-TEXT"
            );

            var aliasedEntity = entity.GetAliasedEntity<xts_relatedentity>("rel");
            Assert.Equal(450m, aliasedEntity.Get(e => e.xts_money).Value);
            Assert.Equal("MY-ALIASED-TEXT", aliasedEntity.Get(e => e.xts_withcolumnattribute));
        }
    }
}
