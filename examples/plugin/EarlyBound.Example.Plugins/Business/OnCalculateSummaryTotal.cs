using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System;
using System.Linq;
using Entities;

namespace EarlyBound.Example.Plugins.Business
{
    public class OnCalculateSummaryTotal : OperationBase<SalesOrder>
    {
        public OnCalculateSummaryTotal(ITransactionContext<SalesOrder> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var period = GetPeriod();

            var orderSummary = GetOrderSummary(period);

            var totalAmount = GetTotalAmountCurrent();
            var orderTotalAmount = orderSummary.GetValue(e => e.new_totalamount) + totalAmount;

            orderSummary.Set(e => e.new_totalamount, orderTotalAmount);

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
                return GetValue(e => e.TotalAmount) * -1;
            }

            var totalAmount = (Context.PluginExecutionContext.MessageName == "Update"
                ? Initial.Get(e => e.TotalAmount).GetValueOrDefault()
                : 0m);
            return GetValue(e => e.TotalAmount) - totalAmount;
        }

        private Entities.new_ordersummary GetOrderSummary(string period)
        {
            var query = new QueryExpression(Entities.new_ordersummary.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<Entities.new_ordersummary>(e => e.new_period,
                    e => e.new_totalamount),
                TopCount = 1
            };
            query.Criteria.AddCondition("new_period", ConditionOperator.Equal, period);
            var result = Service.RetrieveMultiple(query);

            return result.Entities.Any() ? result.Entities[0].ToEntity<Entities.new_ordersummary>() :
                new Entities.new_ordersummary();
        }

        private string GetPeriod()
        {
            var submitDate = GetValue(e => e.SubmitDate);

            return submitDate.ToString("yyyyMM");
        }
    }
}
