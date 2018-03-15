using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.Configurations;
using Xunit;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class TransactionContextReferenceEntityTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPluginExecutionContext _pluginContext;
        private readonly IOrganizationService _service;

        public TransactionContextReferenceEntityTests()
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
        public void Can_get_reference_message_on_create()
        {
            var entity = new Entity("entity")
            {
                Id = Guid.NewGuid(),
                ["xts_attribute"] = new Money(12345m)
            };

            _pluginContext.MessageName.Returns(PluginMessage.Create);
            _pluginContext.InputParameters["Target"] = entity;
            var context = new TransactionContext<Entity>(_serviceProvider);
            Assert.NotSame(context.Initial, context.Current);
            Assert.NotSame(entity, context.Current.Entity);
            Assert.Equal(entity.ToEntityReference(), context.Current.Entity.ToEntityReference());
            Assert.Equal(12345m, context.Current.Entity.GetAttributeValue<Money>("xts_attribute").Value);
        }

        [Theory]
        [MemberData(nameof(GetReferenceNonCreateData))]
        public void Can_get_reference_on_message_non_create(string message, string inputKey, bool isReference)
        {
            var id = Guid.NewGuid();
            var dbEntity = new Entity("entity")
            {
                Id = id,
                ["xts_attribute"] = new Money(1250m)
            };
            _service.Retrieve(Arg.Is("entity"), Arg.Is(id), Arg.Any<ColumnSet>()).Returns(dbEntity);

            var entity = new Entity("entity") { Id = id };
            _pluginContext.Stage.Returns((int)SdkMessageProcessingStepStage.Preoperation);
            _pluginContext.MessageName.Returns(message);
            _pluginContext.InputParameters[inputKey] = isReference ? (object)entity.ToEntityReference() : entity;

            var config = new PluginConfiguration<Entity>
            {
                ColumnSet = new ColumnSet("xts_attribute")
            };
            var context = new TransactionContext<Entity>(_serviceProvider, config);
            Assert.NotSame(context.Initial, context.Current);
            Assert.NotSame(entity, context.Current.Entity);
            Assert.Equal(entity.ToEntityReference(), context.Current.Entity.ToEntityReference());
            Assert.Equal(1250m, context.Current.Entity.GetAttributeValue<Money>("xts_attribute").Value);

            _service.Received(1).Retrieve(Arg.Is("entity"), Arg.Is(id), Arg.Any<ColumnSet>());
        }

        public static IEnumerable<object[]> GetReferenceNonCreateData()
        {
            yield return new object[] { PluginMessage.Update, "Target", false };
            yield return new object[] { PluginMessage.Delete, "Target", true };
            yield return new object[] { PluginMessage.Assign, "Target", true };
            yield return new object[] { PluginMessage.SetState, "EntityMoniker", true };
            yield return new object[] { PluginMessage.SetStateDynamicEntity, "EntityMoniker", true };
        }

        [Fact]
        public void Can_get_reference_on_update()
        {
            var test = new TestHelper();
            var id = Guid.NewGuid();
            var dbEntity = new Entity("entity") { Id = id };
            dbEntity.Set("new_money", new Money(1250m));
            dbEntity.Set("new_int", 234);
            test.Service.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(dbEntity);

            var entity = new Entity("entity")
            {
                Id = id,
                ["new_int"] = null
            };
            test.PluginExecutionContext.Stage.Returns((int) SdkMessageProcessingStepStage.Preoperation);
            test.PluginExecutionContext.MessageName.Returns(PluginMessage.Update);
            test.PluginExecutionContext.InputParameters["Target"] = entity;

            var plugin = Substitute.For<IPluginBase>();
            var container = Substitute.For<IContainer>();
            var config = new PluginConfiguration<Entity>(plugin, container)
            {
                ColumnSet = new ColumnSet("new_money")
            };
            var context = new TransactionContext<Entity>(test.ServiceProvider, config);

            var reference = context.Current.Entity;
            Assert.Equal(dbEntity.ToEntityReference(), reference.ToEntityReference());
            Assert.Equal(1250m, reference.Get<Money>("new_money").Value);
            Assert.Null(reference.Get<int?>("new_int"));
        }
    }
}
