using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class TransactionContextEntityTest
    {
        [Fact]
        public void Can_get_set_indexer()
        {
            var entity = new Entity("entity")
            {
                Id = Guid.NewGuid()
            };

            var txEntity = new TransactionContextEntity<Entity>(entity)
            {
                ["xts_attribute"] = new Money(2500m)
            };

            Assert.Equal(2500m, ((Money)txEntity["xts_attribute"]).Value);
            Assert.Equal(2500m, ((Money)entity["xts_attribute"]).Value);
        }

        [Fact]
        public void Can_execute_attribute_changing()
        {
            var entity = new Entity("entity")
            {
                Id = Guid.NewGuid(),
                ["xts_attribute"] = new Money(200m)
            };

            var txEntity = new TransactionContextEntity<Entity>(entity);

            Entity senderEntity = null;
            string propertyName = null;
            Money value = null;
            txEntity.AttributeChanging += (sender, e) =>
            {
                senderEntity = (Entity)sender;
                propertyName = e.PropertyName;
                value = ((Entity)sender).GetAttributeValue<Money>(e.PropertyName);
            };

            txEntity["xts_attribute"] = new Money(450m);

            Assert.Equal(entity, senderEntity);
            Assert.Equal("xts_attribute", propertyName);
            Assert.Equal(200m, value.Value);
        }

        [Fact]
        public void Can_execute_attribute_changed()
        {
            var entity = new Entity("entity")
            {
                Id = Guid.NewGuid(),
                ["xts_attribute"] = new Money(200m)
            };

            var txEntity = new TransactionContextEntity<Entity>(entity);

            Entity senderEntity = null;
            string propertyName = null;
            Money value = null;
            txEntity.AttributeChanged += (sender, e) =>
            {
                senderEntity = (Entity) sender;
                propertyName = e.PropertyName;
                value = ((Entity) sender).GetAttributeValue<Money>(e.PropertyName);
            };

            txEntity["xts_attribute"] = new Money(450m);

            Assert.Equal(entity, senderEntity);
            Assert.Equal("xts_attribute", propertyName);
            Assert.Equal(450m, value.Value);
        }

        [Fact]
        public void Entity_object_is_same()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            var txEntity = new TransactionContextEntity<xts_entity>(entity);
            Assert.Same(entity, txEntity.Entity);
        }
        
        [Fact]
        public void Can_get_set_value()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            var txEntity = new TransactionContextEntity<xts_entity>(entity);
            var reference = new EntityReference("reference", Guid.NewGuid());
            txEntity.Set(e => e.xts_attribute, reference);

            Assert.Equal(reference, entity.GetAttributeValue<EntityReference>("xts_attribute"));
            Assert.Equal(reference, txEntity.Get(e => e.xts_attribute));
        }
    }
}
