using System;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class CustomGeneratedEntitiesTests
    {
        [Fact]
        public void Can_call_to_entity()
        {
            var entity = new Entity("xts_entity") { Id = Guid.NewGuid() };
            var xtsEntity = entity.ToEntity<xts_entity>();
            Assert.Equal(entity.ToEntityReference(), xtsEntity.ToEntityReference());
            var xtsDerivedEntity = entity.ToEntity<xts_derivedentity>();
            Assert.Equal(xtsEntity.ToEntityReference(), xtsDerivedEntity.ToEntityReference());
        }
    }
}
