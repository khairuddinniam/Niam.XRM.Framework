using System;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Infrastructure;
using Niam.XRM.Framework.Interfaces.Plugin;
using Xunit;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework.Tests.Infrastructure
{
    public class InstanceWrapperTest
    {
        [Fact]
        public void Can_create_entity_wrapper()
        {
            var service = Substitute.For<IOrganizationService>();
            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(service);

            var entity = new Entity("xts_nongenericentity") { Id = Guid.NewGuid() };
            var xtsEntity = new xts_entity { Id = Guid.NewGuid() };

            InstanceEntityWrapper<Entity, TestWrapper>.Create(entity, context);
            InstanceEntityWrapper<xts_entity, TestGenericWrapper<xts_entity>>.Create(xtsEntity, context);
            InstanceEntityWrapper<xts_entity, EntityWrapper<xts_entity>>.Create(xtsEntity, context);
            InstanceEntityWrapper<Entity, TestWrapper>.Create(xtsEntity, context);

            var accessor = new EntityAccessor<Entity>(entity);
            InstanceEntityWrapper<Entity, TestWrapper>.Create(accessor, context);

            var xtsAccessor = new EntityAccessor<xts_entity>(xtsEntity);
            InstanceEntityWrapper<xts_entity, TestGenericWrapper<xts_entity>>.Create(xtsAccessor, context);
            InstanceEntityWrapper<xts_entity, EntityWrapper<xts_entity>>.Create(xtsAccessor, context);
            InstanceEntityWrapper<Entity, TestWrapper>.Create(xtsAccessor, context);
        }

        [Fact]
        public void Can_convert_entity()
        {
            var entity = new Entity("new_testentity")
            {
                Id = Guid.NewGuid()
            };
            var testEntity = entity.ToEntity<new_testentity>();
            Assert.Equal(entity.ToEntityReference(), testEntity.ToEntityReference());
        }

        private class TestWrapper : EntityWrapper<Entity>
        {
            public TestWrapper(Entity entity, ITransactionContextBase context) 
                : base(entity, context)
            {
            }

            public TestWrapper(IEntityAccessor<Entity> accessor, ITransactionContextBase context)
                : base(accessor, context)
            {
            }
        }

        private class TestGenericWrapper<T> : EntityWrapper<T> where T : xts_entity, new()
        {
            public TestGenericWrapper(T entity, ITransactionContextBase context)
                : base(entity, context)
            {
            }

            public TestGenericWrapper(IEntityAccessor<T> accessor, ITransactionContextBase context)
                : base(accessor, context)
            {
            }
        }
    }
}
