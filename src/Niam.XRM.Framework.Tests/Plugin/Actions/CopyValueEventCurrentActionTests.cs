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
                    ["attribute"] = "$100.00"
                }
            };
            var source = new TransactionContextEntity<Entity>(sourceEntity);

            var currentEntity = new Entity("entity")
            {
                Id = id,
                FormattedValues =
                {
                    ["optionsetvalue"] = "Release"
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
            source["attribute"] = new Money(100m);
            Assert.Equal(100m, current.Get<Money>("attribute").Value);
            Assert.Equal("$100.00", current.Entity.FormattedValues["attribute"]);
            Assert.Equal("Release", current.Entity.FormattedValues["optionsetvalue"]);

            source["optionsetvalue"] = new OptionSetValue(12);
            Assert.Equal(12, current.Get<OptionSetValue>("optionsetvalue").Value);
            Assert.Equal("$100.00", current.Entity.FormattedValues["attribute"]);
            Assert.Null(current.Entity.FormattedValues["optionsetvalue"]);

            source.SetFormattedValue("new_format", "Formatted-Value");
            Assert.Equal("Formatted-Value", current.Entity.FormattedValues["new_format"]);
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
