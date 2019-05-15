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
    public class PluginConfiguration<TEntity> : IPluginConfiguration<TEntity>
        where TEntity : Entity
    {
        private readonly Lazy<LogConfig> _logConfig;

        public IContainer Container { get; }

        public IList<Func<IOrganizationService, IServiceProvider, IOrganizationService>> ServiceDecorators { get; } 
            = new List<Func<IOrganizationService, IServiceProvider, IOrganizationService>>();

        public PluginLogOption LogOption => _logConfig.Value.LogOption;

        public string LogDirPath => _logConfig.Value.LogDirPath;

        public IList<ITargetAction> TargetActions { get; } = new List<ITargetAction>(DefaultConfig.TargetActions);

        public IList<ICurrentAction> CurrentActions { get; } = new List<ICurrentAction>(DefaultConfig.CurrentActions);

        public IPluginBase Plugin { get; }

        public ColumnSet<TEntity> ColumnSet { get; set; } = new ColumnSet<TEntity>(true);

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
                    new LogConfig(PluginLogOption.Off, null);

                logConfig.Save(context.SharedVariables);
            }

            return logConfig;
        }

        private LogConfig GetLogConfigFromAssemblyAttribute(_Assembly assembly)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();
            if (TryGetAttribute<FilePluginLoggingAttribute>(assembly, out var filePluginLogAttribute))
            {
                if (Directory.Exists(filePluginLogAttribute.DirPath))
                    return new LogConfig(PluginLogOption.File, filePluginLogAttribute.DirPath);
            } else if (TryGetAttribute<CrmPluginLoggingAttribute>(assembly, out var pluginCrmLogAttribute) && ShouldLogToCrm())
            {
                return new LogConfig(PluginLogOption.Crm, null);
            }

            return null;
        }

        private static bool TryGetAttribute<T>(_Assembly assembly, out T attribute)
            where T : Attribute
        {
            attribute = (T) assembly.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            return attribute != null;
        }

        private bool ShouldLogToCrm()
        {
            var context = Container.Resolve<IPluginExecutionContext>();
            var service = Container.Resolve<IOrganizationService>();
            var organization = service.Retrieve("organization", context.OrganizationId, new ColumnSet("plugintracelogsetting"));
            var pluginTraceLogSetting = organization.Get<OptionSetValue>("plugintracelogsetting");
            var shouldLog = pluginTraceLogSetting.Equal(2); // All
            return shouldLog;
        }
                
        private class LogConfig
        {
            private const string LogOptionKey = "pc-log-option";
            private const string LogDirKey = "pc-log-dir";

            public PluginLogOption LogOption { get; }
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