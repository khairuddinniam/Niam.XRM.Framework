using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Insurgo.Plugins.Business
{
    public class OnCalculateTotal : OperationBase
    {
        public OnCalculateTotal(ITransactionContext<Entity> context) : 
            base(context)
        {
        }

        protected override void HandleExecute()
        {
            var isValid = Context.PluginExecutionContext.SharedVariables.ContainsKey("price-per-month-key");
            Context.TracingService.Trace("isValid: " + isValid);
            if (!isValid) return;

            var price = decimal.Parse(Context.PluginExecutionContext
                .SharedVariables["price-per-month-key"].ToString());
            var subscriptionPeriod = Get<int>("new_subscriptionperiod");
            var total = subscriptionPeriod * price;

            Set("new_total", new Money(total));
        }
    }
}
