using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using NSubstitute;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.TestFramework;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class HelperOrganizationServiceTests
    {
        [Fact]
        public void Invalid_when_delete_null_entity_reference()
        {
            EntityReference reference = null;
            var service = Substitute.For<IOrganizationService>();
            Assert.Throws<ArgumentNullException>(() => service.Delete(reference));
        }

        [Fact]
        public void Can_delete_entity_reference()
        {
            var reference = new EntityReference("entity", Guid.NewGuid());
            var service = Substitute.For<IOrganizationService>();
            service.When(svc => svc.Delete(Arg.Any<string>(), Arg.Any<Guid>()))
                .Do(ci =>
                {
                    var logicalName = ci.ArgAt<string>(0);
                    var id = ci.ArgAt<Guid>(1);
                    var deletedReference = new EntityReference(logicalName, id);
					Assert.Equal(reference, deletedReference);
                });

			service.Delete(reference);
        }

        [Fact]
        public void Can_cast_result_from_execute()
        {
            var service = Substitute.For<IOrganizationService>();
			var response = new RetrieveEntityResponse();
            service.Execute(Arg.Any<OrganizationRequest>()).Returns(response);
			
			var request = new RetrieveEntityRequest();
			Assert.Same(response, service.Execute<RetrieveEntityResponse>(request));
        }

        [Fact]
        public void Null_get_name_on_null_entity_reference()
        {
            var service = Substitute.For<IOrganizationService>();
            Assert.Null(service.GetName<xts_keytest>(null));
        }

        [Fact]
        public void Can_get_name_from_entity_reference_query()
        {
            var test = new TestHelper();
            var entity = new xts_keytest { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_key, "Hello world");
            test.Db["ENTITY"] = entity;

            var service = test.Service;
            Assert.Equal("Hello world", service.GetName<xts_keytest>(entity.ToEntityReference()));
        }

        [Fact]
        public void Can_get_name_from_entity_reference_query_using_metadata()
        {
            var test = new TestHelper();
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_string, "Hello world");
            test.Db["ENTITY"] = entity;
            
            var metadata = new EntityMetadata
            {
                LogicalName = "xts_entity"
            };
            typeof(EntityMetadata).GetProperty("PrimaryNameAttribute").SetValue(metadata, "xts_string");
            test.Service.Execute(Arg.Is<OrganizationRequest>(req => req is RetrieveEntityRequest))
                .Returns(ci =>
                {
                    var request = ci.ArgAt<RetrieveEntityRequest>(0);
                    Assert.Equal("xts_entity", request.LogicalName);
                    Assert.Equal(EntityFilters.Entity, request.EntityFilters);
                    return new RetrieveEntityResponse
                    {
                        ["EntityMetadata"] = metadata
                    };
                });

            var service = test.Service;
            Assert.Equal("Hello world", service.GetName<xts_entity>(entity.ToEntityReference()));
        }

        [Fact]
        public void Can_get_name_from_entity_reference_no_query()
        {
            var service = Substitute.For<IOrganizationService>();
            var reference = new EntityReference("xts_keytest", Guid.NewGuid())
            {
                Name = "Hello world"
            };
            Assert.Equal("Hello world", service.GetName<xts_keytest>(reference));
        }

        [Fact]
        public void Invalid_when_get_name_from_entity_reference_different_logical_name()
        {
            var service = Substitute.For<IOrganizationService>();
            var reference = new EntityReference("xts_keytest", Guid.NewGuid())
            {
                Name = "Hello world"
            };
            var ex = Assert.Throws<InvalidOperationException>(() => service.GetName<xts_entity>(reference));
            Assert.Equal("Logical name from EntityReference: 'xts_keytest' is not same as T: 'xts_entity'.", ex.Message);
        }

        [Fact]
        public void Can_get_primary_attribute()
        {
            Helper.EntityCache.Infos.TryRemove("xts_keytest", out IEntityInfo value);

            var test = new TestHelper();
            var metadata = new EntityMetadata
            {
                LogicalName = "xts_keytest"
            };
            typeof(EntityMetadata).GetProperty("PrimaryNameAttribute").SetValue(metadata, "primarynameattributekey");
            test.Service.Execute(Arg.Is<OrganizationRequest>(req => req is RetrieveEntityRequest))
                .Returns(ci =>
                {
                    var request = ci.ArgAt<RetrieveEntityRequest>(0);
                    Assert.Equal("xts_keytest", request.LogicalName);
                    Assert.Equal(EntityFilters.Entity, request.EntityFilters);
                    return new RetrieveEntityResponse
                    {
                        ["EntityMetadata"] = metadata
                    };
                });

            var service = test.Service;
            Assert.Equal("primarynameattributekey", service.GetPrimaryAttribute("xts_keytest"));
            test.Service.Received(1).Execute(Arg.Any<OrganizationRequest>());

            var info = Helper.Info<xts_keytest>();
            Helper.EntityCache.Infos.TryAdd(info.LogicalName, info); // Cache entity info.
            Assert.Equal("primarynameattributekey", service.GetPrimaryAttribute("xts_keytest"));
            test.Service.Received(1).Execute(Arg.Any<OrganizationRequest>());
        }
    }
}
