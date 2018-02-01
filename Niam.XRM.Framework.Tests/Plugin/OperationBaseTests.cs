using Microsoft.Xrm.Sdk;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.Framework.Interfaces.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.TestFramework;
using Xunit;
using Niam.XRM.Framework.Plugin.Configurations;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Xunit.Abstractions;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class OperationBaseTests
    {
        private readonly ITestOutputHelper _output;

        public OperationBaseTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetCorrectConditionsForUpdate))]
        public void Can_access_wrapper(string message, int stage)
        {
            var pluginContext = Substitute.For<IPluginExecutionContext>();
            pluginContext.MessageName.Returns(message);
            pluginContext.Stage.Returns(stage);

            var referenceEntity = new Entity("reference");
            var reference = new TransactionContextEntity<Entity>(referenceEntity);
            var context = Substitute.For<ITransactionContext<Entity>>();
            context.PluginExecutionContext.Returns(pluginContext);
            context.Reference.Returns(reference);

            var operation = new Operation(context);
            Assert.Equal(referenceEntity, operation.PublicWrapper.Entity);
        }

        [Theory]
        [MemberData(nameof(GetCorrectConditionsForUpdate))]
        public void Can_access_update(string message, int stage)
        {
            var pluginContext = Substitute.For<IPluginExecutionContext>();
            pluginContext.MessageName.Returns(message);
            pluginContext.Stage.Returns(stage);

            var inputEntity = new Entity("input");
            var input = new TransactionContextEntity<Entity>(inputEntity);
            var context = Substitute.For<ITransactionContext<Entity>>();
            context.PluginExecutionContext.Returns(pluginContext);
            context.Input.Returns(input);

            var operation = new Operation(context);
            Assert.Equal(input, operation.PublicInput);
            var update = operation.PublicInput; // other retrieve.
            var temp = context.Received(2).Input;
        }

        [Theory]
        [MemberData(nameof(GetCorrectConditionsForUpdate))]
        public void Can_access_wrapper_generic(string message, int stage)
        {
            var pluginContext = Substitute.For<IPluginExecutionContext>();
            pluginContext.MessageName.Returns(message);
            pluginContext.Stage.Returns(stage);

            var referenceEntity = new xts_entity { Id = Guid.NewGuid() };
            var reference = new TransactionContextEntity<xts_entity>(referenceEntity);
            var context = Substitute.For<ITransactionContext<xts_entity>>();
            context.PluginExecutionContext.Returns(pluginContext);
            context.Reference.Returns(reference);

            Assert.Equal(referenceEntity.ToEntityReference(), ((ITransactionContext<Entity>) context).Reference.Entity.ToEntityReference());
            
            var operation = new WrapperOperation(context);
            Assert.Equal(referenceEntity.ToEntityReference(), operation.PublicWrapper.Entity.ToEntityReference());
        }

        public static IEnumerable<object[]> GetCorrectConditionsForUpdate()
        {
            var messages = new[]
            {
                PluginMessage.Create,
                PluginMessage.Update
            };

            var stages = new[]
            {
                (int)SdkMessageProcessingStepStage.Prevalidation,
                (int)SdkMessageProcessingStepStage.Preoperation
            };

            return from message in messages
                   from stage in stages
                   select new object[] { message, stage };
        }

        private class Operation : OperationBase
        {
            public IEntityWrapper<Entity> PublicWrapper => Wrapper;
            public ITransactionContextEntity<Entity> PublicInput => Context.Input;

            public Operation(ITransactionContext<Entity> context) : base(context)
            {
            }

            protected override void HandleExecute()
            {
            }
        }

        private class WrapperOperation : OperationBase<xts_entity, Wrapper<xts_entity>>
        {
            public Wrapper<xts_entity> PublicWrapper => Wrapper;

            public WrapperOperation(ITransactionContext<xts_entity> context) : base(context)
            {
            }

            protected override void HandleExecute()
            {
            }
        }

        private class Wrapper<T> : EntityWrapper<T>
            where T : xts_entity, new()
        {
            public Wrapper(T entity, ITransactionContextBase context)
                : base(entity, context)
            {
            }
        }

        [Fact]
        public void Can_execute_methods_in_operation_base()
        {
            var test = new TestHelper();
            var reference = new xts_entity { Id = Guid.NewGuid() };
            var context = test.CreateTransactionContext<xts_entity>();
            var txReference = new TransactionContextEntity<xts_entity>(reference);
            context.Reference.Returns(txReference);

            new MethodOperation(context).Execute();
            var input = context.Input;
            Assert.Equal(1234m, input.Get(e => e.xts_money).Value);
            Assert.Equal(3000m, input.Get(e => e.xts_othermoney).Value);
            Assert.Equal(11, input.Get(e => e.xts_optionsetvalue).Value);
            Assert.Equal(45, input.Get(e => e.xts_otheroptionsetvalue).Value);
            Assert.Equal("HELLO", input.Get(e => e.xts_string));
            Assert.Equal(77, input.Get(e => e.xts_int));
            Assert.Equal(566f, input.Get(e => e.xts_float));
            Assert.Equal(88, input.Get(e => e.xts_otherint));
            Assert.Equal("WORLD", input.Get(e => e.xts_otherstring));
            Assert.Equal(7890m, input.Get(e => e.xts_totalmoney).Value);
        }

        private class MethodOperation : OperationBase<xts_entity, Wrapper<xts_entity>>
        {
            public MethodOperation(ITransactionContext<xts_entity> context)
                : base(context)
            {
            }

            protected override void HandleExecute()
            {
                Set(e => e.xts_money, 1234m);
                var decimalValueProvider = Substitute.For<IValueProvider<decimal>>();
                decimalValueProvider.GetValue().Returns(3000m);
                Set(e => e.xts_othermoney, decimalValueProvider);
                Set(e => e.xts_optionsetvalue, EnumTest.Eleven);
                Set(e => e.xts_otheroptionsetvalue, 45);

                var stringAttributeValueProvider = Substitute.For<IAttributeValueProvider>();
                stringAttributeValueProvider.GetValueFor(Arg.Any<string>(), Arg.Any<string>()).Returns("HELLO");
                Set("xts_string", stringAttributeValueProvider);

                var intValueProvider = Substitute.For<IValueProvider<int>>();
                ((IValueProvider) intValueProvider).GetValue().Returns(77);
                Set("xts_int", intValueProvider);

                object floatValue = 566f;
                Set("xts_float", floatValue);

                var otherIntValueProvider = Substitute.For<IValueProvider<int>>();
                otherIntValueProvider.GetValue().Returns(88);
                Set(e => e.xts_otherint, otherIntValueProvider);

                var otherStringAttributeValueProvider = Substitute.For<IAttributeValueProvider<xts_entity, string>>();
                otherStringAttributeValueProvider.GetValueFor(Arg.Any<Expression<Func<xts_entity, string>>>()).Returns("WORLD");
                Set(e => e.xts_otherstring, otherStringAttributeValueProvider);

                var moneyValueProvider = Substitute.For<IValueProvider<Money>>();
                moneyValueProvider.GetValue().Returns(new Money(7890m));
                Set(e => e.xts_totalmoney, moneyValueProvider);
            }
        }

        [Theory]
        [InlineData(PluginLogOption.Crm)]
        [InlineData(PluginLogOption.File)]
        public void Can_log_operation_execution(PluginLogOption pluginLogOption)
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

            new Operation(context).Execute();
            Assert.StartsWith("Entered", list[0]);
            Assert.StartsWith("Exiting", list[1]);
        }

        private enum EnumTest
        {
            Eleven = 11,
            Twelve = 12,
            Thirteen = 13
        }
    }
}
