using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;
using Niam.XRM.Framework.Data;

namespace Niam.XRM.Framework.Plugin.Configurations
{
    public class PluginConfiguration<T> : IPluginConfiguration<T>
        where T : Entity
    {
        private readonly Lazy<LogConfig> _logConfig;

        public IContainer Container { get; }

        public IList<Func<IOrganizationService, IServiceProvider, IOrganizationService>> ServiceDecorators { get; } 
            = new List<Func<IOrganizationService, IServiceProvider, IOrganizationService>>();

        public PluginLogOption LogOption => _logConfig.Value.LogOption;

        public string LogDirPath => _logConfig.Value.LogDirPath;

        public IList<IInputAction> InputActions { get; } = new List<IInputAction>(DefaultConfig.InputActions);

        public IList<IReferenceAction> ReferenceActions { get; } = new List<IReferenceAction>(DefaultConfig.ReferenceActions);

        public IPluginBase Plugin { get; }

        public ColumnSet<T> ColumnSet { get; set; } = new ColumnSet<T>();

        public PluginConfiguration(_Assembly assembly = null)
        {
            _logConfig = new Lazy<LogConfig>(() => GetLogConfig(assembly));
        }

        public PluginConfiguration(IPluginBase plugin, IContainer container, _Assembly assembly = null)
            : this(assembly)
        {
            Plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        private LogConfig GetLogConfig(_Assembly assembly)
        {
            var context = Container.Resolve<IPluginExecutionContext>();
            if (!LogConfig.TryParse(context.SharedVariables, out var logConfig))
            {
                logConfig =
                    GetLogConfigFromAssemblyAttribute(assembly) ??
                    GetLogConfigFromOrganization(context) ??
                    new LogConfig(PluginLogOption.Off, null);

                logConfig.Save(context.SharedVariables);
            }

            return logConfig;
        }

        private static LogConfig GetLogConfigFromAssemblyAttribute(_Assembly assembly)
        {
            var filePluginLogAttribute = (FilePluginLoggingAttribute) (assembly ?? Assembly.GetExecutingAssembly())
                .GetCustomAttributes(typeof(FilePluginLoggingAttribute), false)
                .FirstOrDefault();
            if (filePluginLogAttribute != null && Directory.Exists(filePluginLogAttribute.DirPath))
                return new LogConfig(PluginLogOption.File, filePluginLogAttribute.DirPath);

            return null;
        }

        private LogConfig GetLogConfigFromOrganization(IPluginExecutionContext context)
        {
            var service = Container.Resolve<IOrganizationService>();
            var organization = service.Retrieve("organization", context.OrganizationId, new ColumnSet("plugintracelogsetting"));
            var pluginTraceLogSetting = organization.Get<OptionSetValue>("plugintracelogsetting");
            var shouldLog = pluginTraceLogSetting.Equal(2); // All
            return shouldLog ? new LogConfig(PluginLogOption.Crm, null) : null;
        }
        
        private class LogConfig
        {
            private const string LogOptionKey = "pc-log-option";
            private const string LogDirKey = "pc-log-dir";

            public PluginLogOption LogOption { get; } = PluginLogOption.Off;
            public string LogDirPath { get; }

            public LogConfig(PluginLogOption logOption, string logDirPath)
            {
                LogOption = logOption;
                LogDirPath = logDirPath;
            }

            public void Save(ParameterCollection sharedVariables)
            {
                sharedVariables[LogOptionKey] = (int) LogOption;
                sharedVariables[LogDirKey] = LogDirPath;
            }

            public static bool TryParse(ParameterCollection sharedVariables, out LogConfig logConfig)
            {
                if (sharedVariables.TryGetValue(LogOptionKey, out var logOption))
                {
                    var logOptionEnum = (PluginLogOption) logOption;
                    var logDirPath = sharedVariables.TryGetValue(LogDirKey, out var logDir) ? (string) logDir : null;
                    logConfig = new LogConfig(logOptionEnum, logDirPath);

                    return true;
                }

                logConfig = null;
                return false;
            }
        }
    }
}