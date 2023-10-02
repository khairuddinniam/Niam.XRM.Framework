using Insurgo.Plugins.Business;
using Insurgo.Plugins.Entities;
using Niam.XRM.Framework;
using Niam.XRM.Framework.TestHelper;
using System;
using System.Linq;
using FakeXrmEasy;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using FakeXrmEasy.Middleware.Pipeline;
using Xunit;

namespace Insurgo.Plugins.Tests.Business
{
    public class CalculatePackageSummariesOnSubscriptionChangeTests
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
        public void Can_add_summary_package()
        {
            var customer = new Account { Id = Guid.NewGuid() }.
                Set(e => e.Name, "Temmy");
            var package = new cr953_package { Id = Guid.NewGuid() }
                .Set(e => e.cr953_monthlyprice, 100);

            var target =
                new cr953_subscription { Id = Guid.NewGuid() }
                .Set(e => e.cr953_customerid, customer.ToEntityReference())
                .Set(e => e.cr953_packageid, package.ToEntityReference())
                .Set(e => e.cr953_qty, 2);
            var test = new TestEvent<cr953_subscription>(Context, customer, package);
            test.CreateEventCommand<CalculatePackageSummariesOnSubscriptionChange>(target);

            Assert.True(test.Db.Event.Created.Any());
            var added = test.Db.Event.Created[0].ToEntity<cr953_packagesummary>();
            Assert.Equal(200, added.GetValue(e => e.cr953_totalamount));
            Assert.Equal(customer.Id, added.Get(e => e.cr953_customerid).Id);
            Assert.Equal("Temmy Summary", added.Get(e => e.cr953_name));
        }

        [Fact]
        public void Can_update_summary_package()
        {
            var customer = new Account { Id = Guid.NewGuid() };
            var package = new cr953_package { Id = Guid.NewGuid() }
                .Set(e => e.cr953_monthlyprice, 100);
            var packageSummary = new cr953_packagesummary { Id = Guid.NewGuid() }
                .Set(e => e.cr953_customerid, customer.ToEntityReference())
                .Set(e => e.cr953_totalamount, 300);

            var target =
                new cr953_subscription { Id = Guid.NewGuid() }
                    .Set(e => e.cr953_customerid, customer.ToEntityReference())
                    .Set(e => e.cr953_packageid, package.ToEntityReference())
                    .Set(e => e.cr953_qty, 2);
            var initial = new cr953_subscription { Id = target.Id }.
                Set(e => e.cr953_total, 300);
            var test = new TestEvent<cr953_subscription>(Context, initial, customer, package, packageSummary);
            test.UpdateEventCommand<CalculatePackageSummariesOnSubscriptionChange>(target);

            Assert.Equal(200, target.GetValue(e => e.cr953_total));
            Assert.True(test.Db.Event.Updated.Any());
            var updated = test.Db.Event.Updated[0].ToEntity<cr953_packagesummary>();
            Assert.Equal(packageSummary.Id, updated.Id);
            Assert.Equal(customer.Id, updated.Get(e => e.cr953_customerid).Id);
            Assert.Equal(200, updated.GetValue(e => e.cr953_totalamount));
        }

        [Fact]
        public void Can_delete_subcription()
        {
            var customer = new Account { Id = Guid.NewGuid() };
            var package = new cr953_package { Id = Guid.NewGuid() }
                .Set(e => e.cr953_monthlyprice, 100);
            var packageSummary = new cr953_packagesummary { Id = Guid.NewGuid() }
                .Set(e => e.cr953_customerid, customer.ToEntityReference())
                .Set(e => e.cr953_totalamount, 100);

            var target =
                new cr953_subscription { Id = Guid.NewGuid() }
                    .Set(e => e.cr953_customerid, customer.ToEntityReference())
                    .Set(e => e.cr953_packageid, package.ToEntityReference())
                    .Set(e => e.cr953_qty, 1)
                    .Set(e => e.cr953_total, 100);

            var test = new TestEvent<cr953_subscription>(Context, target, customer, package, packageSummary);
            test.DeleteEventCommand<CalculatePackageSummariesOnSubscriptionChange>(target.ToEntityReference());

            Assert.True(test.Db.Event.Updated.Any());
            var updated = test.Db.Event.Updated[0].ToEntity<cr953_packagesummary>();
            Assert.Equal(packageSummary.Id, updated.Id);
            Assert.Equal(customer.Id, updated.Get(e => e.cr953_customerid).Id);
            Assert.Equal(0, updated.GetValue(e => e.cr953_totalamount));
        }
    }
}
