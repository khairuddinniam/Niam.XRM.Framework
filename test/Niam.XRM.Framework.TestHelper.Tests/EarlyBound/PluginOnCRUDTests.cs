using System;
using FakeXrmEasy;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using FakeXrmEasy.Middleware.Pipeline;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.TestHelper.Tests.LateBound.Commands;
using Xunit;

namespace Niam.XRM.Framework.TestHelper.Tests.EarlyBound
{
    public class PluginOnCRUDTests
    {
        private IXrmFakedContext Context => (XrmFakedContext)MiddlewareBuilder
            .New()
            .AddCrud()
            .AddFakeMessageExecutors()
            .AddPipelineSimulation()
            .UseCrud()
            .UseMessages()
            .SetLicense(FakeXrmEasyLicense.NonCommercial)
            .Build();

        [Fact]
        public void Can_execute_command_on_create()
        {
            var order = new new_order(Guid.NewGuid());
            
            var target = new new_orderdetail(Guid.NewGuid())
                .Set(e => e.new_orderid, order.ToEntityReference())
                .Set(e => e.new_quantity, 2)
                .Set(e => e.new_priceperitem, 1500);

            var test = new TestEvent(Context, order);
            test.CreateEvent<Plugin>(target);
            var summaryRef = target.Get(e => e.new_orderdetailsummaryid);
            Assert.NotNull(summaryRef);
            var summary = test.Db.Event.Created[0].ToEntity<new_orderdetailsummary>();
            Assert.Equal(3000m, summary.Get(e => e.new_totalprice).Value);
        }
        
        [Fact]
        public void Can_execute_command_on_update()
        {
            var order = new new_order(Guid.NewGuid());

            var orderDetailId = Guid.NewGuid();
            var orderDetailSummary = new new_orderdetailsummary(Guid.NewGuid())
                .Set(e => e.new_orderid, order.ToEntityReference())
                .Set(e => e.new_orderdetailid, new EntityReference(new_orderdetail.EntityLogicalName, orderDetailId))
                .Set(e => e.new_totalprice, 3000);

            var orderDetail = new new_orderdetail(orderDetailId)
                .Set(e => e.new_orderid, order.ToEntityReference())
                .Set(e => e.new_orderdetailsummaryid, orderDetailSummary.ToEntityReference())
                .Set(e => e.new_quantity, 2)
                .Set(e => e.new_priceperitem, 1500);

            var target = new new_orderdetail(orderDetailId)
                .Set(e => e.new_quantity, 3);
            
            var test = new TestEvent(Context, order, orderDetail, orderDetailSummary);
            test.UpdateEvent<Plugin>(target);
            var summary = test.Db.Event.Updated[0].ToEntity<new_orderdetailsummary>();
            Assert.Equal(4500m, summary.Get(e => e.new_totalprice).Value);
        }
        
        [Fact]
        public void Can_execute_command_on_delete()
        {
            var order = new new_order(Guid.NewGuid());

            var orderDetailSummary = new new_orderdetailsummary(Guid.NewGuid());

            var orderDetail = new new_orderdetail(Guid.NewGuid())
                .Set(e => e.new_orderid, order.ToEntityReference())
                .Set(e => e.new_orderdetailsummaryid, orderDetailSummary.ToEntityReference());
            
            var test = new TestEvent(Context, order, orderDetail, orderDetailSummary);
            test.DeleteEvent<Plugin>(orderDetail.ToEntityReference());
            Assert.Equal(orderDetailSummary.ToEntityReference(), test.Db.Event.Deleted[0]);
        }
        
        private class Plugin : PluginBase<new_orderdetail>, IPlugin
        {
            public Plugin(string unsecure, string secure) : base(unsecure, secure)
            {
            }

            protected override void ExecuteCrmPlugin(IPluginContext<new_orderdetail> context)
            {
                if (context.Current.ContainsAny(e => e.new_quantity, e => e.new_priceperitem))
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