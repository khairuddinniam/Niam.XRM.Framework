using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin.Configurations;
using Niam.XRM.Framework.Plugin.ServiceProviders;

namespace Niam.XRM.Framework.Plugin
{
    public abstract class PluginBase : PluginBase<Entity>
    {
        protected PluginBase(string unsecure, string secure) 
            : base(unsecure, secure)
        {
        }
    }

    public abstract class PluginBase<T> : PluginCoreBase
        where T : Entity
    {
        protected PluginBase(string unsecure, string secure) 
            : base(unsecure, secure)
        {
        }

        protected override void ExecuteCrmPlugin(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            var contextConfig = new TransactionContextConfiguration<T>(this);
            var container = new Container(serviceProvider);
            var pluginConfig = new PluginConfiguration<T>(container, contextConfig);
            PluginConfigure(pluginConfig);
            var context = container.ToTransactionContext<T>(contextConfig);
            try
            {
                ExecuteCrmPlugin(context);
            }
            finally
            {
                container.Dispose();
            }
        }

        private void PluginConfigure(IPluginConfiguration<T> config)
        {
            PrePluginConfigure(config);
            Configure(config);
            // Preserved for future use. 
            //PostPluginConfigure(config);
        }

        private void PrePluginConfigure(IPluginConfiguration<T> config)
        {
            DefaultConfig.PluginConfigureServiceFactory(config);
            DefaultConfig.PluginConfigureLogging(config);
        }

        protected virtual void Configure(IPluginConfiguration<T> config)
        {
        }
        
        protected abstract void ExecuteCrmPlugin(ITransactionContext<T> context);
    }
}
