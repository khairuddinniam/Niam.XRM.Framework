using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.Configurations;
using Xunit;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class TransactionContextTests
    {
        [Fact]
        public void Register_target_current_events()
        {
            var pluginContext = Substitute.For<IPluginExecutionContext>();
            pluginContext.MessageName.Returns(PluginMessage.Create);
            pluginContext.Stage.Returns((int)SdkMessageProcessingStepStage.Preoperation);

            pluginContext.InputParameters.Returns(new ParameterCollection());
            pluginContext.InputParameters["Target"] = new xts_entity { Id = Guid.NewGuid() };

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Any<Type>()).Returns(ci =>
            {
                var type = ci.ArgAt<Type>(0);
                if (type == typeof (IPluginExecutionContext))
                    return pluginContext;

                return null;
            });

            var context = serviceProvider.ToTransactionContext<xts_entity>();
            var input = context.Target;
            var reference = context.Current;

            // Copy from input to reference.
            input.Set(e => e.xts_money, new Money(120m));
            Assert.Equal(120m, reference.Get(e => e.xts_money).Value);
        }

        [Fact]
        public void Can_get_plugin_from_config()
        {
            var plugin = Substitute.For<IPluginBase>();
            var container = Substitute.For<IContainer>();
            var config = new PluginConfiguration<Entity>(plugin, container);

            var serviceProvider = Substitute.For<IServiceProvider>();
            var txContext = new TransactionContext<Entity>(serviceProvider, config);
            Assert.Same(plugin, txContext.Plugin);
        }

        [Fact]
        public void Can_access_tracing()
        {
            string message = null;
            object[] arguments = null;
            var tracingService = Substitute.For<ITracingService>();
            tracingService.When(s => s.Trace(Arg.Any<string>(), Arg.Any<object[]>()))
                .Do(ci =>
                {
                    message = ci.ArgAt<string>(0);
                    arguments = ci.ArgAt<object[]>(1);
                });
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Is(typeof(ITracingService))).Returns(tracingService);

            var txContext = new TransactionContext<Entity>(serviceProvider);
            Assert.Same(tracingService, txContext.TracingService);
            txContext.Trace("{0}-{1}", "one", "two");
            Assert.Equal("{0}-{1}", message);
            Assert.Equal(new object[] { "one", "two" }, arguments);
        }

        [Fact]
        public void Can_access_session_storage()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var txContext = new TransactionContext<Entity>(serviceProvider);
            txContext.SessionStorage["TEST"] = 1234;
            Assert.Equal(1234, txContext.SessionStorage["TEST"]);
        }

        [Fact]
        public void Can_access_initial_data()
        {
            var test = new TestHelper();
            var input = new xts_entity
            {
                Id = Guid.NewGuid()
            };

            var dbEntity = new xts_entity
            {
                Id = input.Id
            };
            dbEntity.Set(e => e.xts_string, "1234");
            dbEntity.FormattedValues["xts_optionsetvalue"] = "Hello";
            test.Service.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(dbEntity);

            test.PluginExecutionContext.MessageName.Returns(PluginMessage.Update);
            test.PluginExecutionContext.InputParameters["Target"] = input.ToEntity<Entity>();

            var plugin = Substitute.For<IPluginBase>();
            var container = Substitute.For<IContainer>();
            var config = new PluginConfiguration<xts_entity>(plugin, container)
            {
                ColumnSet = new ColumnSet<xts_entity>(e => e.xts_string, e => e.xts_optionsetvalue)
            };

            ITransactionContext<xts_entity> context = new TransactionContext<xts_entity>(test.ServiceProvider, config);
            Assert.Equal("Hello", context.Initial.GetFormattedValue("xts_optionsetvalue"));
        }

        [Theory]
        [MemberData(nameof(GetContextCurrentOnlyFromTargetTestData))]
        public void Context_current_only_from_target(ColumnSet columnSet)
        {
            var testHelper = new TestHelper();
            var pluginContext = testHelper.PluginExecutionContext;
            pluginContext.MessageName.Returns(PluginMessage.Update);
            pluginContext.Stage.Returns((int) SdkMessageProcessingStepStage.Preoperation);

            var target = new Entity("lead")
            {
                Id = Guid.NewGuid(),
                ["new_number"] = 1,
                ["new_optionset"] = new OptionSetValue(12),
                ["new_money"] = new Money(1234m)
            };
            pluginContext.InputParameters.Returns(new ParameterCollection());
            pluginContext.InputParameters["Target"] = target;

            var config = new PluginConfiguration<Entity>
            {
                ColumnSet = columnSet
            };
            var context = testHelper.ServiceProvider.ToTransactionContext(config);

            testHelper.Service.DidNotReceive().Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            Assert.Equal(target.Id, context.Current.Id);
            Assert.Equal(target["new_number"], context.Current["new_number"]);
            Assert.Equal(target["new_optionset"], context.Current["new_optionset"]);
            Assert.Equal(target["new_money"], context.Current["new_money"]);
        }
        
        public static IEnumerable<object[]> GetContextCurrentOnlyFromTargetTestData()
        {
            yield return new object[] { new ColumnSet() };
            yield return new object[] { new ColumnSet("new_optionset", "new_money") };
            yield return new object[] { new ColumnSet("new_optionset", "new_money", "new_number") };
        }

        [Theory]
        [MemberData(nameof(Get_context_current_is_initial_merged_with_target_TestData))]
        public void Context_current_is_initial_merged_with_target(ColumnSet configColumnSet)
        {
            var testHelper = new TestHelper();
            var pluginContext = testHelper.PluginExecutionContext;
            pluginContext.MessageName.Returns(PluginMessage.Update);
            pluginContext.Stage.Returns((int) SdkMessageProcessingStepStage.Preoperation);

            var id = Guid.NewGuid();
            var target = new Entity("lead")
            {
                Id = id,
                ["new_number"] = 1
            };
            pluginContext.InputParameters.Returns(new ParameterCollection());
            pluginContext.InputParameters["Target"] = target;

            var initial = new Entity("lead")
            {
                Id = id,
                ["new_number"] = 15,
                ["new_optionset"] = new OptionSetValue(12),
                ["new_money"] = new Money(1234m)
            };
            testHelper.Service.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>()).Returns(initial);

            var config = new PluginConfiguration<Entity>
            {
                ColumnSet = configColumnSet
            };
            var context = testHelper.ServiceProvider.ToTransactionContext(config);

            Assert.Equal(target.Id, context.Current.Id);
            Assert.Equal(target["new_number"], context.Current["new_number"]);
            Assert.Equal(initial["new_optionset"], context.Current["new_optionset"]);
            Assert.Equal(initial["new_money"], context.Current["new_money"]);
        }

        public static IEnumerable<object[]> Get_context_current_is_initial_merged_with_target_TestData()
        {
            yield return new object[] { new ColumnSet(true) };
            yield return new object[] { new ColumnSet("new_optionset", "new_money", "new_number") };
        }
    }
}
