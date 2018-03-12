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
        public void Can_execute()
        {
            var id = Guid.NewGuid();

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
            
            var target = new Entity("lead")
            {
                Id = id,
                ["new_baseprice"] = new Money(1500m)
            };

            var testEvent = new TestEvent
            {
                Plugin = { Configure = ConfigurePlugin },
                Db = { initial, tax }
            };
            testEvent.ForUpdate(target);

            var context = testEvent.CreateTransactionContext();
            new TestOperation(context).Execute();

            Assert.Equal(1700m, context.Target.Get<Money>("new_totalprice").Value);
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
