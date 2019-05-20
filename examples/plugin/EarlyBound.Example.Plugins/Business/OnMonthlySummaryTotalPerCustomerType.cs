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
            var submitDate = Get(e => e.SubmitDate);
            var accountRef = Get(e => e.AccountId);

            var valid = submitDate != null && accountRef != null;
            if (!valid) return;

            var customer = GetCustomer();
            var customerTypeCode = customer.Get(e => e.CustomerTypeCode);
            if (customerTypeCode == null) return;

            var data = GetData(customerTypeCode.Value);

            var totalAmount = data.GroupBy(so => so.Id)
                .Sum(grp =>
                {
                    var so = grp.FirstOrDefault();
                    if (so == null) return 0;
                    return so.Id == Wrapper.Id ? GetValue(e => e.TotalAmount) : so.GetValue(e => e.TotalAmount);
                });

            var qty = data.Sum(so =>
            {
                var soDetail = so.GetAliasedEntity<SalesOrderDetail>("od");
                return (int)soDetail.GetValue(e => e.Quantity);
            });

            var period = submitDate.GetValueOrDefault().ToString("yyyyMM") + customerTypeCode.Value.ToString("00");

            var orderSummary = new new_ordersummary();
            orderSummary.Set(e => e.new_period, period);
            orderSummary.Set(e => e.new_qty, qty);
            orderSummary.Set(e => e.new_totalamount, totalAmount);

            Service.Create(orderSummary);
        }

        private Account GetCustomer()
        {
            var account = GetRelated(e => e.AccountId, new ColumnSet<Account>(e => e.CustomerTypeCode));
            return account.Entity;
        }

        private SalesOrder[] GetData(int customerTypeCode)
        {
            var dates = GetDates().ToArray();
            var query = new QueryExpression(SalesOrder.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<SalesOrder>(e => e.SubmitDate, e => e.TotalAmount)
            };
            query.Criteria.AddCondition<SalesOrder>(e => e.SubmitDate, ConditionOperator.Between, dates[0], dates[1]);

            var accountLink = query.AddLink(Account.EntityLogicalName, Helper.Name<SalesOrder>(e => e.AccountId),
                Helper.Name<Account>(e => e.Id));
            accountLink.Columns = new ColumnSet<Account>(e => e.CustomerTypeCode);
            accountLink.EntityAlias = "ac";
            accountLink.LinkCriteria.AddCondition<Account>(e => e.CustomerTypeCode, ConditionOperator.Equal, customerTypeCode);

            var orderDetailLink = query.AddLink(SalesOrderDetail.EntityLogicalName,
                Helper.Name<SalesOrder>(e => e.SalesOrderId), Helper.Name<SalesOrderDetail>(e => e.SalesOrderId));
            orderDetailLink.Columns = new ColumnSet<SalesOrderDetail>(e => e.Quantity);
            orderDetailLink.EntityAlias = "od";

            var result = Service.RetrieveMultiple(query);

            return result.Entities.Any()
                ? result.Entities.Select(e => e.ToEntity<SalesOrder>()).ToArray()
                : new SalesOrder[] { };
        }

        private IEnumerable<DateTime> GetDates()
        {
            var submitDate = GetValue(e => e.SubmitDate);
            var year = submitDate.Year;
            var month = submitDate.Month;
            yield return new DateTime(year, month, 1);

            var dateEnd = DateTime.DaysInMonth(year, month);
            // To put 23:59:59
            yield return new DateTime(year, month, dateEnd).AddDays(1).AddSeconds(-1);
        }
    }
}
