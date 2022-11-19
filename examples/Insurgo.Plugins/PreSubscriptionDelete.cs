using Insurgo.Plugins.Business;
using Insurgo.Plugins.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;

namespace Insurgo.Plugins
{
    public class PreSubscriptionDelete : PluginBase<cr953_subscription>, IPlugin
    {
        public PreSubscriptionDelete(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override void Configure(IPluginConfiguration<cr953_subscription> config)
        {
            config.ColumnSet = new ColumnSet(true);
        }

        protected override void ExecuteCrmPlugin(IPluginContext<cr953_subscription> context)
        {
            new CalculatePackageSummariesOnSubscriptionChange(context).Execute();
        }
    }
}