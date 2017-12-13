using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.TestFramework.Tests
{
    public class LateBoundTestBaseTest : TestBase
    {
        protected override void Configure(IPluginConfiguration<Entity> config)
        {
            config.TransactionContext.ColumnSet = new ColumnSet("first", "second");
        }
        
        [Fact]
        public void Can_execute_late_bound_operation()
        {
            var reference = new Entity("entity")
            {
                Id = Guid.NewGuid(),
                ["first"] = 1000,
                ["second"] = 2000
            };
            Reference = reference;
            new LateBoundOperation(Context).Execute();
            Assert.Equal(3000, Context.Input.Get<int?>("total"));
        }

        public class LateBoundOperation : OperationBase
        {
            public LateBoundOperation(ITransactionContext<Entity> context) : base(context)
            {
            }

            protected override void HandleExecute()
            {
                var first = Get<int?>("first");
                var second = Get<int?>("second");
                var total = first + second;
                Set("total", total);
            }
        }
    }
}