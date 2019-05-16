using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;
using EarlyBound.Example.Plugins.Business;

namespace EarlyBound.Example.Plugins
{
    public class PreSalesOrderCreate : PluginBase<Entities.SalesOrder>, IPlugin
    {
        public PreSalesOrderCreate(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override void Configure(IPluginConfiguration<Entities.SalesOrder> config)
        {
            config.ColumnSet = new ColumnSet<Entities.SalesOrder>(
                e => e.SubmitDate, e => e.TotalAmount);
        }

        protected override void ExecuteCrmPlugin(IPluginContext<Entities.SalesOrder> context)
        {
            new OnCalculateSummaryTotal(context).Execute();
        }
    }
}
