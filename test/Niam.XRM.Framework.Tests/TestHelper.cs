using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Plugin;
using NSubstitute;

namespace Niam.XRM.Framework.Tests
{
    internal class TestHelper
    {
        public IOrganizationService Service { get; } = Substitute.For<IOrganizationService>();
        public IPluginExecutionContext PluginExecutionContext { get; }
        public ITracingService TracingService { get; } = Substitute.For<ITracingService>();
        public IServiceProvider ServiceProvider { get; }

        public TestHelper()
        {
            PluginExecutionContext = CreatePluginExecutionContext();
            ServiceProvider = CreateServiceProvider();
        }
        

        private static IPluginExecutionContext CreatePluginExecutionContext()
        {
            var context = Substitute.For<IPluginExecutionContext>();
            context.MessageName.Returns(PluginMessage.Create);
            context.Stage.Returns((int) SdkMessageProcessingStepStage.Preoperation);
            context.SharedVariables.Returns(new ParameterCollection());
            context.InputParameters.Returns(new ParameterCollection());
            context.PreEntityImages.Returns(new EntityImageCollection());
            context.PostEntityImages.Returns(new EntityImageCollection());
            return context;
        }

        private IServiceProvider CreateServiceProvider()
        {
            var serviceFactory = Substitute.For<IOrganizationServiceFactory>();
            serviceFactory.CreateOrganizationService(Arg.Any<Guid?>()).Returns(Service);

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Is(typeof(IPluginExecutionContext))).Returns(PluginExecutionContext);
            serviceProvider.GetService(Arg.Is(typeof(IOrganizationServiceFactory))).Returns(serviceFactory);
            serviceProvider.GetService(Arg.Is(typeof(ITracingService))).Returns(TracingService);

            return serviceProvider;
        }
    }
}
