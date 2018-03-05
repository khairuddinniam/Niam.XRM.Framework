using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.Configurations;
using Niam.XRM.TestFramework;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class ValidationBaseTests : TestBase
    {
        private readonly ITestOutputHelper _output;

        public ValidationBaseTests(ITestOutputHelper output)
        {
            _output = output;
        }

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

        [Theory]
        [InlineData(PluginLogOption.Crm)]
        [InlineData(PluginLogOption.File)]
        public void Can_log_validation_execution(PluginLogOption pluginLogOption)
        {
            var list = new List<string>();
            var tracingService = Substitute.For<ITracingService>();
            tracingService.When(s => s.Trace(Arg.Any<string>(), Arg.Any<object[]>()))
                .Do(ci => {
                    var format = ci.ArgAt<string>(0);
                    var args = ci.ArgAt<object[]>(1);
                    string log = String.Format(format, args);
                    _output.WriteLine(log);
                    list.Add(log);
                });

            var context = Substitute.For<ITransactionContext<xts_entity>>();
            context.LogOption.Returns(pluginLogOption);
            context.TracingService.Returns(tracingService);

            Assert.ThrowsAny<Exception>(() => new ValidationTest(context).Validate());
            Assert.StartsWith("Entered", list[0]);
            Assert.StartsWith("Exiting", list[1]);
        }
    }
}
