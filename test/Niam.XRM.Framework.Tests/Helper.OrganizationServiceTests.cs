using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Data;
using NSubstitute;
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
        public void Null_get_name_on_null_entity_reference()
        {
            var service = Substitute.For<IOrganizationService>();
            Assert.Null(service.GetReferenceName<xts_keytest>(null));
        }

        [Fact]
        public void Can_get_name_from_entity_reference_query()
        {
            var entity = new xts_keytest { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_key, "Hello world");

            var service = Substitute.For<IOrganizationService>();
            service.Retrieve(Arg.Is<string>(name => name == "xts_keytest"), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(entity);
            
            Assert.Equal("Hello world", service.GetReferenceName<xts_keytest>(entity.ToEntityReference()));
        }

        [Fact]
        public void Can_get_name_from_entity_reference_query_using_metadata()
        {
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_string, "Hello world");

            var service = Substitute.For<IOrganizationService>();
            service.Retrieve(Arg.Is<string>(name => name == "xts_entity"), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(entity);

            var metadata = new EntityMetadata
            {
                LogicalName = "xts_entity"
            };
            typeof(EntityMetadata).GetProperty("PrimaryNameAttribute").SetValue(metadata, "xts_string");
            service.Execute(Arg.Is<OrganizationRequest>(req => req is RetrieveEntityRequest))
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
            
            Assert.Equal("Hello world", service.GetReferenceName<xts_entity>(entity.ToEntityReference()));
        }

        [Fact]
        public void Can_get_name_from_entity_reference_no_query()
        {
            var service = Substitute.For<IOrganizationService>();
            var reference = new EntityReference("xts_keytest", Guid.NewGuid())
            {
                Name = "Hello world"
            };
            Assert.Equal("Hello world", service.GetReferenceName<xts_keytest>(reference));
        }

        [Fact]
        public void Invalid_when_get_name_from_entity_reference_different_logical_name()
        {
            var service = Substitute.For<IOrganizationService>();
            var reference = new EntityReference("xts_keytest", Guid.NewGuid())
            {
                Name = "Hello world"
            };
            var ex = Assert.Throws<InvalidOperationException>(() => service.GetReferenceName<xts_entity>(reference));
            Assert.Equal("Logical name from EntityReference: 'xts_keytest' is not same as T: 'xts_entity'.", ex.Message);
        }

        [Fact]
        public void Can_get_primary_attribute()
        {
            Helper.EntityCache.Clear();
            
            var metadata = new EntityMetadata
            {
                LogicalName = "xts_keytest"
            };
            typeof(EntityMetadata).GetProperty("PrimaryNameAttribute").SetValue(metadata, "primarynameattributekey");

            var service = Substitute.For<IOrganizationService>();
            service.Execute(Arg.Is<OrganizationRequest>(req => req is RetrieveEntityRequest))
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
            
            Assert.Equal("primarynameattributekey", service.GetPrimaryAttribute("xts_keytest"));
            service.Received(1).Execute(Arg.Any<OrganizationRequest>());
            
            Helper.EntityCache.GetOrAddInfo(typeof(xts_keytest)); // Cache entity info.
            Assert.Equal("primarynameattributekey", service.GetPrimaryAttribute("xts_keytest"));
            service.Received(1).Execute(Arg.Any<OrganizationRequest>());
        }

        [Fact]
        public void Can_retrieve_multiple_fetch_xml()
        {
            var service = Substitute.For<IOrganizationService>();
            string fetchXml = null;
            service.RetrieveMultiple(Arg.Any<QueryBase>()).Returns(ci =>
            {
                fetchXml = ci.ArgAt<FetchExpression>(0).Query;
                return new EntityCollection();
            });

            service.RetrieveMultiple("this-is-fetch-xml");
            Assert.Equal("this-is-fetch-xml", fetchXml);
        }
        
        [Fact]
        public void Can_retrieve_single_entity_strong_type_all_columns()
        {
            var service = Substitute.For<IOrganizationService>();
            string entityName = null;
            Guid? entityId = null;
            ColumnSet columnSet = null;
            service.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>()).Returns(ci =>
            {
                entityName = ci.ArgAt<string>(0);
                entityId = ci.ArgAt<Guid>(1);
                columnSet = ci.ArgAt<ColumnSet>(2);
                return new Entity(entityName, entityId.Value);
            });
            
            var id = Guid.NewGuid();
            service.Retrieve<Organization>(id);
            Assert.Equal(Organization.EntityLogicalName, entityName);
            Assert.Equal(id, entityId);
            Assert.True(columnSet.AllColumns);
        }

        [Fact]
        public void Can_retrieve_single_entity_strong_type_with_column_set()
        {
            var service = Substitute.For<IOrganizationService>();
            string entityName = null;
            Guid? entityId = null;
            ColumnSet columnSet = null;
            service.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>()).Returns(ci =>
            {
                entityName = ci.ArgAt<string>(0);
                entityId = ci.ArgAt<Guid>(1);
                columnSet = ci.ArgAt<ColumnSet>(2);
                return new Entity(entityName, entityId.Value);
            });
            
            var id = Guid.NewGuid();
            service.Retrieve(id, new ColumnSet<Organization>(e => e.Name));
            Assert.Equal(Organization.EntityLogicalName, entityName);
            Assert.Equal(id, entityId);
            Assert.Equal(new [] { "name" }, columnSet.Columns);
        }
        
        [Fact]
        public void Can_retrieve_single_entity_strong_type_with_attributes()
        {
            var service = Substitute.For<IOrganizationService>();
            string entityName = null;
            Guid? entityId = null;
            ColumnSet columnSet = null;
            service.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>()).Returns(ci =>
            {
                entityName = ci.ArgAt<string>(0);
                entityId = ci.ArgAt<Guid>(1);
                columnSet = ci.ArgAt<ColumnSet>(2);
                return new Entity(entityName, entityId.Value);
            });
            
            var id = Guid.NewGuid();
            service.Retrieve<Organization>(id, e => e.Name, e => e.CreatedOn);
            Assert.Equal(Organization.EntityLogicalName, entityName);
            Assert.Equal(id, entityId);
            Assert.Equal(new [] { "name", "createdon" }, columnSet.Columns);
        }
    }
}
