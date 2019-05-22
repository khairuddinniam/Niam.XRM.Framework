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
        [InlineData(Account.Options.CustomerTypeCode.Consultant, 500)]
        public void SalesOrder_OnMonthlySummaryTotalPerCustomerType_Create(Account.Options.CustomerTypeCode type,
            decimal amount)
        {
            var account = new Account { Id = Guid.NewGuid() }
                .Set(e => e.CustomerTypeCode, type);

            var order = new SalesOrder { Id = Guid.NewGuid() }
                .Set(e => e.TotalAmount, amount)
                .Set(e => e.SubmitDate, new DateTime(2019, 01, 01))
                .Set(e => e.AccountId, account.ToEntityReference());

            var orderDetail = new SalesOrderDetail { Id = Guid.NewGuid() }
                .Set(e => e.Quantity, 10)
                .Set(e => e.SalesOrderId, order.ToEntityReference());

            var testEvent = new TestEvent<SalesOrder>(account, order, orderDetail);
            testEvent.CreateEventCommand<OnMonthlySummaryTotalPerCustomerType>(order);

            var created = testEvent.Db.Event.Created[0].ToEntity<new_ordersummary>();

            var period = "201901" + ((int)type).ToString("00");
            Assert.Equal(period, created.Get(e => e.new_period));
            Assert.Equal(10, created.GetValue(e => e.new_qty));
            Assert.Equal(amount, created.GetValue(e => e.new_totalamount));
        }

        [Theory]
        [InlineData(Account.Options.CustomerTypeCode.Competitor, 1000)]
        [InlineData(Account.Options.CustomerTypeCode.Consultant, 500)]
        public void SalesOrder_OnMonthlySummaryTotalPerCustomerType_Update(Account.Options.CustomerTypeCode type,
            decimal amount)
        {
            var typeNumber = ((int)type).ToString("00");
            var orderSummary = new new_ordersummary { Id = Guid.NewGuid() }
                .Set(e => e.new_period, "201901" + typeNumber)
                .Set(e => e.new_qty, 0)
                .Set(e => e.new_totalamount, 0);

            var account = new Account { Id = Guid.NewGuid() }
                .Set(e => e.CustomerTypeCode, type);

            var refOrder = new SalesOrder { Id = Guid.NewGuid() }
                .Set(e => e.TotalAmount, 5000)
                .Set(e => e.SubmitDate, new DateTime(2019, 01, 01))
                .Set(e => e.AccountId, account.ToEntityReference());

            var refOrderDetail = new SalesOrderDetail { Id = Guid.NewGuid() }
                .Set(e => e.SalesOrderId, refOrder.ToEntityReference())
                .Set(e => e.Quantity, 100);

            var order = new SalesOrder { Id = Guid.NewGuid() }
                .Set(e => e.TotalAmount, amount)
                .Set(e => e.SubmitDate, new DateTime(2019, 01, 01))
                .Set(e => e.AccountId, account.ToEntityReference());

            var orderDetail = new SalesOrderDetail { Id = Guid.NewGuid() }
                .Set(e => e.Quantity, 10)
                .Set(e => e.SalesOrderId, order.ToEntityReference());

            var testEvent = new TestEvent<SalesOrder>(account, order, orderDetail, orderSummary, 
                refOrder, refOrderDetail);
            testEvent.UpdateEventCommand<OnMonthlySummaryTotalPerCustomerType>(order);

            var updated = testEvent.Db.Event.Updated[0].ToEntity<new_ordersummary>();
            
            Assert.Equal(orderSummary.Id, updated.Id);
            Assert.Equal(110, updated.GetValue(e => e.new_qty));
            Assert.Equal(amount + 5000, updated.GetValue(e => e.new_totalamount));
        }

        [Fact]
        public void SalesOrder_OnMonthlySummaryTotalPerCustomerType_Delete()
        {
            var type = Account.Options.CustomerTypeCode.Consultant;
            var typeNumber = ((int)type).ToString("00");
            var orderSummary = new new_ordersummary { Id = Guid.NewGuid() }
                .Set(e => e.new_period, "201901" + typeNumber)
                .Set(e => e.new_qty, 0)
                .Set(e => e.new_totalamount, 0);

            var account = new Account { Id = Guid.NewGuid() }
                .Set(e => e.CustomerTypeCode, type);

            var refOrder = new SalesOrder { Id = Guid.NewGuid() }
                .Set(e => e.TotalAmount, 5000)
                .Set(e => e.SubmitDate, new DateTime(2019, 01, 01))
                .Set(e => e.AccountId, account.ToEntityReference());

            var refOrderDetail = new SalesOrderDetail { Id = Guid.NewGuid() }
                .Set(e => e.SalesOrderId, refOrder.ToEntityReference())
                .Set(e => e.Quantity, 100);

            var order = new SalesOrder { Id = Guid.NewGuid() }
                .Set(e => e.TotalAmount, 1000)
                .Set(e => e.SubmitDate, new DateTime(2019, 01, 01))
                .Set(e => e.AccountId, account.ToEntityReference());

            var orderDetail = new SalesOrderDetail { Id = Guid.NewGuid() }
                .Set(e => e.Quantity, 10)
                .Set(e => e.SalesOrderId, order.ToEntityReference());

            var testEvent = new TestEvent<SalesOrder>(account, order, orderDetail, orderSummary,
                refOrder, refOrderDetail);
            testEvent.DeleteEventCommand<OnMonthlySummaryTotalPerCustomerType>(order.ToEntityReference());

            var updated = testEvent.Db.Event.Updated[0].ToEntity<new_ordersummary>();

            Assert.Equal(orderSummary.Id, updated.Id);
            Assert.Equal(100, updated.GetValue(e => e.new_qty));
            Assert.Equal(5000, updated.GetValue(e => e.new_totalamount));
        }
    }
}
