using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.TestFramework.Tests
{
    public class TestCrudEventTests
    {
        [Fact]
        public void Validate_failed_on_target_is_null()
        {
            var testEvent = new TestCrudEvent
            {
                Target = null
            };

            var ex = Assert.Throws<TestException>(() => testEvent.CreateTransactionContext());
            Assert.Equal("Target property must not null.", ex.Message);
        }

        [Theory]
        [InlineData("Create", 40)]
        [InlineData("Update", 10)]
        [InlineData("Update", 20)]
        [InlineData("Update", 40)]
        [InlineData("Delete", 10)]
        [InlineData("Delete", 20)]
        public void Validate_failed_on_initial_is_null(string messageName, int stage)
        {
            var testEvent = new TestCrudEvent
            {
                Initial = null
            };
            testEvent.PluginExecutionContext.MessageName = messageName;
            testEvent.PluginExecutionContext.Stage = stage;

            var ex = Assert.Throws<TestException>(() => testEvent.CreateTransactionContext());
            Assert.Equal("Initial property must not null.", ex.Message);
        }

        [Theory]
        [InlineData("Create", 40)]
        [InlineData("Update", 10)]
        [InlineData("Update", 20)]
        [InlineData("Update", 40)]
        [InlineData("Delete", 10)]
        [InlineData("Delete", 20)]
        public void Validate_failed_on_target_and_initial_are_not_same(string messageName, int stage)
        {
            var testEvent = new TestCrudEvent
            {
                Initial = new Entity("lead", Guid.NewGuid())
            };
            testEvent.PluginExecutionContext.MessageName = messageName;
            testEvent.PluginExecutionContext.Stage = stage;

            var ex = Assert.Throws<TestException>(() => testEvent.CreateTransactionContext());
            Assert.Equal("Target entity name and id is not same as Initial entity name and id.", ex.Message);
        }

        [Fact]
        public void Can_execute()
        {
            var id = Guid.NewGuid();
            var target = new Entity("lead")
            {
                Id = id,
                ["new_baseprice"] = new Money(1500m)
            };

            var tax = new Entity("tax")
            {
                Id = Guid.NewGuid(),
                ["new_taxamount"] = new Money(200m)
            };

            var initial = new Entity("lead")
            {
                Id = id,
                ["new_baseprice"] = new Money(100m),
                ["new_taxid"] = tax.ToEntityReference()
            };

            // Default is "Update" and stage "20" (Pre)
            var testEvent = new TestCrudEvent
            {
                Plugin = { Configure = ConfigurePlugin },
                Target = target,
                Initial = initial,
                Db = { tax }
            };

            var context = testEvent.CreateTransactionContext();
            new TestOperation(context).Execute();

            Assert.Equal(1700m, context.Input.Get<Money>("new_totalprice").Value);
        }

        private void ConfigurePlugin(IPluginConfiguration<Entity> config)
        {
            config.ColumnSet = new ColumnSet("new_baseprice", "new_taxid");
        }

        private class TestOperation : OperationBase
        {
            public TestOperation(ITransactionContext<Entity> context) : base(context)
            {
            }

            protected override void HandleExecute()
            {
                var basePrice = Get<Money>("new_baseprice");
                var tax = GetRelated("new_taxid", new ColumnSet("new_taxamount"));
                var taxAmount = tax.Get<Money>("new_taxamount");

                Set("new_totalprice", new Money(basePrice.Value + taxAmount.Value));
            }
        }
    }
}
