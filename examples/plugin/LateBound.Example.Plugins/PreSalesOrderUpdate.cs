using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;
using LateBound.Example.Plugins.Business;
using Niam.XRM.Framework;

namespace LateBound.Example.Plugins
{
    public class PreSalesOrderUpdate: PluginBase, IPlugin
    {
        public PreSalesOrderUpdate(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override void Configure(IPluginConfiguration<Entity> config)
        {
            config.ColumnSet = new ColumnSet("submitdate", "totalamount");
        }

        protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
        {
            if (context.Current.ContainsAny("submitdate", "totalamount"))
            {
                new OnCalculateSummaryTotal(context).Execute();
            }
        }
    }
}
