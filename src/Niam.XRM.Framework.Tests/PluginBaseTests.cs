using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using NSubstitute;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class PluginBaseTests
    {
        [Fact]
        public void Retrieve_db_once_for_reference_entity()
        {
            var testHelper = new TestHelper();
            
            testHelper.PluginExecutionContext.Stage.Returns(40);
            testHelper.PluginExecutionContext.MessageName.Returns(PluginMessage.Create);
            var id = Guid.NewGuid();
            var createRequest = new CreateRequest
            {
                Target = new Entity("lead") { Id = id }
            };
            testHelper.PluginExecutionContext.InputParameters.AddRange(createRequest.Parameters);

            var dbEntity = new Entity("lead")
            {
                Id = id,
                ["subject"] = "Hello World"
            };
            testHelper.Service.Retrieve(Arg.Is<string>(name => name == "lead"), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(dbEntity);

            testHelper.Service.DidNotReceive().Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            var plugin = new TestRetrieveInitial(null, null);
            plugin.Execute(testHelper.ServiceProvider);
            Assert.Equal(id, plugin.EntityId);
            testHelper.Service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
        }
        
        [Fact]
        public void Retrieve_image_once_for_reference_entity()
        {
            var testHelper = new TestHelper();

            testHelper.PluginExecutionContext.Stage.Returns(40);
            testHelper.PluginExecutionContext.MessageName.Returns(PluginMessage.Create);
            var id = Guid.NewGuid();
            var createRequest = new CreateRequest
            {
                Target = new Entity("lead") { Id = id }
            };
            testHelper.PluginExecutionContext.InputParameters.AddRange(createRequest.Parameters);

            var imageEntity = new Entity("lead")
            {
                Id = id,
                ["subject"] = "Hello World"
            };
            testHelper.PluginExecutionContext.PostEntityImages["EntityImage"] = imageEntity;

            testHelper.Service.DidNotReceive().Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            var plugin = new TestRetrieveInitial(null, null);
            plugin.Execute(testHelper.ServiceProvider);
            Assert.Equal(id, plugin.EntityId);
            testHelper.Service.DidNotReceive().Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
        }

        public class TestRetrieveInitial : PluginBase
        {
            public Guid EntityId { get; private set; }

            public TestRetrieveInitial(string unsecure, string secure) : base(unsecure, secure)
            {
            }

            protected override void ExecuteCrmPlugin(ITransactionContext<Entity> context)
            {
                EntityId = context.Current.Id;
            }
        }
    }
}
