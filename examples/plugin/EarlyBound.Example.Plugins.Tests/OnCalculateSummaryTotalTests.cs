using System;
using EarlyBound.Example.Plugins.Business;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using Niam.XRM.Framework.TestHelper;
using Xunit;

namespace EarlyBound.Example.Plugins.Tests
{
    public class OnCalculateSummaryTotalTests
    {
        [Fact]
        public void SalesOrder_OnCalculateSummaryTotal_CreateOrderSummaryRecord()
        {
            var order = new Entities.SalesOrder { Id = Guid.NewGuid() }
                .Set(e => e.SubmitDate, new DateTime(2019, 01, 01))
                .Set(e => e.TotalAmount, 1000);

            var testEvent = new TestEvent<Entities.SalesOrder>();
            testEvent.CreateEventCommand<OnCalculateSummaryTotal>(order);

            var created = testEvent.Db.Event.Created[0].ToEntity<Entities.new_ordersummary>();
            Assert.Equal("201901", created.Get(e => e.new_period));
            Assert.Equal(1000m, created.GetValue(e => e.new_totalamount));
        }

        [Fact]
        public void SalesOrder_OnCalculateSummaryTotal_UpdateSummaryRecord()
        {
            var orderSummary = new Entities.new_ordersummary { Id = Guid.NewGuid() }
                .Set(e => e.new_period, "201901")
                .Set(e => e.new_totalamount, 1200m);

            var testEvent = new TestEvent<Entities.SalesOrder>(orderSummary);

           var order = new Entities.SalesOrder { Id = Guid.NewGuid() }
                .Set(e => e.SubmitDate, new DateTime(2019, 01, 01))
                .Set(e => e.TotalAmount, 1000);

            testEvent.CreateEventCommand<OnCalculateSummaryTotal>(order);

            var updatedOrderSummary = testEvent.Db.Event.Updated[0].ToEntity<Entities.new_ordersummary>();
            Assert.Equal(2200m, updatedOrderSummary.GetValue(e => e.new_totalamount));
        }

        [Fact]
        public void SalesOrder_OnCalculateSummaryTotal_UpdateSalesOrder()
        {
            var id = Guid.NewGuid();
            var initial = new Entities.SalesOrder { Id = id }
                .Set(e => e.TotalAmount, new Money(500));

            var orderSummary = new Entities.new_ordersummary { Id = Guid.NewGuid() }
              .Set(e => e.new_period, "201901")
              .Set(e => e.new_totalamount, 1200m);

            var order = new Entities.SalesOrder { Id = id }
               .Set(e => e.SubmitDate, new DateTime(2019, 01, 01))
               .Set(e => e.TotalAmount, 1000);

            var testEvent = new TestEvent<Entities.SalesOrder>(initial, orderSummary);
            testEvent.UpdateEventCommand<OnCalculateSummaryTotal>(order);

            var updatedOrderSummary = testEvent.Db.Event.Updated[0].ToEntity<Entities.new_ordersummary>();
            Assert.Equal(1700m, updatedOrderSummary.GetValue(e => e.new_totalamount));
        }

        [Fact]
        public void SalesOrder_OnCalculateSummaryTotal_DeleteSalesOrder()
        {
            var orderSummary = new Entities.new_ordersummary { Id = Guid.NewGuid() }
              .Set(e => e.new_period, "201901")
              .Set(e => e.new_totalamount, 1200m);

            var order = new Entities.SalesOrder { Id = Guid.NewGuid() }
               .Set(e => e.SubmitDate, new DateTime(2019, 01, 01))
               .Set(e => e.TotalAmount, 1000);

            var testEvent = new TestEvent<Entities.SalesOrder>(order, orderSummary);
            testEvent.DeleteEventCommand<OnCalculateSummaryTotal>(order.ToEntityReference());

            var updatedOrderSummary = testEvent.Db.Event.Updated[0].ToEntity<Entities.new_ordersummary>();
            Assert.Equal(200m, updatedOrderSummary.GetValue(e => e.new_totalamount));
        }
    }
}
