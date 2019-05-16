using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System;
using System.Linq;

namespace LateBound.Example.Plugins.Business
{
    public class OnCalculateSummaryTotal : OperationBase
    {
        public OnCalculateSummaryTotal(ITransactionContext<Entity> context) :
            base(context)
        {
        }

        protected override void HandleExecute()
        {
            var period = GetPeriod();

            var orderSummary = GetOrderSummary(period);

            var totalAmount = GetTotalAmountCurrent();
            var orderTotalAmount = orderSummary.Get<Money>("new_totalamount").GetValueOrDefault() + totalAmount;

            orderSummary.Set("new_totalamount", new Money(orderTotalAmount));

            if (orderSummary.Id == Guid.Empty)
            {
                orderSummary.Set("new_period", period);
                Service.Create(orderSummary);
                return;
            }

            Service.Update(orderSummary);
        }

        private decimal GetTotalAmountCurrent()
        {
            if (Context.PluginExecutionContext.MessageName == "Delete")
            {
                return Get<Money>("totalamount").GetValueOrDefault() * -1;
            }

            var totalAmount = (Context.PluginExecutionContext.MessageName == "Update"
                ? Initial.Get<Money>("totalamount").GetValueOrDefault()
                : 0m);
            return Get<Money>("totalamount").GetValueOrDefault() - totalAmount;
        }

        private Entity GetOrderSummary(string period)
        {
            var query = new QueryExpression("new_ordersummary")
            {
                ColumnSet = new ColumnSet("new_period", "new_totalamount"),
                TopCount = 1
            };
            query.Criteria.AddCondition("new_period", ConditionOperator.Equal, period);
            var result = Service.RetrieveMultiple(query);

            return result.Entities.Any() ? result.Entities[0] : new Entity("new_ordersummary");
        }

        private string GetPeriod()
        {
            var submitDate = Get<DateTime?>("submitdate").GetValueOrDefault();

            return submitDate.ToString("yyyyMM");
        }
    }
}
