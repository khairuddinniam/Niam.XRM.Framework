using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;
using LateBound.Example.Plugins.Business;

namespace LateBound.Example.Plugins
{
    public class PreSalesOrderCreate : PluginBase, IPlugin
    {
        public PreSalesOrderCreate(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override void Configure(IPluginConfiguration<Entity> config)
        {
            config.ColumnSet = new ColumnSet("submitdate", "totalamount");
        }

        protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
        {
            new OnCalculateSummaryTotal(context).Execute();
        }
    }
}
