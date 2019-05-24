using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;
using EarlyBound.Example.Plugins.Business;
using Entities;

namespace EarlyBound.Example.Plugins
{
    public class PostSalesOrderCreate : PluginBase<SalesOrder>, IPlugin
    {
        public PostSalesOrderCreate(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override void Configure(IPluginConfiguration<SalesOrder> config)
        {
            config.ColumnSet = new ColumnSet<SalesOrder>(
                e => e.SubmitDate, e => e.TotalAmount);
        }

        protected override void ExecuteCrmPlugin(IPluginContext<SalesOrder> context)
        {
            new OnCalculateSummaryTotal(context).Execute();
            new OnMonthlySummaryTotalPerCustomerType(context).Execute();
        }
    }
}
