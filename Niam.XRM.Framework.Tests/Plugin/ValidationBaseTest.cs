using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.TestFramework;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class ValidationBaseTest : TestBase
    {
        [Fact]
        public void Can_execute_validate()
        {
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => new ValidationTest(Context).Validate());
            Assert.Equal("TEST", ex.Message);
        }

        private class ValidationTest : ValidationBase
        {
            public ValidationTest(ITransactionContext<Entity> context)
                : base(context)
            {
            }

            protected override void HandleValidate()
            {
                throw new InvalidPluginExecutionException("TEST");
            }
        }
    }
}
