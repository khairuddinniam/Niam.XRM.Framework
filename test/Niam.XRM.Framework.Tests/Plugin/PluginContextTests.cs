using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using NSubstitute;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class PluginContextTests
    {
        [Fact]
        public void Can_get_value_from_current()
        {
            var current = new Entity("lead")
            {
                Id = Guid.NewGuid(),
                ["new_number"] = 1234,
                FormattedValues =
                {
                    ["new_number"] = "Hello world"
                }
            };

            var txContext = Substitute.For<ITransactionContext<Entity>>();
            txContext.Current.Returns(new TransactionContextEntity<Entity>(current));
            var context = new PluginContext<Entity>(txContext);
            Assert.Equal(1234, context.Get<int?>("new_number"));
            Assert.Equal("Hello world", context.GetFormattedValue("new_number"));
        }

        [Fact]
        public void Can_set_value_to_target()
        {
            var id = Guid.NewGuid();
            var target = new Entity("lead")
            {
                Id = id
            };

            var current = new Entity("lead")
            {
                Id = id
            };

            var txContext = Substitute.For<ITransactionContext<Entity>>();
            txContext.Target.Returns(new TransactionContextEntity<Entity>(target));
            txContext.Current.Returns(new TransactionContextEntity<Entity>(current));
            var context = new PluginContext<Entity>(txContext);

            context.Set("new_number", 1234);
            Assert.Equal(1234, target["new_number"]);

            var valueProvider = Substitute.For<IValueProvider>();
            valueProvider.GetValue().Returns(new OptionSetValue(1));
            context.Set("new_option", valueProvider);
            Assert.Equal(1, target.Get<OptionSetValue>("new_option").Value);

            var attributeValueProvider = Substitute.For<IAttributeValueProvider>();
            attributeValueProvider.GetValueFor(Arg.Any<string>(), Arg.Any<string>()).Returns(new Money(1500m));
            context.Set("new_money", attributeValueProvider);
            Assert.Equal(1500m, target.Get<Money>("new_money").Value);

            context.SetFormattedValue("new_referenceid", "Hello world");
            Assert.Equal("Hello world", target.GetFormattedValue("new_referenceid"));
        }
    }
}
