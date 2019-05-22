using LateBound.Example.Plugins.Business;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using Niam.XRM.Framework.TestHelper;
using System;
using Xunit;

namespace LateBound.Example.Plugins.Tests
{
    public class OnMonthlySummaryTotalPerCustomerTypeTests
    {
        [Theory]
        [InlineData(1, 1000)]
        [InlineData(1, 500)]
        public void SalesOrder_OnMonthlySummaryTotalPerCustomerType_Create(int type,
            decimal amount)
        {
            var account = new Entity { Id = Guid.NewGuid(), LogicalName = "account" }
                .Set("customertypecode", new OptionSetValue(type));

            var order = new Entity { Id = Guid.NewGuid(), LogicalName = "salesorder" }
                .Set("totalamount", new Money(amount))
                .Set("submitdate", new DateTime(2019, 01, 01))
                .Set("accountid", account.ToEntityReference());

            var orderDetail = new Entity { Id = Guid.NewGuid(), LogicalName = "salesorderdetail" }
                .Set("quantity", 10)
                .Set("salesorderid", order.ToEntityReference());

            var testEvent = new TestEvent(account, order, orderDetail);
            testEvent.CreateEventCommand<OnMonthlySummaryTotalPerCustomerType>(order);

            var created = testEvent.Db.Event.Created[0];

            var period = "201901" + type.ToString("00");
            Assert.Equal(period, created.Get<string>("new_period"));
            Assert.Equal(10, created.Get<int?>("new_qty").GetValueOrDefault());
            Assert.Equal(amount, created.Get<Money>("new_totalamount").GetValueOrDefault());
        }

        [Theory]
        [InlineData(1, 1000)]
        [InlineData(2, 500)]
        public void SalesOrder_OnMonthlySummaryTotalPerCustomerType_Update(int type,
            decimal amount)
        {
            var typeNumber = type.ToString("00");
            var orderSummary = new Entity { Id = Guid.NewGuid(), LogicalName = "new_ordersummary" }
                .Set("new_period", "201901" + typeNumber)
                .Set("new_qty", 0)
                .Set("new_totalamount", new Money(0));

            var account = new Entity { Id = Guid.NewGuid(), LogicalName = "account" }
                .Set("customertypecode", new OptionSetValue(type));

            var refOrder = new Entity { Id = Guid.NewGuid(), LogicalName = "salesorder" }
                .Set("totalamount", new Money(5000))
                .Set("submitdate", new DateTime(2019, 01, 01))
                .Set("accountid", account.ToEntityReference());

            var refOrderDetail = new Entity { Id = Guid.NewGuid(), LogicalName = "salesorderdetail" }
                .Set("salesorderid", refOrder.ToEntityReference())
                .Set("quantity", 100);

            var order = new Entity { Id = Guid.NewGuid(), LogicalName = "salesorder" }
                .Set("totalamount", new Money(amount))
                .Set("submitdate", new DateTime(2019, 01, 01))
                .Set("accountid", account.ToEntityReference());

            var orderDetail = new Entity { Id = Guid.NewGuid(), LogicalName = "salesorderdetail" }
                .Set("quantity", 10)
                .Set("salesorderid", order.ToEntityReference());

            var testEvent = new TestEvent(account, order, orderDetail, orderSummary,
                refOrder, refOrderDetail);
            testEvent.UpdateEventCommand<OnMonthlySummaryTotalPerCustomerType>(order);

            var updated = testEvent.Db.Event.Updated[0];

            Assert.Equal(orderSummary.Id, updated.Id);
            Assert.Equal(110, updated.Get<int?>("new_qty").GetValueOrDefault());
            Assert.Equal(amount + 5000, updated.Get<Money>("new_totalamount").GetValueOrDefault());
        }

        [Fact]
        public void SalesOrder_OnMonthlySummaryTotalPerCustomerType_Delete()
        {
            var type = 1;
            var typeNumber = ((int)type).ToString("00");
            var orderSummary = new Entity { Id = Guid.NewGuid(), LogicalName = "new_ordersummary" }
                .Set("new_period", "201901" + typeNumber)
                .Set("new_qty", 0)
                .Set("new_totalamount", new Money(0));

            var account = new Entity { Id = Guid.NewGuid(), LogicalName = "account"}
                .Set("customertypecode", new OptionSetValue(type));

            var refOrder = new Entity { Id = Guid.NewGuid(), LogicalName = "salesorder"}
                .Set("totalamount", new Money(5000))
                .Set("submitdate", new DateTime(2019, 01, 01))
                .Set("accountid", account.ToEntityReference());

            var refOrderDetail = new Entity { Id = Guid.NewGuid(), LogicalName = "salesorderdetail"}
                .Set("salesorderid", refOrder.ToEntityReference())
                .Set("quantity", 100);

            var order = new Entity { Id = Guid.NewGuid(), LogicalName = "salesorder"}
                .Set("totalamount", new Money(1000))
                .Set("submitdate", new DateTime(2019, 01, 01))
                .Set("accountid", account.ToEntityReference());

            var orderDetail = new Entity { Id = Guid.NewGuid(), LogicalName = "salesorderdetail"}
                .Set("quantity", 10)
                .Set("salesorderid", order.ToEntityReference());

            var testEvent = new TestEvent(account, order, orderDetail, orderSummary,
                refOrder, refOrderDetail);
            testEvent.DeleteEventCommand<OnMonthlySummaryTotalPerCustomerType>(order.ToEntityReference());

            var updated = testEvent.Db.Event.Updated[0];

            Assert.Equal(orderSummary.Id, updated.Id);
            Assert.Equal(100, updated.Get<int?>("new_qty").GetValueOrDefault());
            Assert.Equal(5000, updated.Get<Money>("new_totalamount").GetValueOrDefault());
        }
    }
}
