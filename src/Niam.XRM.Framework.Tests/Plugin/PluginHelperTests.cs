using System;
using NSubstitute;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class PluginHelperTests
    {
        [Fact]
        public void Can_create_entity_transaction_context()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            Assert.NotNull(serviceProvider.ToTransactionContext());
        }

        [Fact]
        public void Can_create_transaction_context()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            Assert.NotNull(serviceProvider.ToTransactionContext<xts_entity>());
        }
    }
}
