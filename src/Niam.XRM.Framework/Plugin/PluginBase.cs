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

    public abstract class PluginBase<TE> : PluginCoreBase
        where TE : Entity
    {
        protected PluginBase(string unsecure, string secure) 
            : base(unsecure, secure)
        {
        }

        protected override void ExecuteCrmPlugin(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            var container = new Container(serviceProvider);
            var config = new PluginConfiguration<TE>(this, container);
            PluginConfigure(config);
            var context = container.ToTransactionContext<TE>(config);
            var pluginContext = new PluginContext<TE>(context);
            try
            {
                ExecuteCrmPlugin(pluginContext);
            }
            finally
            {
                container.Dispose();
            }
        }

        private void PluginConfigure(IPluginConfiguration<TE> config)
        {
            PrePluginConfigure(config);
            Configure(config);
            // Preserved for future use. 
            //PostPluginConfigure(config);
        }

        private void PrePluginConfigure(IPluginConfiguration<TE> config)
        {
            DefaultConfig.PluginConfigureServiceFactory(config);
            DefaultConfig.PluginConfigureLogging(config);
        }

        protected virtual void Configure(IPluginConfiguration<TE> config)
        {
        }
        
        protected abstract void ExecuteCrmPlugin(IPluginContext<TE> context);
    }
}
