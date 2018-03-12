using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.Actions;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.Actions
{
    public class CopyValueEventCurrentActionTests
    {
        [Fact]
        public void Can_copy_attribute_value()
        {
            var id = Guid.NewGuid();
            var sourceEntity = new Entity("entity")
            {
                Id = id,
                FormattedValues =
                {
                    ["xts_attribute"] = "$100.00"
                }
            };
            var source = new TransactionContextEntity<Entity>(sourceEntity);

            var currentEntity = new Entity("entity")
            {
                Id = id,
                FormattedValues =
                {
                    ["xts_optionsetvalue"] = "Release"
                }
            };
            var current = new TransactionContextEntity<Entity>(currentEntity);

            var actionContext = new CurrentActionContext
            {
                TransactionContext = null,
                Target = source,
                Current = current
            };
            var action = new CopyValueEventCurrentAction();
            action.Execute(actionContext);
            source["xts_attribute"] = new Money(100m);
            Assert.Equal(100m, current.Get<Money>("xts_attribute").Value);
            Assert.Equal("$100.00", current.Entity.FormattedValues["xts_attribute"]);
            Assert.Equal("Release", current.Entity.FormattedValues["xts_optionsetvalue"]);

            source["xts_optionsetvalue"] = new OptionSetValue(12);
            Assert.Equal(12, current.Get<OptionSetValue>("xts_optionsetvalue").Value);
            Assert.Equal("$100.00", current.Entity.FormattedValues["xts_attribute"]);
            Assert.Null(current.Entity.FormattedValues["xts_optionsetvalue"]);
        }

        [Fact]
        public void Can_copy_attribute_value_from_ms_generated_entity()
        {
            var id = Guid.NewGuid();
            var sourceEntity = new MsGenerated.Account
            {
                Id = id,
                FormattedValues =
                {
                    ["creditlimit"] = "$100.00"
                }
            };
            var source = new TransactionContextEntity<MsGenerated.Account>(sourceEntity);

            var currentEntity = new MsGenerated.Account
            {
                Id = id,
                FormattedValues =
                {
                    ["accountratingcode"] = "Release"
                }
            };
            var current = new TransactionContextEntity<MsGenerated.Account>(currentEntity);

            var actionContext = new CurrentActionContext
            {
                TransactionContext = null,
                Target = source,
                Current = current
            };
            var action = new CopyValueEventCurrentAction();
            action.Execute(actionContext);
            source.Set(e => e.CreditLimit, new Money(100m));
            Assert.Equal(100m, current.Get<Money>("creditlimit").Value);
            Assert.Equal("$100.00", current.Entity.FormattedValues["creditlimit"]);
            Assert.Equal("Release", current.Entity.FormattedValues["accountratingcode"]);

            source.Entity.AccountRatingCode = new OptionSetValue(12);
            Assert.Equal(12, current.Get<OptionSetValue>("accountratingcode").Value);
            Assert.Equal("$100.00", current.Entity.FormattedValues["creditlimit"]);
            Assert.Null(current.Entity.FormattedValues["accountratingcode"]);
        }
    }
}
