using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.Configurations;
using NSubstitute;
using Xunit;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Tests.Plugin.Configurations
{
    public class PluginConfigurationTests
    {
        [Theory]
        [InlineData(PluginLogOption.Off)]
        [InlineData(PluginLogOption.Crm)]
        [InlineData(PluginLogOption.File)]
        public void Get_enable_logging_from_shared_variables(PluginLogOption logOption)
        {
            var context = Substitute.For<IPluginExecutionContext>();
            context.SharedVariables.Returns(new ParameterCollection());

            var container = Substitute.For<IContainer>();
            container.Resolve<IPluginExecutionContext>().Returns(context);
            context.SharedVariables["pc-log-option"] = (int) logOption;
            var txConfig = Substitute.For<ITransactionContextConfiguration<xts_entity>>();
            var plugin = Substitute.For<IPluginBase>();
            var config = new PluginConfiguration<xts_entity>(plugin, container);
            Assert.Equal(logOption, config.LogOption);
        }

        [Fact]
        public void Get_enable_logging_from_assembly_attribute_file()
        {
            const string dirName = "log-folder";
            Directory.CreateDirectory(dirName);

            var assembly = Substitute.For<_Assembly>();
            assembly.GetCustomAttributes(Arg.Is<Type>(t => t == typeof(FilePluginLoggingAttribute)), Arg.Any<bool>())
                .Returns(new object[] { new FilePluginLoggingAttribute(dirName) });

            var context = Substitute.For<IPluginExecutionContext>();
            context.SharedVariables.Returns(new ParameterCollection());

            var container = Substitute.For<IContainer>();
            container.Resolve<IPluginExecutionContext>().Returns(context);
            var txConfig = Substitute.For<ITransactionContextConfiguration<xts_entity>>();
            
            var plugin = Substitute.For<IPluginBase>();
            var config = new PluginConfiguration<xts_entity>(plugin, container, assembly);
            Assert.Equal(PluginLogOption.File, config.LogOption);
            Directory.Delete(dirName);
            Assert.Equal(PluginLogOption.File, config.LogOption);
            Assert.Equal((int) PluginLogOption.File, context.SharedVariables["pc-log-option"]);
            Assert.Equal(dirName, context.SharedVariables["pc-log-dir"]);
            Assert.Equal(dirName, config.LogDirPath);
        }

        [Theory]
        [InlineData(Organization.Options.PluginTraceLogSetting.Off, PluginLogOption.Off)]
        [InlineData(Organization.Options.PluginTraceLogSetting.Exception, PluginLogOption.Off)]
        [InlineData(Organization.Options.PluginTraceLogSetting.All, PluginLogOption.Crm)]
        public void Get_enable_logging_from_assembly_attribute_trace_log_setting(
            Organization.Options.PluginTraceLogSetting setting, PluginLogOption logOption)
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

            var assembly = Substitute.For<_Assembly>();
            assembly.GetCustomAttributes(Arg.Is<Type>(t => t == typeof(PluginCrmLoggingAttribute)), Arg.Any<bool>()).Returns(new object[] { new PluginCrmLoggingAttribute() });
            
            var plugin = Substitute.For<IPluginBase>();
            var pluginConfig = new PluginConfiguration<xts_entity>(plugin, container, assembly);
            service.DidNotReceive().Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            Assert.Equal(logOption, pluginConfig.LogOption);
            service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            Assert.Equal(logOption, pluginConfig.LogOption);
            service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            Assert.Null(pluginConfig.LogDirPath);

            var anotherPluginConfig = new PluginConfiguration<xts_entity>(plugin, container);
            Assert.Equal(logOption, anotherPluginConfig.LogOption);
            service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            Assert.Equal(logOption, anotherPluginConfig.LogOption);
            service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());

            Assert.Equal((int) logOption, context.SharedVariables["pc-log-option"]);
            Assert.Null(context.SharedVariables["pc-log-dir"]);
            Assert.Null(anotherPluginConfig.LogDirPath);
        }

        [Fact]
        public void Disable_log()
        {
            var assembly = Substitute.For<_Assembly>();
            assembly.GetCustomAttributes(Arg.Any<Type>(), Arg.Any<bool>()).Returns(new object[0]);
            var plugin = Substitute.For<IPluginBase>();

            var context = Substitute.For<IPluginExecutionContext>();
            context.SharedVariables.Returns(new ParameterCollection());
            var container = Substitute.For<IContainer>();
            container.Resolve<IPluginExecutionContext>().Returns(context);
            var pluginConfig = new PluginConfiguration<xts_entity>(plugin, container, assembly);

            Assert.Equal(PluginLogOption.Off, pluginConfig.LogOption);
        }
    }
}
