using System;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.Configurations;
using Xunit;
using Niam.XRM.TestFramework;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class TransactionContextTests
    {
        [Fact]
        public void Register_input_reference_events()
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
            var input = context.Input;
            var reference = context.Reference;

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
            test.Db["INPUT-DB-001"] = dbEntity;

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
    }
}
