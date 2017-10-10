using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Plugin.Strategy;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.Strategy
{
    public class InputEntityGetterTest
    {
        [Fact]
        public void Can_get_input_entity_create()
        {
            var entity = new Entity();
            var context = Substitute.For<IPluginExecutionContext>();
            context.InputParameters.Returns(new ParameterCollection
            {
                ["Target"] = entity
            });

            var handler = InputEntityGetter.GetHandler(PluginMessage.Create);
            Assert.Same(entity, handler.Get(context));
        }

        [Fact]
        public void Can_get_input_entity_update()
        {
            var entity = new Entity();
            var context = Substitute.For<IPluginExecutionContext>();
            context.InputParameters.Returns(new ParameterCollection
            {
                ["Target"] = entity
            });

            var handler = InputEntityGetter.GetHandler(PluginMessage.Update);
            Assert.Same(entity, handler.Get(context));
        }

        [Fact]
        public void Can_get_input_entity_delete()
        {
            var reference = new EntityReference("test", Guid.NewGuid());
            var context = Substitute.For<IPluginExecutionContext>();
            context.InputParameters.Returns(new ParameterCollection
            {
                ["Target"] = reference
            });

            var handler = InputEntityGetter.GetHandler(PluginMessage.Delete);
            Assert.Equal(reference, handler.Get(context).ToEntityReference());
        }

        [Fact]
        public void Can_get_input_entity_assign()
        {
            var reference = new EntityReference("test", Guid.NewGuid());
            var context = Substitute.For<IPluginExecutionContext>();
            context.InputParameters.Returns(new ParameterCollection
            {
                ["Target"] = reference
            });

            var handler = InputEntityGetter.GetHandler(PluginMessage.Assign);
            Assert.Equal(reference, handler.Get(context).ToEntityReference());
        }

        [Theory]
        [MemberData(nameof(GetInputEntitySetStateData))]
        public void Can_get_input_entity_set_state(string message)
        {
            var reference = new EntityReference("test", Guid.NewGuid());
            var context = Substitute.For<IPluginExecutionContext>();
            context.InputParameters.Returns(new ParameterCollection
            {
                ["EntityMoniker"] = reference
            });

            var handler = InputEntityGetter.GetHandler(message);
            Assert.Equal(reference, handler.Get(context).ToEntityReference());
        }

        public static IEnumerable<object[]> GetInputEntitySetStateData()
        {
            yield return new object[] { PluginMessage.SetState };
            yield return new object[] { PluginMessage.SetStateDynamicEntity };
        } 
    }
}
