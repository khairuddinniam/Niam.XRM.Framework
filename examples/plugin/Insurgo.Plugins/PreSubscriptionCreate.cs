using Insurgo.Plugins.Business;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;

namespace Insurgo.Plugins
{
    public class PreSubscriptionCreate : PluginBase, IPlugin
    {
        public PreSubscriptionCreate(string unsecure, string secure) : 
            base(unsecure, secure)
        {
        }

        protected override void Configure(IPluginConfiguration<Entity> config)
        {
            config.ColumnSet = new ColumnSet(true);
        }

        protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
        {
            //new OnCalculateTotal(context).Execute();
        }
    }
}
