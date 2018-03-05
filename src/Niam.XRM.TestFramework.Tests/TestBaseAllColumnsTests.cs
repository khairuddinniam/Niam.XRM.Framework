using System;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Xunit;

namespace Niam.XRM.TestFramework.Tests
{
    public class TestBaseAllColumnsTests : TestBase<xts_entity>
    {
        protected override void Configure(IPluginConfiguration<xts_entity> config)
        {
            config.ColumnSet = new ColumnSet(true);
        }
        
        [Fact]
        public void Can_get_value_with_column_set_all_columns()
        {
            var reference = new xts_entity
            {
                Id = Guid.NewGuid()
            };
            reference.Set(e => e.xts_int, 1234);
            Reference = reference;

            Assert.Equal(1234, Context.Reference.Get(e => e.xts_int));
        }
    }
}
