using System;
using System.Collections.Generic;
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
            AttributeChangingEventArgs ev = null;
            Money value = null;
            txEntity.AttributeChanging += (sender, e) =>
            {
                senderEntity = (Entity)sender;
                ev = e;
                value = ((Entity)sender).GetAttributeValue<Money>(e.AttributeName);
            };

            txEntity["xts_attribute"] = new Money(450m);

            Assert.Equal(entity, senderEntity);
            Assert.Null(ev.PropertyName);
            Assert.Equal("xts_attribute", ev.AttributeName);
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
            AttributeChangedEventArgs ev = null;
            Money value = null;
            txEntity.AttributeChanged += (sender, e) =>
            {
                senderEntity = (Entity) sender;
                ev = e;
                value = ((Entity) sender).GetAttributeValue<Money>(e.AttributeName);
            };

            txEntity["xts_attribute"] = new Money(450m);

            Assert.Equal(entity, senderEntity);
            Assert.Null(ev.PropertyName);
            Assert.Equal("xts_attribute", ev.AttributeName);
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

    public class MsGeneratedTransactionContextEntityTest
    {
        [Fact]
        public void Can_get_set_indexer()
        {
            var entity = new MsGenerated.Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "1234"
            };

            var txEntity = new TransactionContextEntity<MsGenerated.Account>(entity);

            Assert.Equal("1234", (string) txEntity["accountnumber"]);
            Assert.Equal("1234", (string) entity["accountnumber"]);
        }

        [Fact]
        public void Can_execute_attribute_changing()
        {
            var entity = new MsGenerated.Account
            {
                Id = Guid.NewGuid(),
                CreditLimit = new Money(200m)
            };

            var txEntity = new TransactionContextEntity<MsGenerated.Account>(entity);

            Entity senderEntity = null;
            AttributeChangingEventArgs ev = null;
            Money value = null;
            txEntity.AttributeChanging += (sender, e) =>
            {
                senderEntity = (Entity) sender;
                ev = e;
                value = ((Entity) sender).GetAttributeValue<Money>(e.AttributeName);
            };

            txEntity.Entity.CreditLimit = new Money(450m);

            Assert.Equal(entity, senderEntity);
            Assert.Equal("CreditLimit", ev.PropertyName);
            Assert.Equal("creditlimit", ev.AttributeName);
            Assert.Equal(200m, value.Value);
        }

        [Fact]
        public void Can_execute_attribute_changed()
        {
            var entity = new MsGenerated.Account
            {
                Id = Guid.NewGuid(),
                CreditLimit = new Money(200m)
            };

            var txEntity = new TransactionContextEntity<MsGenerated.Account>(entity);

            Entity senderEntity = null;
            AttributeChangedEventArgs ev = null;
            Money value = null;
            txEntity.AttributeChanged += (sender, e) =>
            {
                senderEntity = (Entity) sender;
                ev = e;
                value = ((Entity) sender).GetAttributeValue<Money>(e.AttributeName);
            };

            txEntity.Entity.CreditLimit = new Money(450m);

            Assert.Equal(entity, senderEntity);
            Assert.Equal("CreditLimit", ev.PropertyName);
            Assert.Equal("creditlimit", ev.AttributeName);
            Assert.Equal(450m, value.Value);
        }

        [Fact]
        public void Can_combine_events()
        {
            var logs = new List<string>();

            var entity = new MsGenerated.Account
            {
                Id = Guid.NewGuid(),
                CreditLimit = new Money(200m)
            };
            entity.PropertyChanging += (sender, e) => logs.Add("Hello");

            var txEntity = new TransactionContextEntity<MsGenerated.Account>(entity);
            txEntity.AttributeChanging += (sender, e) => logs.Add("World");

            txEntity.Set(e => e.CreditLimit, new Money(450m));
            Assert.Equal("Hello World", String.Join(" ", logs));

            logs = new List<string>();
            entity.CreditLimit = new Money(450m);
            Assert.Equal("Hello World", String.Join(" ", logs));

            logs = new List<string>();
            txEntity["creditlimit"] = new Money(450m);
            Assert.Equal("World", String.Join(" ", logs));
        }

        [Fact]
        public void Can_add_and_remove_events()
        {
            var logs = new List<string>();

            var entity = new MsGenerated.Account
            {
                Id = Guid.NewGuid(),
                CreditLimit = new Money(200m)
            };
            entity.PropertyChanging += (sender, e) => logs.Add("Hello");

            AttributeChangingEventHandler attributeChanging = (sender, e) => logs.Add("World");
            var txEntity = new TransactionContextEntity<MsGenerated.Account>(entity);
            txEntity.AttributeChanging += attributeChanging;

            txEntity.Set(e => e.CreditLimit, new Money(450m));
            Assert.Equal("Hello World", String.Join(" ", logs));

            txEntity.AttributeChanging -= attributeChanging;
            logs = new List<string>();
            entity.CreditLimit = new Money(450m);
            Assert.Equal("Hello", String.Join(" ", logs));

            logs = new List<string>();
            txEntity["creditlimit"] = new Money(450m);
            Assert.Equal("", String.Join(" ", logs));
        }
    }
}
