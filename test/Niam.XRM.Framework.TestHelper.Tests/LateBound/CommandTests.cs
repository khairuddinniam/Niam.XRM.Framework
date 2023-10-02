using System;
using FakeXrmEasy;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using FakeXrmEasy.Middleware.Pipeline;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.TestHelper.Tests.LateBound.Commands;
using Xunit;

namespace Niam.XRM.Framework.TestHelper.Tests.LateBound
{
    public class CommandTests
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
            var order = new Entity("new_order", Guid.NewGuid());

            var target = new Entity("new_orderdetail", Guid.NewGuid())
            {
                ["new_orderid"] = order.ToEntityReference(),
                ["new_quantity"] = 2,
                ["new_priceperitem"] = new Money(1500)
            };

            var test = new TestEvent(Context, order);
            test.CreateEventCommand<CalculateTotalPrice>(target);
            var summaryRef = target.Get<EntityReference>("new_orderdetailsummaryid");
            Assert.NotNull(summaryRef);
            var summary = test.Db.Event.Created[0];
            Assert.Equal(3000m, summary.Get<Money>("new_totalprice").Value);
        }

        [Fact]
        public void Can_execute_command_on_update()
        {
            var order = new Entity("new_order", Guid.NewGuid());
            var orderDetailId = Guid.NewGuid();
            var orderDetailSummary = new Entity("new_orderdetailsummary", Guid.NewGuid())
            {
                ["new_orderid"] = order.ToEntityReference(),
                ["new_orderdetailid"] = new EntityReference("new_orderdetail", orderDetailId),
                ["new_totalprice"] = new Money(3000)
            };

            var orderDetail = new Entity("new_orderdetail", orderDetailId)
            {
                ["new_orderid"] = order.ToEntityReference(),
                ["new_orderdetailsummaryid"] = orderDetailSummary.ToEntityReference(),
                ["new_quantity"] = 2,
                ["new_priceperitem"] = new Money(1500)
            };

            var target = new Entity("new_orderdetail", orderDetailId)
            {
                ["new_quantity"] = 3
            };

            var test = new TestEvent(Context, order, orderDetail, orderDetailSummary);
            test.UpdateEventCommand<CalculateTotalPrice>(target);
            var summary = test.Db.Event.Updated[0];
            Assert.Equal(4500m, summary.Get<Money>("new_totalprice").Value);
        }

        [Fact]
        public void Can_execute_command_on_delete()
        {
            var order = new Entity("new_order", Guid.NewGuid());

            var orderDetailSummary = new Entity("new_orderdetailsummary", Guid.NewGuid());

            var orderDetail = new Entity("new_orderdetail", Guid.NewGuid())
            {
                ["new_orderid"] = order.ToEntityReference(),
                ["new_orderdetailsummaryid"] = orderDetailSummary.ToEntityReference()
            };

            var test = new TestEvent(Context,order, orderDetail, orderDetailSummary);
            test.DeleteEventCommand<DeleteOrderDetailSummary>(orderDetail.ToEntityReference());
            Assert.Equal(orderDetailSummary.ToEntityReference(), test.Db.Event.Deleted[0]);
        }
    }
}