using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.TestHelper.Tests.EarlyBound.Commands;
using Xunit;

namespace Niam.XRM.Framework.TestHelper.Tests.EarlyBound
{
    public class CommandTests
    {
        [Fact]
        public void Can_execute_command_on_create()
        {
            var order = new Entity("new_order")
            {
                Id = Guid.NewGuid()
            };
            
            var target = new Entity("new_orderdetail")
            {
                Id = Guid.NewGuid(),
                ["new_orderid"] = order.ToEntityReference(),
                ["new_quantity"] = 2,
                ["new_priceperitem"] = new Money(1500)
            };

            var test = new TestEvent(order);
            test.CreateEventCommand<CalculateTotalPrice>(target);
            var summaryRef = target.Get<EntityReference>("new_orderdetailsummaryid");
            Assert.NotNull(summaryRef);
            var summary = test.Db.Event.Created[0];
            Assert.Equal(3000m, summary.Get<Money>("new_totalprice").Value);
        }
        
        [Fact]
        public void Can_execute_command_on_update()
        {
            var order = new Entity("new_order")
            {
                Id = Guid.NewGuid()
            };

            var orderDetailId = Guid.NewGuid();
            var orderDetailSummary = new Entity("new_orderdetailsummary")
            {
                Id = Guid.NewGuid(),
                ["new_orderid"] = order.ToEntityReference(),
                ["new_orderdetailid"] = new EntityReference("new_orderdetail", orderDetailId),
                ["new_totalprice"] = new Money(3000)
            };
            
            var orderDetail = new Entity("new_orderdetail")
            {
                Id = orderDetailId,
                ["new_orderid"] =order.ToEntityReference(),
                ["new_orderdetailsummaryid"] = orderDetailSummary.ToEntityReference(),
                ["new_quantity"] = 2,
                ["new_priceperitem"] = new Money(1500)
            };
            
            var target = new Entity("new_orderdetail")
            {
                Id = orderDetailId,
                ["new_quantity"] = 3 
            };
            
            var test = new TestEvent(order, orderDetail, orderDetailSummary);
            test.UpdateEventCommand<CalculateTotalPrice>(target);
            var summary = test.Db.Event.Updated[0];
            Assert.Equal(4500m, summary.Get<Money>("new_totalprice").Value);
        }
        
        [Fact]
        public void Can_execute_command_on_delete()
        {
            var order = new Entity("new_order")
            {
                Id = Guid.NewGuid()
            };

            var orderDetailSummary = new Entity("new_orderdetailsummary")
            {
                Id = Guid.NewGuid()
            };
            
            var orderDetail = new Entity("new_orderdetail")
            {
                Id = Guid.NewGuid(),
                ["new_orderid"] = order.ToEntityReference(),
                ["new_orderdetailsummaryid"] = orderDetailSummary.ToEntityReference()
            };
            
            var test = new TestEvent(order, orderDetail, orderDetailSummary);
            test.DeleteEventCommand<DeleteOrderDetailSummary>(orderDetail.ToEntityReference());
            Assert.Equal(orderDetailSummary.ToEntityReference(), test.Db.Event.Deleted[0]);
        }
    }
}