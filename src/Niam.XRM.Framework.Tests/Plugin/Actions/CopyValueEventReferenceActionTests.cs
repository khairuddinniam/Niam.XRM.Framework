using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.Actions;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.Actions
{
    public class CopyValueEventReferenceActionTests
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

            var targetEntity = new Entity("entity")
            {
                Id = id,
                FormattedValues =
                {
                    ["xts_optionsetvalue"] = "Release"
                }
            };
            var target = new TransactionContextEntity<Entity>(targetEntity);

            var actionContext = new ReferenceActionContext
            {
                TransactionContext = null,
                Input = source,
                Reference = target
            };
            var action = new CopyValueEventReferenceAction();
            action.Execute(actionContext);
            source["xts_attribute"] = new Money(100m);
            Assert.Equal(100m, target.Get<Money>("xts_attribute").Value);
            Assert.Equal("$100.00", target.Entity.FormattedValues["xts_attribute"]);
            Assert.Equal("Release", target.Entity.FormattedValues["xts_optionsetvalue"]);

            source["xts_optionsetvalue"] = new OptionSetValue(12);
            Assert.Equal(12, target.Get<OptionSetValue>("xts_optionsetvalue").Value);
            Assert.Equal("$100.00", target.Entity.FormattedValues["xts_attribute"]);
            Assert.Null(target.Entity.FormattedValues["xts_optionsetvalue"]);
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

            var targetEntity = new MsGenerated.Account
            {
                Id = id,
                FormattedValues =
                {
                    ["accountratingcode"] = "Release"
                }
            };
            var target = new TransactionContextEntity<MsGenerated.Account>(targetEntity);

            var actionContext = new ReferenceActionContext
            {
                TransactionContext = null,
                Input = source,
                Reference = target
            };
            var action = new CopyValueEventReferenceAction();
            action.Execute(actionContext);
            source.Set(e => e.CreditLimit, new Money(100m));
            Assert.Equal(100m, target.Get<Money>("creditlimit").Value);
            Assert.Equal("$100.00", target.Entity.FormattedValues["creditlimit"]);
            Assert.Equal("Release", target.Entity.FormattedValues["accountratingcode"]);

            source.Entity.AccountRatingCode = new OptionSetValue(12);
            Assert.Equal(12, target.Get<OptionSetValue>("accountratingcode").Value);
            Assert.Equal("$100.00", target.Entity.FormattedValues["creditlimit"]);
            Assert.Null(target.Entity.FormattedValues["accountratingcode"]);
        }
    }
}
