using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework;
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

        [Fact]
        public void Can_execute_custom_early_bound()
        {
            var id = Guid.NewGuid();

            var tax = new new_tax
            {
                Id = Guid.NewGuid()
            };
            tax.Set(e => e.new_taxamount, 200m);

            var initial = new new_customentity
            {
                Id = id
            };
            initial.Set(e => e.new_baseprice, 100m);
            initial.Set(e => e.new_taxid, tax.ToEntityReference());

            var target = new new_customentity
            {
                Id = id
            };
            initial.Set(e => e.new_baseprice, 1500m);

            var testEvent = new TestEvent<new_customentity>
            {
                Plugin = { Configure = EarlyBoundConfigurePlugin },
                Db = { initial, tax }
            };
            testEvent.ForUpdate(target);

            var context = testEvent.CreateTransactionContext();
            new EarlyBoundTestOperation(context).Execute();

            Assert.Equal(1700m, context.Target.Get(e => e.new_totalprice).Value);
        }

        private void EarlyBoundConfigurePlugin(IPluginConfiguration<new_customentity> config)
        {
            config.ColumnSet = new ColumnSet<new_customentity>(e => e.new_baseprice, e => e.new_taxid);
        }

        private class EarlyBoundTestOperation : OperationBase<new_customentity>
        {
            public EarlyBoundTestOperation(ITransactionContext<new_customentity> context) : base(context)
            {
            }

            protected override void HandleExecute()
            {
                var basePrice = Get(e => e.new_baseprice);
                var tax = GetRelated(e => e.new_taxid, new ColumnSet<new_tax>(e => e.new_taxamount));
                var taxAmount = tax.Get(e => e.new_taxamount);

                Set(e => e.new_totalprice, new Money(basePrice.Value + taxAmount.Value));
            }
        }
    }
}
