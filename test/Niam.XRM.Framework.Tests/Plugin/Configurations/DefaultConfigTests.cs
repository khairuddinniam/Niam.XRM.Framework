using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin.Configurations;
using Niam.XRM.Framework.Plugin.ServiceProviders;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.Configurations
{
    public class DefaultConfigTests
    {
        [Fact]
        public void Can_configure_service_factory()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Is(typeof(IOrganizationServiceFactory))).Returns(Substitute.For<IOrganizationServiceFactory>());
            var container = new Container(serviceProvider);
            var pluginConfig = Substitute.For<IPluginConfiguration<xts_entity>>();
            pluginConfig.Container.Returns(container);
            pluginConfig.ServiceDecorators.Returns(new List<Func<IOrganizationService, IServiceProvider, IOrganizationService>>());

            DefaultConfig.PluginConfigureServiceFactory(pluginConfig);

            Assert.IsType<ServiceFactory>(container.GetService<IOrganizationServiceFactory>());
        }

        [Fact]
        public void Can_configure_logging_plugin_off()
        {
            var tracingService = Substitute.For<ITracingService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Is(typeof(ITracingService))).Returns(tracingService);
            var container = new Container(serviceProvider);
            var pluginConfig = Substitute.For<IPluginConfiguration<xts_entity>>();
            pluginConfig.Container.Returns(container);
            pluginConfig.ServiceDecorators.Returns(new List<Func<IOrganizationService, IServiceProvider, IOrganizationService>>());
            pluginConfig.LogOption.Returns(PluginLogOption.Off);

            DefaultConfig.PluginConfigureLogging(pluginConfig);

            Assert.Same(tracingService, container.GetService<ITracingService>());
            var actualDecoratedService = pluginConfig.ServiceDecorators[0].Invoke(Substitute.For<IOrganizationService>(), container);
            Assert.IsType<ToEntityService>(actualDecoratedService);
        }

        [Fact]
        public void Can_configure_logging_plugin_crm()
        {
            var tracingService = Substitute.For<ITracingService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Is(typeof(ITracingService))).Returns(tracingService);
            var container = new Container(serviceProvider);
            var pluginConfig = Substitute.For<IPluginConfiguration<xts_entity>>();
            pluginConfig.Container.Returns(container);
            pluginConfig.ServiceDecorators.Returns(new List<Func<IOrganizationService, IServiceProvider, IOrganizationService>>());
            pluginConfig.LogOption.Returns(PluginLogOption.Crm);

            DefaultConfig.PluginConfigureLogging(pluginConfig);

            Assert.Same(tracingService, container.GetService<ITracingService>());
            Assert.NotEmpty(pluginConfig.ServiceDecorators);
            var actualDecoratedService = pluginConfig.ServiceDecorators[1].Invoke(Substitute.For<IOrganizationService>(), container);
            Assert.IsType<LogOrganizationService>(actualDecoratedService);
        }

        [Fact]
        public void Can_configure_logging_plugin_file()
        {
            var tracingService = Substitute.For<ITracingService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Is(typeof(ITracingService))).Returns(tracingService);
            serviceProvider.GetService(Arg.Is(typeof(IPluginExecutionContext))).Returns(Substitute.For<IPluginExecutionContext>());
            var container = new Container(serviceProvider);
            var pluginConfig = Substitute.For<IPluginConfiguration<xts_entity>>();
            pluginConfig.Container.Returns(container);
            pluginConfig.ServiceDecorators.Returns(new List<Func<IOrganizationService, IServiceProvider, IOrganizationService>>());
            pluginConfig.LogOption.Returns(PluginLogOption.File);
            pluginConfig.LogDirPath.Returns("log-dir");

            DefaultConfig.PluginConfigureLogging(pluginConfig);

            var actualTracingService = container.GetService<ITracingService>();
            Assert.NotSame(tracingService, actualTracingService);
            Assert.IsType<FilePluginTracingService>(actualTracingService);
            Assert.NotEmpty(pluginConfig.ServiceDecorators);
            var actualDecoratedService = pluginConfig.ServiceDecorators[1].Invoke(Substitute.For<IOrganizationService>(), container);
            Assert.IsType<LogOrganizationService>(actualDecoratedService);
        }
    }
}
