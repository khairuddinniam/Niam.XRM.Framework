using Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EarlyBound.Example.Plugins.Business
{
    public class OnMonthlySummaryTotalPerCustomerType : OperationBase<SalesOrder>
    {
        public OnMonthlySummaryTotalPerCustomerType(ITransactionContext<SalesOrder> context) :
            base(context)
        {
        }

        protected override void HandleExecute()
        {
            if (!IsValid()) return;

            var customerTypeCode = GetCustomerTypeCode();
            if (customerTypeCode == null) return;

            var summary = GetSummary(customerTypeCode);
            var orderSummary = new new_ordersummary()
                .Set(e => e.new_period, GetOrderSummaryPeriod(customerTypeCode))
                .Set(e => e.new_qty, (int) summary.TotalQuantity)
                .Set(e => e.new_totalamount, summary.TotalAmount);
            Service.Create(orderSummary);
        }

        private bool IsValid() => Get(e => e.SubmitDate) != null && Get(e => e.AccountId) != null;

        private OptionSetValue GetCustomerTypeCode()
        {
            var columnSet = new ColumnSet<Account>(e => e.CustomerTypeCode);
            return GetRelated(e => e.AccountId, columnSet)?.Entity.Get(e => e.CustomerTypeCode);
        }

        private Summary GetSummary(OptionSetValue customerTypeCode)
        {
            var dateRange = GetSummaryDateRange();
            var details = GetSalesOrderAndDetails(dateRange, customerTypeCode);
            return details.GroupBy(x => x.Id)
                .Aggregate(new Summary(0, 0), (summary, group) =>
                {
                    var totalQuantity = group.Sum(so => 
                        so.GetAliasedEntity<SalesOrderDetail>("od").GetValue(e => e.Quantity));
                    
                    var salesOrder = group.First();
                    var totalAmount = salesOrder.Id == Id
                        ? GetValue(e => e.TotalAmount)
                        : salesOrder.GetValue(e => e.TotalAmount);
                    
                    return new Summary(summary.TotalQuantity + totalQuantity, summary.TotalAmount + totalAmount);
                });
        }

        private DateRange GetSummaryDateRange()
        {
            var submitDate = GetValue(e => e.SubmitDate);
            var from = new DateTime(submitDate.Year, submitDate.Month, 1);
            var lastDay = DateTime.DaysInMonth(submitDate.Year, submitDate.Month);
            var to = new DateTime(submitDate.Year, submitDate.Month, lastDay);
            return new DateRange(from, to);
        }

        private IList<SalesOrder> GetSalesOrderAndDetails(DateRange dateRange, OptionSetValue customerTypeCode)
        {
            var query = new QueryExpression(SalesOrder.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<SalesOrder>( e => e.Id, e => e.TotalAmount)
            };
            query.Criteria
                .AddCondition<SalesOrder>(e => e.SubmitDate, ConditionOperator.OnOrAfter, dateRange.From)
                .AddCondition<SalesOrder>(e => e.SubmitDate, ConditionOperator.OnOrBefore, dateRange.To);

            query.AddLink<SalesOrder, Account>(from => from.AccountId, to => to.Id)
                .SetAlias("ac")
                .LinkCriteria.AddCondition<Account>(e => e.CustomerTypeCode, ConditionOperator.Equal, customerTypeCode.Value);

            query.AddLink<SalesOrder, SalesOrderDetail>(from => from.SalesOrderId, to => to.SalesOrderId)
                .SetAlias("od")
                .SetColumns<SalesOrderDetail>(e => e.Quantity);

            var collection = Service.RetrieveMultiple(query);
            return collection.Entities.Select(e => e.ToEntity<SalesOrder>()).ToList();
        }

        private string GetOrderSummaryPeriod(OptionSetValue customerTypeCode)
        {
            var submitDate = GetValue(e => e.SubmitDate).ToString("yyyyMM");
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
