using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class TransactionContextTargetTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPluginExecutionContext _pluginContext;

        public TransactionContextTargetTests()
        {
            _pluginContext = Substitute.For<IPluginExecutionContext>();
            _pluginContext.InputParameters.Returns(new ParameterCollection());

            _serviceProvider = Substitute.For<IServiceProvider>();
            _serviceProvider.GetService(Arg.Is(typeof(IPluginExecutionContext))).Returns(_pluginContext);
        }

        [Fact]
        public void Can_get_target_message_create()
        {
            var entity = new Entity("entity") { Id = Guid.NewGuid() };
            _pluginContext.MessageName.Returns(PluginMessage.Create);
            _pluginContext.InputParameters["Target"] = entity;
            var context = new TransactionContext<Entity>(_serviceProvider);
            var targetEntity = context.Target.Entity;
            entity["xts_attr"] = new Money(200m);
            Assert.Equal(new Money(200m), targetEntity["xts_attr"]);
        }

        [Fact]
        public void Can_get_target_message_update()
        {
            var entity = new Entity("entity") { Id = Guid.NewGuid() };
            _pluginContext.MessageName.Returns(PluginMessage.Update);
            _pluginContext.InputParameters["Target"] = entity;
            var context = new TransactionContext<Entity>(_serviceProvider);
            var targetEntity = context.Target.Entity;
            entity["xts_attr"] = new Money(200m);
            Assert.Equal(new Money(200m), targetEntity["xts_attr"]);
        }

        [Fact]
        public void Can_get_target_message_delete()
        {
            var reference = new EntityReference("entity", Guid.NewGuid());
            _pluginContext.MessageName.Returns(PluginMessage.Delete);
            _pluginContext.InputParameters["Target"] = reference;
            var context = new TransactionContext<Entity>(_serviceProvider);
            Assert.Equal(reference, context.Target.Entity.ToEntityReference());
        }

        [Fact]
        public void Can_get_target_message_assign()
        {
            var reference = new EntityReference("entity", Guid.NewGuid());
            _pluginContext.MessageName.Returns(PluginMessage.Assign);
            _pluginContext.InputParameters["Target"] = reference;
            var context = new TransactionContext<Entity>(_serviceProvider);
            Assert.Equal(reference, context.Target.Entity.ToEntityReference());
        }

        [Theory]
        [MemberData(nameof(GetTargetMessageSetStateData))]
        public void Can_get_target_message_set_state(string messageName)
        {
            var reference = new EntityReference("entity", Guid.NewGuid());
            _pluginContext.MessageName.Returns(messageName);
            _pluginContext.InputParameters["EntityMoniker"] = reference;
            var context = new TransactionContext<Entity>(_serviceProvider);
            Assert.Equal(reference, context.Target.Entity.ToEntityReference());
        }

        public static IEnumerable<object[]> GetTargetMessageSetStateData()
        {
            yield return new object[] { PluginMessage.SetState };
            yield return new object[] { PluginMessage.SetStateDynamicEntity };
        }
    }
}
