using LateBound.Example.Plugins.Business;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using Niam.XRM.Framework.TestHelper;
using System;
using Xunit;

namespace LateBound.Example.Plugins.Tests
{
    public class OnCalculateSummaryTotalTests
    {
        [Fact]
        public void SalesOrder_OnCalculateSummaryTotal_CreateOrderSummaryRecord()
        {
            var order = new Entity("salesorder") { Id = Guid.NewGuid() }
                .Set("submitdate", new DateTime(2019, 01, 01))
                .Set("totalamount", new Money(1000));

            var testEvent = new TestEvent();
            testEvent.CreateEventCommand<OnCalculateSummaryTotal>(order);

            var created = testEvent.Db.Event.Created[0];
            Assert.Equal("201901", created.Get<string>("new_period"));
            Assert.Equal(1000m, created.Get<Money>("new_totalamount").GetValueOrDefault());
        }

        [Fact]
        public void SalesOrder_OnCalculateSummaryTotal_UpdateSummaryRecord()
        {
            var orderSummary = new Entity("new_ordersummary") { Id = Guid.NewGuid() }
                .Set("new_period", "201901")
                .Set("new_totalamount", new Money(1200m));

            var testEvent = new TestEvent(orderSummary);

            var order = new Entity("salesorder") { Id = Guid.NewGuid() }
                .Set("submitdate", new DateTime(2019, 01, 01))
                .Set("totalamount", new Money(1000));

            testEvent.CreateEventCommand<OnCalculateSummaryTotal>(order);

            var updatedOrderSummary = testEvent.Db.Event.Updated[0];
            Assert.Equal(2200m, updatedOrderSummary.Get<Money>("new_totalamount").GetValueOrDefault());
        }

        [Fact]
        public void SalesOrder_OnCalculateSummaryTotal_UpdateSalesOrder()
        {
            var id = Guid.NewGuid();
            var initial = new Entity("salesorder") { Id = id }
                .Set("totalamount", new Money(500));

            var orderSummary = new Entity("new_ordersummary") { Id = Guid.NewGuid() }
                .Set("new_period", "201901")
                .Set("new_totalamount", new Money(1200m));

            var order = new Entity("salesorder") { Id = id }
                .Set("submitdate", new DateTime(2019, 01, 01))
                .Set("totalamount", new Money(1000));

            var testEvent = new TestEvent(initial, orderSummary);
            testEvent.UpdateEventCommand<OnCalculateSummaryTotal>(order);

            var updatedOrderSummary = testEvent.Db.Event.Updated[0];
            Assert.Equal(1700m, updatedOrderSummary.Get<Money>("new_totalamount").GetValueOrDefault());
        }

        [Fact]
        public void SalesOrder_OnCalculateSummaryTotal_DeleteSalesOrder()
        {
            var orderSummary = new Entity("new_ordersummary") { Id = Guid.NewGuid() }
                .Set("new_period", "201901")
                .Set("new_totalamount", new Money(1200m));

            var order = new Entity("salesorder") { Id = Guid.NewGuid() }
                .Set("submitdate", new DateTime(2019, 01, 01))
                .Set("totalamount", new Money(1000));

            var testEvent = new TestEvent(order, orderSummary);
            testEvent.DeleteEventCommand<OnCalculateSummaryTotal>(order.ToEntityReference());

            var updatedOrderSummary = testEvent.Db.Event.Updated[0];
            Assert.Equal(200m, updatedOrderSummary.Get<Money>("new_totalamount").GetValueOrDefault());
        }
    }
}
