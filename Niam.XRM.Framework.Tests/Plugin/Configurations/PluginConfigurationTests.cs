using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders;
using Niam.XRM.Framework.Plugin.Configurations;
using NSubstitute;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.Configurations
{
    public class PluginConfigurationTests
    {
        [Theory]
        [InlineData(Organization.Options.PluginTraceLogSetting.Off, false)]
        [InlineData(Organization.Options.PluginTraceLogSetting.Exception, false)]
        [InlineData(Organization.Options.PluginTraceLogSetting.All, true)]
        public void Get_enable_logging_from_trace_log_setting(
            Organization.Options.PluginTraceLogSetting setting, bool enableLogging)
        {
            var organization = new Organization { Id = Guid.NewGuid() };
            organization.Set(e => e.PluginTraceLogSetting, setting);
            var service = Substitute.For<IOrganizationService>();
            service.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>()).Returns(organization);
            var context = Substitute.For<IPluginExecutionContext>();
            context.SharedVariables.Returns(new ParameterCollection());
            context.OrganizationId.Returns(organization.Id);

            var container = Substitute.For<IContainer>();
            container.Resolve<IOrganizationService>().Returns(service);
            container.Resolve<IPluginExecutionContext>().Returns(context);
            var txConfig = Substitute.For<ITransactionContextConfiguration<xts_entity>>();

            var pluginConfig = new PluginConfiguration<xts_entity>(container, txConfig);
            service.DidNotReceive().Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            Assert.Equal(enableLogging, pluginConfig.EnableLogging);
            service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            Assert.Equal(enableLogging, pluginConfig.EnableLogging);
            service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());

            var anotherPluginConfig = new PluginConfiguration<xts_entity>(container, txConfig);
            Assert.Equal(enableLogging, anotherPluginConfig.EnableLogging);
            service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            Assert.Equal(enableLogging, anotherPluginConfig.EnableLogging);
            service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
        }
    }
}
