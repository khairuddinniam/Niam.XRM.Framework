using EarlyBound.Example.Plugins.Business;
using Entities;
using Niam.XRM.Framework;
using Niam.XRM.Framework.TestHelper;
using System;
using Xunit;

namespace EarlyBound.Example.Plugins.Tests
{
    public class OnMonthlySummaryTotalPerCustomerTypeTests
    {
        [Theory]
        [InlineData(Account.Options.CustomerTypeCode.Competitor, 1000)]
        public void SalesOrder_OnMonthlySummaryTotalPerCustomerType_Create(Account.Options.CustomerTypeCode type,
            decimal amount)
        {
            var account = new Account { Id = Guid.NewGuid() };
            account.Set(e => e.CustomerTypeCode, type);

            var order = new SalesOrder { Id = Guid.NewGuid() };
            order.Set(e => e.TotalAmount, amount);
            order.Set(e => e.SubmitDate, new DateTime(2019, 01, 01));
            order.Set(e => e.AccountId, account.ToEntityReference());

            var orderDetail = new SalesOrderDetail { Id = Guid.NewGuid() };
            orderDetail.Set(e => e.Quantity, 10);
            orderDetail.Set(e => e.SalesOrderId, order.ToEntityReference());

            var testEvent = new TestEvent<SalesOrder>(account, order, orderDetail);
            testEvent.CreateEventCommand<OnMonthlySummaryTotalPerCustomerType>(order);

            var created = testEvent.Db.Event.Created[0].ToEntity<new_ordersummary>();

            var period = "201901" + ((int)type).ToString("00");
            Assert.Equal(period, created.Get(e => e.new_period));
            Assert.Equal(10, created.GetValue(e => e.new_qty));
            Assert.Equal(amount, created.GetValue(e => e.new_totalamount));
        }
    }
}
