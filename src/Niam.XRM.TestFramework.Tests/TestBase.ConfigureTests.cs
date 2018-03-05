using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Xunit;

namespace Niam.XRM.TestFramework.Tests
{
    public class TestBaseConfigureTests : TestBase<xts_entity>
    {
        protected override void Configure(IPluginConfiguration<xts_entity> config)
        {
            config.ColumnSet = new ColumnSet<xts_entity>(e => e.xts_money);
        }

        [Fact]
        public void Can_filter_reference_by_plugin_config()
        {
            var reference = new xts_entity { Id = Guid.NewGuid() };
            reference.Set(e => e.xts_money, new Money(1234m));
            reference.Set(e => e.xts_int, 2300);
            Reference = reference;

            Assert.Equal(1234m, Context.Reference.Get(e => e.xts_money).Value);
            Assert.Null(Context.Reference.Get(e => e.xts_int));
        }
    }

    public class BaseTestConfigure2Test : TestBase<xts_entity>
    {
        protected override void Configure(IPluginConfiguration<xts_entity> config)
        {
            var attribute = config.Plugin.UnsecureConfig;
            config.ColumnSet = new ColumnSet(attribute);
        }

        [Fact]
        public void Can_filter_reference_by_plugin_config_unsecure_config()
        {
            Context.Plugin.UnsecureConfig.Returns("xts_int");
            var reference = new xts_entity { Id = Guid.NewGuid() };
            reference.Set(e => e.xts_money, new Money(1234m));
            reference.Set(e => e.xts_int, 2300);
            Reference = reference;

            Assert.Null(Context.Reference.Get(e => e.xts_money));
            Assert.Equal(2300, Context.Reference.Get(e => e.xts_int));
        }
    }
}
