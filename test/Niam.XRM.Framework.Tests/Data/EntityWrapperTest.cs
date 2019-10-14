using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Xunit;

namespace Niam.XRM.Framework.Tests.Data
{
    public class EntityWrapperTest
    {
        [Fact]
        public void Can_retrieve_related()
        {
            var relatedEntity = new Entity("xts_relatedentity")
            {
                Id = Guid.NewGuid()
            };

            var service = Substitute.For<IOrganizationService>();
            service.Retrieve(Arg.Is(relatedEntity.LogicalName), Arg.Is(relatedEntity.Id), Arg.Any<ColumnSet>())
                   .Returns(relatedEntity);

            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(service);
            
            var entity = new Entity("xts_entity")
            {
                Id = Guid.NewGuid()
            };
            entity["xts_attribute"] = relatedEntity.ToEntityReference();;

            var wrapper = new TestWrapper(entity, context);
            Assert.Equal(relatedEntity.ToEntityReference(), wrapper.GetRelated("xts_attribute", new ColumnSet()).ToEntityReference());

            var genericWrapper = new XtsEntityWrapper(entity.ToEntity<xts_entity>(), context);
            Assert.Equal(relatedEntity.ToEntityReference(), genericWrapper.GetRelated("xts_attribute", new ColumnSet()).ToEntityReference());
            Assert.Equal(relatedEntity.ToEntityReference(), genericWrapper.GetRelated(e => e.xts_attribute, new ColumnSet<xts_relatedentity>()).ToEntityReference());
            Assert.Equal(relatedEntity.ToEntityReference(), genericWrapper.GetRelated<xts_relatedentity, XtsRelatedEntityWrapper>(e => e.xts_attribute, new ColumnSet<xts_relatedentity>()).ToEntityReference());
        }

        [Fact]
        public void Can_retrieve_all_related()
        {
            var relatedEntity = new Entity("xts_relatedentity")
            {
                Id = Guid.NewGuid()
            };

            var entity = new Entity("xts_entity")
            {
                Id = Guid.NewGuid()
            };

            var service = Substitute.For<IOrganizationService>();
            service.RetrieveMultiple(Arg.Any<QueryBase>()).Returns(ci =>
            {
                var query = ci.ArgAt<QueryExpression>(0);
                var condition = query.Criteria.Conditions[0];
                Assert.Equal("xts_relatedentity", query.EntityName);
                Assert.Equal("xts_relatedid", condition.AttributeName);
                Assert.Equal(ConditionOperator.Equal, condition.Operator);
                Assert.Equal(entity.Id, condition.Values[0]);
                return new EntityCollection { Entities = { relatedEntity } };
            });

            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(service);

            var wrapper = new TestWrapper(entity, context);
            Assert.Equal(relatedEntity.ToEntityReference(), wrapper.GetAllRelated("xts_relatedentity", "xts_relatedid", new ColumnSet()).First().ToEntityReference());
            Assert.Equal(relatedEntity.ToEntityReference(), wrapper.GetAllRelated(e => e.xts_relatedid, new ColumnSet<xts_relatedentity>()).First().ToEntityReference());
            Assert.Equal(relatedEntity.ToEntityReference(), wrapper.GetAllRelated<xts_relatedentity, XtsRelatedEntityWrapper>(e => e.xts_relatedid, new ColumnSet<xts_relatedentity>()).First().ToEntityReference());

            var genericWrapper = new XtsEntityWrapper(entity.ToEntity<xts_entity>(), context);
            Assert.Equal(relatedEntity.ToEntityReference(), genericWrapper.GetAllRelated("xts_relatedentity", "xts_relatedid", new ColumnSet()).First().ToEntityReference());
            Assert.Equal(relatedEntity.ToEntityReference(), genericWrapper.GetAllRelated(e => e.xts_relatedid, new ColumnSet<xts_relatedentity>()).First().ToEntityReference());
            Assert.Equal(relatedEntity.ToEntityReference(), genericWrapper.GetAllRelated<xts_relatedentity, XtsRelatedEntityWrapper>(e => e.xts_relatedid, new ColumnSet<xts_relatedentity>()).First().ToEntityReference());
        }

        private class TestWrapper : EntityWrapper<Entity>
        {
            public TestWrapper(Entity entity, ITransactionContextBase context)
                : base(entity, context)
            {
            }
        }

        private class XtsEntityWrapper : EntityWrapper<xts_entity>
        {
            public XtsEntityWrapper(xts_entity entity, ITransactionContextBase context) 
                : base(entity, context)
            {
            }
        }

        private class XtsRelatedEntityWrapper : EntityWrapper<xts_relatedentity>
        {
            public XtsRelatedEntityWrapper(xts_relatedentity entity, ITransactionContextBase context) 
                : base(entity, context)
            {
            }
        }

        [Fact]
        public void Can_convert_to_derived_wrapper()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            var reference = new EntityReference("reference", Guid.NewGuid());
            entity[Helper.Name<xts_derivedentity>(e => e.xts_derivedattribute)] = reference;
            var service = Substitute.For<IOrganizationService>();

            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(service);

            var entityWrapper = new EntityWrapper<xts_entity>(entity, context);
            var testWrapper = entityWrapper.ToWrapper<xts_entity, ConvertTestWrapper<xts_entity>>();
            var derivedWrapper = testWrapper.ToWrapper<xts_derivedentity, ConvertDerivedTestWrapper<xts_derivedentity>>();
            Assert.Equal(reference, derivedWrapper.Entity.GetAttributeValue<EntityReference>("xts_derivedattribute"));

            var sameDerivedWrapper = derivedWrapper.ToWrapper<xts_derivedentity, ConvertDerivedTestWrapper<xts_derivedentity>>();
            Assert.Same(derivedWrapper, sameDerivedWrapper);
        }

        private class ConvertTestWrapper<T> : EntityWrapper<T> where T : xts_entity
        {
            public ConvertTestWrapper(T entity, ITransactionContextBase context)
                : base(entity, context)
            {
            }
        }

        private class ConvertDerivedTestWrapper<T> : ConvertTestWrapper<T> where T : xts_derivedentity
        {
            public ConvertDerivedTestWrapper(T entity, ITransactionContextBase context)
                : base(entity, context)
            {
            }
        }

        [Fact]
        public void Can_get_name()
        {
            var keyEntity = new xts_keytest { Id = Guid.NewGuid() };
            keyEntity.Set(e => e.xts_key, "Hello world");

            var service = Substitute.For<IOrganizationService>();
            service.Retrieve(Arg.Is<string>(name => name == "xts_keytest"), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(keyEntity);

            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(service);

            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_referenceid, keyEntity.ToEntityReference());
            var wrapper = new EntityWrapper<xts_entity>(entity, context);

            Assert.Equal("Hello world", wrapper.GetReferenceName<xts_keytest>(e => e.xts_referenceid));
        }
    }
}
