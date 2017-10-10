using System;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using NSubstitute;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin.Metadata;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.Metadata
{
    public class MetadataExtTest
    {
        [Fact]
        public void Can_get_entity_metadata()
        {
            var entityMetadata = new EntityMetadata();
            RetrieveEntityRequest request = null;
            var context = Substitute.For<ITransactionContext<Entity>>();
            context.Service.Execute(Arg.Any<RetrieveEntityRequest>())
                .Returns(ci =>
                {
                    request = ci.ArgAt<RetrieveEntityRequest>(0);
                    var response = new RetrieveEntityResponse
                    {
                        ["EntityMetadata"] = entityMetadata
                    };
                    return response;
                });

            var retrievedEntityMetadata = context.GetMetadata("xts_entity");
            Assert.Same(entityMetadata, retrievedEntityMetadata);
            Assert.Equal("xts_entity", request.LogicalName);
            Assert.Equal(EntityFilters.Default, request.EntityFilters);
            Assert.False(request.RetrieveAsIfPublished);

            context.GetMetadata("xts_entity", EntityFilters.Attributes);
            Assert.Equal(EntityFilters.Attributes, request.EntityFilters);
        }

        [Fact]
        public void Can_get_entity_metadata_generic()
        {
            var entityMetadata = new EntityMetadata();
            RetrieveEntityRequest request = null;
            var context = Substitute.For<ITransactionContext<Entity>>();
            context.Service.Execute(Arg.Any<RetrieveEntityRequest>())
                .Returns(ci =>
                {
                    request = ci.ArgAt<RetrieveEntityRequest>(0);
                    var response = new RetrieveEntityResponse
                    {
                        ["EntityMetadata"] = entityMetadata
                    };
                    return response;
                });

            var retrievedEntityMetadata = context.GetMetadata<xts_relatedentity>();
            Assert.Same(entityMetadata, retrievedEntityMetadata);
            Assert.Equal("xts_relatedentity", request.LogicalName);
            Assert.Equal(EntityFilters.Default, request.EntityFilters);
            Assert.False(request.RetrieveAsIfPublished);

            context.GetMetadata<xts_relatedentity>(EntityFilters.Attributes);
            Assert.Equal(EntityFilters.Attributes, request.EntityFilters);
        }

        [Fact]
        public void Can_get_attribute_metadata()
        {
            var attributeMetadata = new AttributeMetadata();
            RetrieveAttributeRequest request = null;
            var context = Substitute.For<ITransactionContext<Entity>>();
            context.Service.Execute(Arg.Any<RetrieveAttributeRequest>())
                .Returns(ci =>
                {
                    request = ci.ArgAt<RetrieveAttributeRequest>(0);
                    var response = new RetrieveAttributeResponse
                    {
                        ["AttributeMetadata"] = attributeMetadata
                    };
                    return response;
                });

            var retrievedAttributeMetadata = context.GetMetadata("xts_entity", "xts_attribute");
            Assert.Same(attributeMetadata, retrievedAttributeMetadata);
            Assert.Equal("xts_entity", request.EntityLogicalName);
            Assert.Equal("xts_attribute", request.LogicalName);
            Assert.False(request.RetrieveAsIfPublished);
        }

        [Fact]
        public void Can_get_attribute_metadata_generic()
        {
            var attributeMetadata = new AttributeMetadata();
            RetrieveAttributeRequest request = null;
            var context = Substitute.For<ITransactionContext<Entity>>();
            context.Service.Execute(Arg.Any<RetrieveAttributeRequest>())
                .Returns(ci =>
                {
                    request = ci.ArgAt<RetrieveAttributeRequest>(0);
                    var response = new RetrieveAttributeResponse
                    {
                        ["AttributeMetadata"] = attributeMetadata
                    };
                    return response;
                });

            var retrievedAttributeMetadata = context.GetMetadata<xts_entity>(e => e.xts_attribute);
            Assert.Same(attributeMetadata, retrievedAttributeMetadata);
            Assert.Equal("xts_entity", request.EntityLogicalName);
            Assert.Equal("xts_attribute", request.LogicalName);
            Assert.False(request.RetrieveAsIfPublished);
        }

        [Fact]
        public void Can_get_attribute_metadata_generic_with_spesific_expression()
        {
            var attributeMetadata = new AttributeMetadata();
            RetrieveAttributeRequest request = null;
            var context = Substitute.For<ITransactionContext<Entity>>();
            context.Service.Execute(Arg.Any<RetrieveAttributeRequest>())
                .Returns(ci =>
                {
                    request = ci.ArgAt<RetrieveAttributeRequest>(0);
                    var response = new RetrieveAttributeResponse
                    {
                        ["AttributeMetadata"] = attributeMetadata
                    };
                    return response;
                });

            Expression<Func<xts_entity, EntityReference>> attribute = e => e.xts_attribute;
            var retrievedAttributeMetadata = context.GetMetadata(attribute);
            Assert.Same(attributeMetadata, retrievedAttributeMetadata);
            Assert.Equal("xts_entity", request.EntityLogicalName);
            Assert.Equal("xts_attribute", request.LogicalName);
            Assert.False(request.RetrieveAsIfPublished);
        }
    }
}
