using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class TransactionContextInputTest
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPluginExecutionContext _pluginContext;

        public TransactionContextInputTest()
        {
            _pluginContext = Substitute.For<IPluginExecutionContext>();
            _pluginContext.InputParameters.Returns(new ParameterCollection());

            _serviceProvider = Substitute.For<IServiceProvider>();
            _serviceProvider.GetService(Arg.Is(typeof(IPluginExecutionContext))).Returns(_pluginContext);
        }

        [Fact]
        public void Can_get_input_message_create()
        {
            var entity = new Entity("entity") { Id = Guid.NewGuid() };
            _pluginContext.MessageName.Returns(PluginMessage.Create);
            _pluginContext.InputParameters["Target"] = entity;
            var context = new TransactionContext<Entity>(_serviceProvider);
            var inputEntity = context.Input.Entity;
            entity["xts_attr"] = new Money(200m);
            Assert.Equal(new Money(200m), inputEntity["xts_attr"]);
        }

        [Fact]
        public void Can_get_input_message_update()
        {
            var entity = new Entity("entity") { Id = Guid.NewGuid() };
            _pluginContext.MessageName.Returns(PluginMessage.Update);
            _pluginContext.InputParameters["Target"] = entity;
            var context = new TransactionContext<Entity>(_serviceProvider);
            var inputEntity = context.Input.Entity;
            entity["xts_attr"] = new Money(200m);
            Assert.Equal(new Money(200m), inputEntity["xts_attr"]);
        }

        [Fact]
        public void Can_get_input_message_delete()
        {
            var reference = new EntityReference("entity", Guid.NewGuid());;
            _pluginContext.MessageName.Returns(PluginMessage.Delete);
            _pluginContext.InputParameters["Target"] = reference;
            var context = new TransactionContext<Entity>(_serviceProvider);
            Assert.Equal(reference, context.Input.Entity.ToEntityReference());
        }

        [Fact]
        public void Can_get_input_message_assign()
        {
            var reference = new EntityReference("entity", Guid.NewGuid()); ;
            _pluginContext.MessageName.Returns(PluginMessage.Assign);
            _pluginContext.InputParameters["Target"] = reference;
            var context = new TransactionContext<Entity>(_serviceProvider);
            Assert.Equal(reference, context.Input.Entity.ToEntityReference());
        }

        [Theory]
        [MemberData(nameof(GetInputMessageSetStateData))]
        public void Can_get_input_message_set_state(string messageName)
        {
            var reference = new EntityReference("entity", Guid.NewGuid()); ;
            _pluginContext.MessageName.Returns(messageName);
            _pluginContext.InputParameters["EntityMoniker"] = reference;
            var context = new TransactionContext<Entity>(_serviceProvider);
            Assert.Equal(reference, context.Input.Entity.ToEntityReference());
        }

        public static IEnumerable<object[]> GetInputMessageSetStateData()
        {
            yield return new object[] { PluginMessage.SetState };
            yield return new object[] { PluginMessage.SetStateDynamicEntity };
        }
    }
}
