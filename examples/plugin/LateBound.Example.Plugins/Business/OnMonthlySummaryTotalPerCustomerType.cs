using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LateBound.Example.Plugins.Business
{
    public class OnMonthlySummaryTotalPerCustomerType : OperationBase
    {
        public OnMonthlySummaryTotalPerCustomerType(ITransactionContext<Entity> context) :
            base(context)
        {
        }

        protected override void HandleExecute()
        {
            if (!IsValid()) return;

            var customerTypeCode = GetCustomerTypeCode();
            if (customerTypeCode == null) return;

            CreateOrUpdateOrderSummary(customerTypeCode);
        }

        private void CreateOrUpdateOrderSummary(OptionSetValue customerTypeCode)
        {
            var summary = GetSummary(customerTypeCode);
            var period = GetOrderSummaryPeriod(customerTypeCode);

            var orderSummary = GetOrderSummary(period)
                .Set("new_period", period)
                .Set("new_qty", (int)summary.TotalQuantity)
                .Set("new_totalamount", new Money(summary.TotalAmount));

            if (orderSummary.Id != Guid.Empty)
                Service.Update(orderSummary);
            else
                Service.Create(orderSummary);
        }

        private Entity GetOrderSummary(string period)
        {
            var query = new QueryExpression("new_ordersummary")
            {
                ColumnSet = new ColumnSet(false),
                TopCount = 1
            };
            query.Criteria.AddCondition("new_period", ConditionOperator.Equal, period);

            var result = Service.RetrieveMultiple(query);

            return result.Entities.FirstOrDefault() ?? new Entity("new_ordersummary");
        }

        private bool IsValid() => Get<DateTime?>("submitdate") != null && Get<EntityReference>("accountid") != null;

        private OptionSetValue GetCustomerTypeCode()
        {
            var columnSet = new ColumnSet("customertypecode");
            return GetRelated("accountid", columnSet)?.Entity.Get<OptionSetValue>("customertypecode");
        }

        private Summary GetSummary(OptionSetValue customerTypeCode)
        {
            var dateRange = GetSummaryDateRange();
            var details = GetSalesOrderAndDetails(dateRange, customerTypeCode);
            return details.GroupBy(x => x.Id)
                .Aggregate(new Summary(0, 0), (summary, group) =>
                {
                    var totalQuantity = group.Sum(so =>
                        so.GetAliasedEntity<Entity>("od").Get<int?>("quantity").GetValueOrDefault());

                    var salesOrder = group.First();
                    var totalAmount = salesOrder.Id == Id
                        ? Get<Money>("totalamount").GetValueOrDefault()
                        : salesOrder.Get<Money>("totalamount").GetValueOrDefault();

                    return new Summary(summary.TotalQuantity + totalQuantity, summary.TotalAmount + totalAmount);
                });
        }

        private DateRange GetSummaryDateRange()
        {
            var submitDate = Get<DateTime?>("submitdate").GetValueOrDefault();
            var from = new DateTime(submitDate.Year, submitDate.Month, 1);
            var lastDay = DateTime.DaysInMonth(submitDate.Year, submitDate.Month);
            var to = new DateTime(submitDate.Year, submitDate.Month, lastDay);
            return new DateRange(from, to);
        }

        private IList<Entity> GetSalesOrderAndDetails(DateRange dateRange, OptionSetValue customerTypeCode)
        {
            var query = new QueryExpression("salesorder")
            {
                ColumnSet = new ColumnSet("totalamount")
            };
            query.Criteria.AddCondition("submitdate", ConditionOperator.Between, dateRange.From, dateRange.To);

            if (Context.PluginExecutionContext.MessageName == "Delete")
            {
                query.Criteria.AddCondition("salesorderid", ConditionOperator.NotEqual, Wrapper.Id);
            }

            var accountLink = query.AddLink("account", "accountid", "accountid");
            accountLink.EntityAlias = "ac";
            accountLink.LinkCriteria.AddCondition("customertypecode", ConditionOperator.Equal, customerTypeCode.Value);

            var salesOrderDetailLink = query.AddLink("salesorderdetail", "salesorderid", "salesorderid");
            salesOrderDetailLink.EntityAlias = "od";
            salesOrderDetailLink.Columns = new ColumnSet("quantity");

            var collection = Service.RetrieveMultiple(query);
            return collection.Entities.Select(e => e).ToList();
        }

        private string GetOrderSummaryPeriod(OptionSetValue customerTypeCode)
        {
            var submitDate = Get<DateTime?>("submitdate").GetValueOrDefault().ToString("yyyyMM");
            var customerTypeCodeValue = customerTypeCode.Value.ToString("00");
            return $"{submitDate}{customerTypeCodeValue}";
        }

        private class Summary
        {
            public decimal TotalQuantity { get; }

            public decimal TotalAmount { get; }

            public Summary(decimal totalQuantity, decimal totalAmount)
            {
                TotalQuantity = totalQuantity;
                TotalAmount = totalAmount;
            }
        }
    }
}
