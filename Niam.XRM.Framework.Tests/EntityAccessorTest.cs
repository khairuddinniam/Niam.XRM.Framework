using System;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class EntityAccessorTest
    {
        [Fact]
        public void Can_get_attribute_value()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            var reference = new EntityReference("reference", Guid.NewGuid());
            entity["xts_attribute"] = reference;

            var accessor = new EntityAccessor<xts_entity>(entity);
            Assert.Equal(reference, accessor.Get<EntityReference>("xts_attribute"));
        }
        
        [Fact]
        public void Can_set_primary_field()
        {
            var id = Guid.NewGuid();
            var testEntity = new new_testentity
            {
                Id = id
            };

            var accessor = new EntityAccessor<new_testentity>(testEntity);
            Assert.Equal(id, accessor.Entity.Id);
            Assert.Equal(id, accessor.Get<Guid>("new_testentityid"));

            testEntity = new new_testentity();
            accessor = new EntityAccessor<new_testentity>(testEntity);
            accessor.Set("new_testentityid", id);
            Assert.Equal(id, accessor.Entity.Id);
            Assert.Equal(id, accessor.Get<Guid>("new_testentityid"));
        }

        [Fact]
        public void Can_set_attribute_value()
        {
            var entity = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            var accessor = new EntityAccessor<xts_entity>(entity);
            var reference = new EntityReference("reference", Guid.NewGuid());
            accessor.Set("xts_attribute", reference);

            Assert.Equal(reference, entity["xts_attribute"]);
        }

        [Fact]
        public void Can_get_formatted_value()
        {
            var entity = new xts_entity();
            entity.FormattedValues["xts_attribute"] = "formatted-value-1234";
            var accessor = new EntityAccessor<xts_entity>(entity);
            Assert.Equal("formatted-value-1234", accessor.GetFormattedValue("xts_attribute"));
            Assert.Null(entity.GetFormattedValue("xts_notexist"));
        }
        
        [Fact]
        public void Can_set_formatted_value()
        {
            var entity = new xts_entity();
            var accessor = new EntityAccessor<xts_entity>(entity);
            accessor.SetFormattedValue("xts_attribute", "formatted-value-1234");
            Assert.Equal("formatted-value-1234", entity.FormattedValues["xts_attribute"]);
        }
    }
}
