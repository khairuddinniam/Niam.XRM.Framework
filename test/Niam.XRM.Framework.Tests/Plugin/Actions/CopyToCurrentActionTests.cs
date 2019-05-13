using System;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.Actions;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.Actions
{
    public class CopyToCurrentActionTests
    {
        [Theory]
        [InlineData((int) SdkMessageProcessingStepStage.Prevalidation)]
        [InlineData((int) SdkMessageProcessingStepStage.Preoperation)]
        [InlineData((int) SdkMessageProcessingStepStage.Postoperation)]
        public void Can_copy_attribute_values(int stage)
        {
            var id = Guid.NewGuid();
            var reference = new EntityReference("entity", Guid.NewGuid()) { Name = "name" };
            var optionSet = new OptionSetValue(1);
            var money = new Money(100m);
            var number = 123;

            var source = new Entity
            {
                ["attr_1"] = id,
                ["attr_2"] = reference,
                ["attr_3"] = optionSet,
                ["attr_4"] = money,
                ["attr_5"] = number,
                FormattedValues =
                {
                    ["attr_3"] = "Release",
                    ["attr_4"] = "Rp100.000,45"
                }
            };
            var txSource = new TransactionContextEntity<Entity>(source);

            var target = new Entity
            {
                ["attr_1"] = Guid.NewGuid(),
                ["attr_2"] = new EntityReference("entity", Guid.NewGuid()) {Name = "another-name"},
                ["attr_3"] = new OptionSetValue(2),
                ["attr_4"] = new Money(125m),
                ["attr_5"] = 345,
                ["attr_6"] = new Money(700m)
            };
            var txTarget = new TransactionContextEntity<Entity>(target);

            var context = Substitute.For<ICurrentActionContext>();
            context.TransactionContext.PluginExecutionContext.Stage.Returns(stage);
            context.Target.Returns(txSource);
            context.Current.Returns(txTarget);
            var action = new CopyToCurrentAction();
            action.Execute(context);

            Assert.Equal(6, txTarget.Attributes.Count);
            Assert.Equal(id, txTarget["attr_1"]);

            Assert.NotSame(reference, txTarget["attr_2"]);
            Assert.Equal(reference, txTarget["attr_2"]);
            Assert.Equal(reference.Name, ((EntityReference) txTarget["attr_2"]).Name);

            Assert.NotSame(optionSet, txTarget["attr_3"]);
            Assert.Equal(optionSet, txTarget["attr_3"]);

            Assert.NotSame(money, txTarget["attr_4"]);
            Assert.Equal(money, txTarget["attr_4"]);

            Assert.Equal(number, txTarget["attr_5"]);
            Assert.Equal(700m, ((Money) txTarget["attr_6"]).Value);

            Assert.Equal("Release", target.FormattedValues["attr_3"]);
            Assert.Equal("Rp100.000,45", target.FormattedValues["attr_4"]);
        }
    }
}
