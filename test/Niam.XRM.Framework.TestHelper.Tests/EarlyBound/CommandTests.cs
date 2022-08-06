using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.TestHelper.Tests.EarlyBound.Commands;
using System;
using Xunit;

namespace Niam.XRM.Framework.TestHelper.Tests.EarlyBound
{
    public class CommandTests
    {
        [Fact]
        public void Can_execute_command_on_create()
        {
            var order = new new_order(Guid.NewGuid());

            var target = new new_orderdetail(Guid.NewGuid())
                .Set(e => e.new_orderid, order.ToEntityReference())
                .Set(e => e.new_quantity, 2)
                .Set(e => e.new_priceperitem, 1500);

            var test = new TestEvent<new_orderdetail>(order);
            test.CreateEventCommand<CalculateTotalPrice>(target);
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

            var test = new TestEvent<new_orderdetail>(order, orderDetail, orderDetailSummary);
            test.UpdateEventCommand<CalculateTotalPrice>(target);
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

            var test = new TestEvent<new_orderdetail>(order, orderDetail, orderDetailSummary);
            test.DeleteEventCommand<DeleteOrderDetailSummary>(orderDetail.ToEntityReference());
            Assert.Equal(orderDetailSummary.ToEntityReference(), test.Db.Event.Deleted[0]);
        }


        [Fact]
        public void Can_execute_command_for_retrieve()
        {
            var order = new new_order(Guid.NewGuid()).Set(e => e.new_name, "TEST");
            var orderDetail1 = new new_orderdetail(Guid.NewGuid())
                .Set(e => e.new_orderid, order.ToEntityReference())
                .Set(e => e.new_priceperitem, 100)
                .Set(e => e.new_quantity, 10);

            var orderDetail2 = new new_orderdetail(Guid.NewGuid())
                .Set(e => e.new_orderid, order.ToEntityReference())
                .Set(e => e.new_priceperitem, 10)
                .Set(e => e.new_quantity, 10);

            var orderDetailSummary = new new_orderdetailsummary(Guid.NewGuid());

            var test = new TestEvent<new_orderdetailsummary>(order, orderDetail1, orderDetail2);
            test.CreateEventCommand<RetrieveTotalPrice>(orderDetailSummary);

            Assert.Equal(1100m, orderDetailSummary.GetValue(e => e.new_totalprice));
        }

        [Fact]
        public void Can_execute_command_other_early_bound_generator()
        {
            var account = new Account { Id = Guid.NewGuid() }.Set(e => e.StateCode, Account.OptionSets.StateCode.Active);

            var test = new TestEvent<Account>(account);
            test.CreateEventCommand<AccountCommand>(null);

            Assert.NotNull(test.Db.Event.Deleted);
            Assert.Equal(account.Id, test.Db.Event.Deleted[0].Id);

            Assert.NotNull(test.Db.Event.Created);

            Assert.NotNull(test.Db.Event.Updated);
            Assert.Equal(test.Db.Event.Created[0].Id, test.Db.Event.Updated[0].Id);
        }
    }
}