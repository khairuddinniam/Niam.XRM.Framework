using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.TestFramework;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class FormattedValueReferenceAccessorTest
    {
        [Fact]
        public void Can_get_existing_formatted_value()
        {
            var test = new TestHelper();
            var input = new Entity();
            var reference = new Entity
            {
                ["xts_optionsetvalue"] = new OptionSetValue(123),
                FormattedValues =
                {
                    ["xts_optionsetvalue"] = "Release"
                }
            };
            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(test.Service);

            var accessor = new FormattedValueReferenceAccessor<Entity>(input, reference, context);
            Assert.Equal("Release", accessor.GetFormattedValue("xts_optionsetvalue"));
            test.Service.DidNotReceive().Execute(Arg.Any<OrganizationRequest>());
        }

        [Fact]
        public void Can_get_empty_formatted_value()
        {
            var test = new TestHelper();
            var input = new Entity();
            var reference = new Entity();
            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(test.Service);

            var accessor = new FormattedValueReferenceAccessor<Entity>(input, reference, context);
            Assert.Null(accessor.GetFormattedValue("xts_optionsetvalue"));
            test.Service.DidNotReceive().Execute(Arg.Any<OrganizationRequest>());
        }

        [Fact]
        public void Can_get_option_set_formatted_value()
        {
            var metadata = new PicklistAttributeMetadata
            {
                OptionSet = new OptionSetMetadata
                {
                    Options =
                    {
                        new OptionMetadata
                        {
                            Value = 111,
                            Label = new Label
                            {
                                UserLocalizedLabel = new LocalizedLabel("Cancel", 1033)
                            }
                        },
                        new OptionMetadata
                        {
                            Value = 123,
                            Label = new Label
                            {
                                UserLocalizedLabel = new LocalizedLabel("Release", 1033)
                            }
                        }
                    }
                }
            };
            var test = new TestHelper();
            test.Service.Execute(Arg.Is<OrganizationRequest>(req => req is RetrieveAttributeRequest))
                .Returns(ci =>
                {
                    var request = ci.ArgAt<RetrieveAttributeRequest>(0);
                    Assert.Equal("entity", request.EntityLogicalName);
                    Assert.Equal("xts_optionsetvalue", request.LogicalName);
                    return new RetrieveAttributeResponse
                    {
                        ["AttributeMetadata"] = metadata
                    };
                });
            var input = new Entity("entity");
            var reference = new Entity("entity")
            {
                ["xts_optionsetvalue"] = new OptionSetValue(123)
            };
            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(test.Service);

            var accessor = new FormattedValueReferenceAccessor<Entity>(input, reference, context);
            Assert.Equal("Release", accessor.GetFormattedValue("xts_optionsetvalue"));
            Assert.Equal("Release", input.GetFormattedValue("xts_optionsetvalue"));
            test.Service.Received(1).Execute(Arg.Any<OrganizationRequest>());
            // Retrieve again, should not get from server again.
            Assert.Equal("Release", accessor.GetFormattedValue("xts_optionsetvalue"));
            test.Service.Received(1).Execute(Arg.Any<OrganizationRequest>());
        }

        [Fact]
        public void Can_get_entity_reference_formatted_value()
        {
            var metadata = new EntityMetadata
            {
                LogicalName = "xts_related"
            };
            typeof(EntityMetadata).GetProperty("PrimaryNameAttribute").SetValue(metadata, "xts_name");

            var test = new TestHelper();
            test.Service.Execute(Arg.Is<OrganizationRequest>(req => req is RetrieveEntityRequest))
                .Returns(ci =>
                {
                    var request = ci.ArgAt<RetrieveEntityRequest>(0);
                    Assert.Equal("xts_related", request.LogicalName);
                    Assert.Equal(EntityFilters.Entity, request.EntityFilters);
                    return new RetrieveEntityResponse
                    {
                        ["EntityMetadata"] = metadata
                    };
                });

            var related = new Entity("xts_related", Guid.NewGuid())
            {
                ["xts_name"] = "Related Name"
            };
            test.Db["RELATED-001"] = related;

            var input = new Entity("entity");
            var reference = new Entity("entity")
            {
                ["xts_referenceid"] = related.ToEntityReference()
            };
            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(test.Service);

            var accessor = new FormattedValueReferenceAccessor<Entity>(input, reference, context);
            Assert.Equal("Related Name", accessor.GetFormattedValue("xts_referenceid"));
            test.Service.Received(1).Execute(Arg.Any<OrganizationRequest>());
            test.Service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());

            // Retrieve again, should not get from server again.
            Assert.Equal("Related Name", accessor.GetFormattedValue("xts_referenceid"));
            test.Service.Received(1).Execute(Arg.Any<OrganizationRequest>());
            test.Service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
        }

        [Fact]
        public void Skip_retrieve_on_not_supported_formatted_value()
        {
            var test = new TestHelper();
            var input = new Entity("entity");
            var reference = new Entity("entity")
            {
                ["xts_int"] = 234
            };
            var context = Substitute.For<ITransactionContextBase>();
            context.Service.Returns(test.Service);

            var accessor = new FormattedValueReferenceAccessor<Entity>(input, reference, context);
            Assert.Null(accessor.GetFormattedValue("xts_int"));
            Assert.False(accessor.Entity.FormattedValues.Contains("xts_int"));
            test.Service.DidNotReceive().Execute(Arg.Any<OrganizationRequest>());
        }
    }
}
