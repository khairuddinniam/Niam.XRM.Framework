using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Xunit;

namespace Niam.XRM.TestFramework.Tests
{
    public class TestExtensionsTests
    {
        [Fact]
        public void Can_set_request()
        {
            var request = new UpdateRequest
            {
                Target = new Entity()
            };

            var context = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection()
            };
            context.SetRequest(request);

            Assert.Same(request.Target, context.InputParameters["Target"]);
            Assert.Equal("Update", context.MessageName);
            Assert.True(context.InputParameters.ContainsKey("ConcurrencyBehavior"));
        }
    }
}
