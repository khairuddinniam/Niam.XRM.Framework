using System;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Plugin.ServiceProviders;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.ServiceProviders
{
    public class ContainerTest
    {
        [Fact]
        public void Can_get_service()
        {
            var crmContext = Substitute.For<IPluginExecutionContext>();
            var crmServiceProvider = Substitute.For<IServiceProvider>();
            crmServiceProvider.GetService(Arg.Any<Type>()).Returns(crmContext);
            var serviceCollection = new Container(crmServiceProvider);
            Assert.Same(crmContext, serviceCollection.Resolve<IPluginExecutionContext>());

            var manualRegisteredContext = Substitute.For<IPluginExecutionContext>();
            serviceCollection.Register(manualRegisteredContext);
            Assert.Same(manualRegisteredContext, serviceCollection.Resolve<IPluginExecutionContext>());

            var updatedManualRegisteredContext = Substitute.For<IPluginExecutionContext>();
            serviceCollection.Register(updatedManualRegisteredContext);
            Assert.Same(updatedManualRegisteredContext, serviceCollection.Resolve<IPluginExecutionContext>());
        }

        [Fact]
        public void Can_register_service_using_factory()
        {
            var crmContext = Substitute.For<IPluginExecutionContext>();
            var crmServiceProvider = Substitute.For<IServiceProvider>();
            crmServiceProvider.GetService(Arg.Any<Type>()).Returns(crmContext);
            var container = new Container(crmServiceProvider);
            Assert.Same(crmContext, container.Resolve<IPluginExecutionContext>());

            var manualRegisteredContext = Substitute.For<IPluginExecutionContext>();
            var previousContext = container.Resolve<IPluginExecutionContext>();
            container.Register(c => manualRegisteredContext);
            Assert.Same(manualRegisteredContext, container.Resolve<IPluginExecutionContext>());
            Assert.Same(crmContext, previousContext);

            var updatedManualRegisteredContext = Substitute.For<IPluginExecutionContext>();
            var lastPreviousContext = container.Resolve<IPluginExecutionContext>();
            container.Register(c => updatedManualRegisteredContext);
            Assert.Same(updatedManualRegisteredContext, container.Resolve<IPluginExecutionContext>());
            Assert.Same(manualRegisteredContext, lastPreviousContext);
        }
    }
}
