using System;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.Actions;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.Actions
{
    public class UpdateFormattedValueEventInputActionTest
    {
        [Fact]
        public void Update_formatted_value_when_set_attribute_value()
        {
            var input = new Entity
            {
                Id = Guid.NewGuid(),
                ["xts_optionsetvalue"] = new OptionSetValue(1234),
                ["xts_int"] = 5678,
                FormattedValues =
                {
                    ["xts_optionsetvalue"] = "Release"
                }
            };

            var txEntity = new TransactionContextEntity<Entity>(input);
            var context = Substitute.For<IInputActionContext>();
            context.Input.Returns(txEntity);
            var action = new UpdateFormattedValueEventInputAction();
            action.Execute(context);

            txEntity.Set("xts_optionsetvalue", new OptionSetValue(2));
            Assert.False(input.FormattedValues.Contains("xts_optionsetvalue"));
            txEntity.Set("xts_int", 4556);
            Assert.False(input.FormattedValues.Contains("xts_int"));

            txEntity.Set("xts_entityreference", (EntityReference) null);
            Assert.False(input.FormattedValues.Contains("xts_entityreference"));
            txEntity.Set("xts_entityreference", new EntityReference { Name = "lookup" });
            Assert.Equal("lookup", input.FormattedValues["xts_entityreference"]);
            txEntity.Set("xts_entityreference", new EntityReference());
            Assert.False(input.FormattedValues.Contains("xts_entityreference"));
        }
    }
}
