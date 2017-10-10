using System;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.ServiceProviders;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class PluginBaseTest
    {
        [Fact]
        public void Can_register_new_service()
        {
            var crmServiceProvider = Substitute.For<IServiceProvider>();
            var crmServiceFactory = Substitute.For<IOrganizationServiceFactory>();
            var crmTracing = Substitute.For<ITracingService>();
            crmServiceProvider.GetService(Arg.Is(typeof(IOrganizationServiceFactory))).Returns(crmServiceFactory);
            crmServiceProvider.GetService(Arg.Is(typeof(ITracingService))).Returns(crmTracing);
            var plugin = new DerivedPluginCore("HELLO", "WORLD");
            plugin.Execute(crmServiceProvider);

            Assert.Equal("HELLO", plugin.UnsecureConfig);
            Assert.Equal("WORLD", plugin.SecureConfig);
        }

        public class DerivedPluginCore : PluginBase
        {
            public DerivedPluginCore(string unsecure, string secure) : base(unsecure, secure)
            {
            }

            public IPluginConfiguration<Entity> PluginConfiguration { get; private set; }

            protected override void ExecuteCrmPlugin(ITransactionContext<Entity> context)
            {
                var serviceProvider = PluginConfiguration.Container.Resolve<IServiceProvider>();
                Assert.IsType<Container>(serviceProvider);
                Assert.IsType<ServiceFactory>(serviceProvider.GetService(typeof(IOrganizationServiceFactory)));

                Assert.Same(this, PluginConfiguration.TransactionContext.Plugin);
                Assert.Same(this, context.Plugin);
            }

            protected override void Configure(IPluginConfiguration<Entity> config)
            {
                base.Configure(config);
                PluginConfiguration = config;
            }
        }
    }
}
