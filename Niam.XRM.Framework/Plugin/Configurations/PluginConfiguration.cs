using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders;

namespace Niam.XRM.Framework.Plugin.Configurations
{
    public class PluginConfiguration<T> : IPluginConfiguration<T>
        where T : Entity
    {
        private bool? _enableLogging;

        public IContainer Container { get; }
        public IList<Func<IOrganizationService, IServiceProvider, IOrganizationService>> ServiceDecorators { get; } 
            = new List<Func<IOrganizationService, IServiceProvider, IOrganizationService>>();
        public ITransactionContextConfiguration<T> TransactionContext { get; }

        public bool EnableLogging
        {
            get
            {
                if (!_enableLogging.HasValue)
                    _enableLogging = GetEnableLogging();

                return _enableLogging.Value;
            }
            set => _enableLogging = value;
        }

        public PluginConfiguration(IContainer container, ITransactionContextConfiguration<T> context)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            TransactionContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        private bool GetEnableLogging()
        {
            var context = Container.Resolve<IPluginExecutionContext>();
            const string key = "plugin-config-org";
            var organization = context.GetShared<Entity>(key) ?? GetOrganizationFromDb(context.OrganizationId);
            context.SharedVariables[key] = organization;

            var pluginTraceLogSetting = organization.Get<OptionSetValue>("plugintracelogsetting");
            return pluginTraceLogSetting.Equal(2); // All
        }
        
        private Entity GetOrganizationFromDb(Guid organizationId)
        {
            var service = Container.Resolve<IOrganizationService>();
            return service.Retrieve("organization", organizationId, new ColumnSet("plugintracelogsetting"));
        }
    }
}