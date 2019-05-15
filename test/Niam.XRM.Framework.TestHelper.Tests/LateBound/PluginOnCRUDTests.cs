using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.TestHelper.Tests.LateBound.Commands;
using Xunit;

namespace Niam.XRM.Framework.TestHelper.Tests.LateBound
{
    public class PluginOnCRUDTests
    {
        [Fact]
        public void Can_execute_plugin_on_create()
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
            test.CreateEvent<Plugin>(target);
            var summaryRef = target.Get<EntityReference>("new_orderdetailsummaryid");
            Assert.NotNull(summaryRef);
            var summary = test.Db.Event.Created[0];
            Assert.Equal(3000m, summary.Get<Money>("new_totalprice").Value);
        }
        
        [Fact]
        public void Can_execute_plugin_on_update()
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
            test.UpdateEvent<Plugin>(target);
            var summary = test.Db.Event.Updated[0];
            Assert.Equal(4500m, summary.Get<Money>("new_totalprice").Value);
        }
        
        [Fact]
        public void Can_execute_plugin_on_delete()
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
            test.DeleteEvent<Plugin>(orderDetail.ToEntityReference());
            Assert.Equal(orderDetailSummary.ToEntityReference(), test.Db.Event.Deleted[0]);
        }
        
        private class Plugin : PluginBase, IPlugin
        {
            public Plugin(string unsecure, string secure) : base(unsecure, secure)
            {
            }

            protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
            {
                if (context.Current.ContainsAny("new_quantity", "new_priceperitem"))
                {
                    new CalculateTotalPrice(context).Execute();
                }

                if (context.PluginExecutionContext.MessageName == "Delete")
                {
                    new DeleteOrderDetailSummary(context).Execute();
                }
            }
        }
    }
}