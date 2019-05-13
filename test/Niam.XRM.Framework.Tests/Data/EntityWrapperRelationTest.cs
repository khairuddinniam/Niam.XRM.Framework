using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using NSubstitute;
using Xunit;

namespace Niam.XRM.Framework.Tests.Data
{
    internal static class EntityWrapperRelationExtensions
    {
        public static IEntityWrapper<xts_relatedentity> GetReference(
            this IEntityWrapperRelation<xts_entity> relation, IColumnSet<xts_relatedentity> columnSet)
        {
            return relation.GetRelated(e => e.xts_referenceid, columnSet);
        }

        public static IEntityWrapper<xts_derivedentity> GetProduct(
            this IEntityWrapperRelation<xts_relatedentity> relation, IColumnSet<xts_derivedentity> columnSet)
        {
            return relation.GetRelated(e => e.xts_relatedid, columnSet);
        }
    }

    public class EntityWrapperRelationTest
    {
        [Fact]
        public void Can_use_extensions()
        {
            var derived = new xts_derivedentity { Id = Guid.NewGuid() };
            derived.Set(e => e.xts_string, "Hello world");

            var related = new xts_relatedentity { Id = Guid.NewGuid() };
            related.Set(e => e.xts_relatedid, derived.ToEntityReference());
            
            var service = Substitute.For<IOrganizationService>();
            service.Retrieve(Arg.Any<string>(), Arg.Is<Guid>(id => id == derived.Id), Arg.Any<ColumnSet>())
                .Returns(derived);
            service.Retrieve(Arg.Any<string>(), Arg.Is<Guid>(id => id == related.Id), Arg.Any<ColumnSet>())
                .Returns(related);

            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_referenceid, related.ToEntityReference());
            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(service);
            var wrapper = new EntityWrapper<xts_entity>(entity, context);

            var text = wrapper
                .GetReference(new ColumnSet<xts_relatedentity>(e => e.xts_relatedid))
                .GetProduct(new ColumnSet<xts_derivedentity>(e => e.xts_string))
                .Get(e => e.xts_string);
            Assert.Equal("Hello world", text);
        }
    }
}
