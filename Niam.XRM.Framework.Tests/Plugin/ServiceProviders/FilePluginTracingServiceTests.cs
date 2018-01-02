using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Plugin.ServiceProviders;
using NSubstitute;
using System;
using System.IO;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.ServiceProviders
{
    public class FilePluginTracingServiceTests
    {
        [Fact]
        public void Can_write_to_file()
        {
            var correlationId = Guid.NewGuid();
            var entityName = "entity";
            var depth = 2;
            var context = Substitute.For<IPluginExecutionContext>();
            context.CorrelationId.Returns(correlationId);
            context.PrimaryEntityName.Returns(entityName);
            context.Depth.Returns(depth);
            context.SharedVariables.Returns(new ParameterCollection { ["file-log-index"] = 4 });

            var first = Guid.NewGuid();
            var second = Guid.NewGuid();
            using (var tracingService = new FilePluginTracingService("", context))
            {
                tracingService.Trace("First: {0:D}", first);
                tracingService.Trace("Second: {0:D}", second);
            }

            var fileName = $"{correlationId:D}-{entityName}-{depth}-5.txt";
            Assert.True(File.Exists(fileName));
            var contents = File.ReadAllLines(fileName);
            Assert.Equal($"First: {first:D}", contents[0]);
            Assert.Equal($"Second: {second:D}", contents[1]);
            Assert.Equal(5, context.SharedVariables["file-log-index"]);

            File.Delete(fileName);
        }
    }
}
