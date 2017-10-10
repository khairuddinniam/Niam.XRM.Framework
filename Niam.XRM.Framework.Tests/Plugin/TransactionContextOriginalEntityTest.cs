using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class TransactionContextOriginalEntityTest
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPluginExecutionContext _pluginContext;
        private readonly IOrganizationService _service;

        public TransactionContextOriginalEntityTest()
        {
            _pluginContext = Substitute.For<IPluginExecutionContext>();
            _pluginContext.InputParameters.Returns(new ParameterCollection());
            _pluginContext.PreEntityImages.Returns(new EntityImageCollection());

            _serviceProvider = Substitute.For<IServiceProvider>();
            _serviceProvider.GetService(Arg.Is(typeof(IPluginExecutionContext))).Returns(_pluginContext);

            _service = Substitute.For<IOrganizationService>();
            var serviceFactory = Substitute.For<IOrganizationServiceFactory>();
            serviceFactory.CreateOrganizationService(Arg.Any<Guid?>()).Returns(_service);
            _serviceProvider.GetService(Arg.Is(typeof(IOrganizationServiceFactory))).Returns(serviceFactory);
        }

        [Fact]
        public void Can_get_original_entity_message_create()
        {
            var entity = new Entity("entity") { Id = Guid.NewGuid() };
            _pluginContext.MessageName.Returns(PluginMessage.Create);
            _pluginContext.InputParameters["Target"] = entity;
            var context = new TransactionContext<Entity>(_serviceProvider);
            Assert.NotSame(entity, context.Initial.Entity);
            Assert.Equal(entity.ToEntityReference(), context.Initial.Entity.ToEntityReference());
        }

        [Theory]
        [MemberData(nameof(GetOriginalEntityNonCreateData))]
        public void Can_get_original_entity_message_non_create(string message, string inputKey, bool isReference)
        {
            var id = Guid.NewGuid();
            var dbEntity = new Entity("entity")
            {
                Id = id,
                ["xts_attribute"] = new Money(1250m)
            };
            _service.Retrieve(Arg.Is("entity"), Arg.Is(id), Arg.Any<ColumnSet>()).Returns(dbEntity);

            var entity = new Entity("entity") { Id = id };
            _pluginContext.Stage.Returns((int) SdkMessageProcessingStepStage.Preoperation);
            _pluginContext.MessageName.Returns(message);
            _pluginContext.InputParameters[inputKey] = isReference ? (object)entity.ToEntityReference() : entity;

            var context = new TransactionContext<Entity>(_serviceProvider);
            Assert.NotSame(dbEntity, context.Initial.Entity);
            Assert.NotSame(entity, context.Initial.Entity);
            Assert.Equal(entity.ToEntityReference(), context.Initial.Entity.ToEntityReference());
            Assert.Equal(1250m, context.Initial.Entity.GetAttributeValue<Money>("xts_attribute").Value);

            _service.Received(1).Retrieve(Arg.Is("entity"), Arg.Is(id), Arg.Any<ColumnSet>());
        }

        public static IEnumerable<object[]> GetOriginalEntityNonCreateData()
        {
            yield return new object[] { PluginMessage.Update, "Target", false };
            yield return new object[] { PluginMessage.Delete, "Target", true };
            yield return new object[] { PluginMessage.Assign, "Target", true };
            yield return new object[] { PluginMessage.SetState, "EntityMoniker", true };
            yield return new object[] { PluginMessage.SetStateDynamicEntity, "EntityMoniker", true };
        } 
    }
}
