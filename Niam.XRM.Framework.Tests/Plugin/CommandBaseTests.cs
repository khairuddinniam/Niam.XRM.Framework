using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Xunit;
using Niam.XRM.TestFramework;
using System.Linq;
using System.Linq.Expressions;
using Niam.XRM.Framework.Interfaces.Data;
using Xunit.Abstractions;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class CommandBaseTests
    {
        private readonly ITestOutputHelper _output;

        public CommandBaseTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Can_create_command()
        {
            var context = Substitute.For<ITransactionContext<Entity>>();
            var reference = new Entity("entity", Guid.NewGuid());
            var txReference = new TransactionContextEntity<Entity>(reference);
            context.Reference.Returns(txReference);
            var input = new Entity("entity", reference.Id);
            var txInput = new TransactionContextEntity<Entity>(input);
            context.Input.Returns(txInput);

            var command = new Command(context);
            Assert.NotNull(command.PublicWrapper);
            Assert.Same(reference, command.PublicWrapper.Entity);
        }

        [Fact]
        public void Can_create_command_generic()
        {
            var context = Substitute.For<ITransactionContext<xts_entity>>();

            var reference = new xts_entity { Id = Guid.NewGuid() };
            var txReference = new TransactionContextEntity<xts_entity>(reference);
            context.Reference.Returns(txReference);
            Assert.Same(reference, context.Reference.Entity);

            var input = new xts_entity { Id = reference.Id };
            var txInput = new TransactionContextEntity<xts_entity>(input);
            context.Input.Returns(txInput);

            var command = new CommandGeneric(context);
            Assert.NotNull(command.PublicWrapper);
            Assert.Same(reference, command.PublicWrapper.Entity);
        }

        private class CommandGeneric : CommandBase<xts_entity, EntityWrapper<xts_entity>>
        {
            public EntityWrapper<xts_entity> PublicWrapper => Wrapper;

            public CommandGeneric(ITransactionContext<xts_entity> context) : base(context)
            {
            }

            protected override void HandleExecuteCore()
            {
            }
        }

        [Fact]
        public void Can_execute_command_methods()
        {
            var test = new TestHelper();
            var db = test.Db;

            var related = new xts_relatedentity {Id = Guid.NewGuid()};
            related.Set(e => e.xts_name, "RELATED");
            db["RELATED-001"] = related;

            var reference = new xts_entity {Id = Guid.NewGuid()};
            reference.Set(e => e.xts_optionsetvalue, 12);
            reference.Set(e => e.xts_int, 345);
            reference.Set(e => e.xts_string, "PARENT");
            reference.Set(e => e.xts_referenceid, related.ToEntityReference());
            reference.Set(e => e.xts_money, new Money(1234m));

            reference.SetFormattedValue(e => e.xts_attribute, "Hello Formatted Value");
            reference.SetFormattedValue("xts_anotherattribute", "1234-another-fv");

            var child = new xts_relatedentity {Id = Guid.NewGuid()};
            child.Set(e => e.xts_name, "CHILD");
            child.Set(e => e.xts_relatedid, reference.ToEntityReference());
            db["CHILD-001"] = child;

            var context = test.CreateTransactionContext<xts_entity>();
            var txReference = new TransactionContextEntity<xts_entity>(reference);
            context.Reference.Returns(txReference);

            var commandTest = new CommandTest(context);
            commandTest.ExecuteTest();

            IEntityWrapperRelation<xts_entity> wrapper = commandTest;

            var relatedColumnSet = new ColumnSet<xts_relatedentity>(
                e => e.Id,
                e => e.xts_name
            );

            var relatedGeneric = wrapper.GetRelated(e => e.xts_referenceid, relatedColumnSet);
            Assert.Equal("RELATED", relatedGeneric.Get(e => e.xts_name));
        }

        public static IEnumerable<object[]> GetChangeOnMoneyData()
        {
            yield return new object[]
            {
                new Money(50m), new Money(75m), false  
            };
            yield return new object[]
            {
                new Money(100m), new Money(75m), false
            };
            yield return new object[]
            {
                new Money(50m), new Money(250m), false
            };
            yield return new object[]
            {
                new Money(100m), new Money(250m), true
            };
            yield return new object[]
            {
                null, null, false
            };
        }

        [Theory]
        [MemberData(nameof(GetChangeOnMoneyData))]
        public void Change_on_money(Money from, Money to, bool expected)
        {
            var test = new TestHelper();
            var id = Guid.NewGuid();
            var initial = new xts_entity { Id = id };
            initial.Set(e => e.xts_money, 100m);

            var reference = new xts_entity { Id = id };
            reference.Set(e => e.xts_money, 250m);

            var context = test.CreateTransactionContext<xts_entity>();
            context.Initial.Returns(initial.ToEntityAccessor());
            context.Reference.Returns(new TransactionContextEntity<xts_entity>(reference));

            var command = new CommandTest(context);
            var actual = command.ChangeTest(e => e.xts_money, from, to);
            Assert.Equal(expected, actual);
            actual = command.ChangeTest("xts_money", from, to);
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> GetChangeOnMoneyDecimalData()
        {
            yield return new object[]
            {
                50m, 75m, false
            };
            yield return new object[]
            {
                100m, 75m, false
            };
            yield return new object[]
            {
                50m, 250m, false
            };
            yield return new object[]
            {
                100m, 250m, true
            };
            yield return new object[]
            {
                null, null, false
            };
        }

        [Theory]
        [MemberData(nameof(GetChangeOnMoneyDecimalData))]
        public void Change_on_money_decimal(decimal? from, decimal? to, bool expected)
        {
            var test = new TestHelper();
            var id = Guid.NewGuid();
            var initial = new xts_entity { Id = id };
            initial.Set(e => e.xts_money, 100m);

            var reference = new xts_entity { Id = id };
            reference.Set(e => e.xts_money, 250m);

            var context = test.CreateTransactionContext<xts_entity>();
            context.Initial.Returns(initial.ToEntityAccessor());
            context.Reference.Returns(new TransactionContextEntity<xts_entity>(reference));

            var command = new CommandTest(context);
            var actual = command.ChangeTest(e => e.xts_money, from, to);
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> GetChangeOnOptionSetValueData()
        {
            yield return new object[]
            {
                new OptionSetValue(2), new OptionSetValue(4), false
            };
            yield return new object[]
            {
                new OptionSetValue(1), new OptionSetValue(4), false
            };
            yield return new object[]
            {
                new OptionSetValue(2), new OptionSetValue(3), false
            };
            yield return new object[]
            {
                new OptionSetValue(1), new OptionSetValue(3), true
            };
            yield return new object[]
            {
                null, null, false
            };
        }

        [Theory]
        [MemberData(nameof(GetChangeOnOptionSetValueData))]
        public void Change_on_option_set_value(OptionSetValue from, OptionSetValue to, bool expected)
        {
            var test = new TestHelper();
            var id = Guid.NewGuid();
            var initial = new xts_entity { Id = id };
            initial.Set(e => e.xts_optionsetvalue, new OptionSetValue(1));

            var reference = new xts_entity { Id = id };
            reference.Set(e => e.xts_optionsetvalue, new OptionSetValue(3));

            var context = test.CreateTransactionContext<xts_entity>();
            context.Initial.Returns(initial.ToEntityAccessor());
            context.Reference.Returns(new TransactionContextEntity<xts_entity>(reference));

            var command = new CommandTest(context);
            var actual = command.ChangeTest(e => e.xts_optionsetvalue, from, to);
            Assert.Equal(expected, actual);
            actual = command.ChangeTest("xts_optionsetvalue", from, to);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(CommandEnumTest.Twelve, CommandEnumTest.Fourteen, false)]
        [InlineData(CommandEnumTest.Eleven, CommandEnumTest.Fourteen, false)]
        [InlineData(CommandEnumTest.Twelve, CommandEnumTest.Thirteen, false)]
        [InlineData(CommandEnumTest.Eleven, CommandEnumTest.Thirteen, true)]
        [InlineData(null, null, false)]
        public void Change_on_option_set_value_enum(CommandEnumTest? from, CommandEnumTest? to, bool expected)
        {
            var test = new TestHelper();
            var id = Guid.NewGuid();
            var initial = new xts_entity { Id = id };
            initial.Set(e => e.xts_optionsetvalue, CommandEnumTest.Eleven);

            var reference = new xts_entity { Id = id };
            reference.Set(e => e.xts_optionsetvalue, CommandEnumTest.Thirteen);

            var context = test.CreateTransactionContext<xts_entity>();
            context.Initial.Returns(initial.ToEntityAccessor());
            context.Reference.Returns(new TransactionContextEntity<xts_entity>(reference));

            var command = new CommandTest(context);
            var actual = command.ChangeTest(e => e.xts_optionsetvalue, from, to);
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> GetChangeOnEntityReferenceData()
        {
            var from = new EntityReference("entity", new Guid("e6ed2acb-8f66-44c9-847b-5897a2120147"));
            var to = new EntityReference("entity", new Guid("c78e077f-81d1-4791-853c-257e47580c86"));

            yield return new object[]
            {
                new EntityReference("entity", Guid.NewGuid()), new EntityReference("entity", Guid.NewGuid()), false
            };
            yield return new object[]
            {
                from, new EntityReference("entity", Guid.NewGuid()), false
            };
            yield return new object[]
            {
                new EntityReference("entity", Guid.NewGuid()), to, false
            };
            yield return new object[]
            {
                from, to, true
            };
            yield return new object[]
            {
                null, null, false
            };
        }

        [Theory]
        [MemberData(nameof(GetChangeOnEntityReferenceData))]
        public void Change_on_entity_reference(EntityReference from, EntityReference to, bool expected)
        {
            var expectedFrom = new EntityReference("entity", new Guid("e6ed2acb-8f66-44c9-847b-5897a2120147"));
            var expectedTo = new EntityReference("entity", new Guid("c78e077f-81d1-4791-853c-257e47580c86"));
            var test = new TestHelper();
            var id = Guid.NewGuid();
            var initial = new xts_entity { Id = id };
            initial.Set(e => e.xts_attribute, expectedFrom);

            var reference = new xts_entity { Id = id };
            reference.Set(e => e.xts_attribute, expectedTo);

            var context = test.CreateTransactionContext<xts_entity>();
            context.Initial.Returns(initial.ToEntityAccessor());
            context.Reference.Returns(new TransactionContextEntity<xts_entity>(reference));

            var command = new CommandTest(context);
            var actual = command.ChangeTest(e => e.xts_attribute, from, to);
            Assert.Equal(expected, actual);
            actual = command.ChangeTest("xts_attribute", from, to);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_get_name()
        {
            var test = new TestHelper();
            var keyEntity = new xts_keytest { Id = Guid.NewGuid() };
            keyEntity.Set(e => e.xts_key, "Hello world");
            test.Db["ENTITY"] = keyEntity;

            var context = test.CreateTransactionContext<xts_entity>();
            var entity = new xts_entity { Id = Guid.NewGuid() };
            entity.Set(e => e.xts_referenceid, keyEntity.ToEntityReference());
            context.Reference.Returns(new TransactionContextEntity<xts_entity>(entity));

            var command = new CommandTest(context);
            Assert.Equal("Hello world", command.GetNameTest<xts_keytest>(e => e.xts_referenceid));
        }

        [Fact]
        public void Can_execute_events()
        {
            var logs = new List<string>();
            var tracingService = Substitute.For<ITracingService>();
            tracingService.When(s => s.Trace(Arg.Any<string>(), Arg.Any<object[]>()))
                .Do(ci => {
                    var format = ci.ArgAt<string>(0);
                    var args = ci.ArgAt<object[]>(1);
                    string log = String.Format(format, args);
                    _output.WriteLine(log);
                    logs.Add(log);
                });

            var context = Substitute.For<ITransactionContext<xts_entity>>();
            context.TracingService.Returns(tracingService);

            Assert.ThrowsAny<Exception>(() => new Command(context).Execute());
            Assert.Equal("OnExecuting", logs[0]);
            Assert.Equal("Hello Error", logs[1]);
            Assert.Equal("OnExecuted", logs[2]);
        }

        public enum CommandEnumTest
        {
            Eleven = 11,
            Twelve = 12,
            Thirteen = 13,
            Fourteen = 14
        }

        private class Command : CommandBase<Entity, EntityWrapper<Entity>>
        {
            public EntityWrapper<Entity> PublicWrapper => Wrapper;

            public Command(ITransactionContext<Entity> context) : base(context)
            {
                OnExecuting += () => Context.Trace("OnExecuting");
                OnExecutionError += ex => Context.Trace(ex.Message);
                OnExecuted += () => Context.Trace("OnExecuted");
            }

            public void Execute() => ExecuteCore();

            protected override void HandleExecuteCore()
            {
                throw new InvalidOperationException("Hello Error");
            }
        }

        private class CommandTest : CommandBase<xts_entity, EntityWrapper<xts_entity>>
        {
            public CommandTest(ITransactionContext<xts_entity> context) : base(context)
            {
            }
            
            protected override void HandleExecuteCore()
            {
            }

            public bool ChangeTest<TV>(Expression<Func<xts_entity, TV>> attribute, TV from, TV to)
                => Changed(attribute, from, to);

            public bool ChangeTest<TV>(string attribute, TV from, TV to)
                => Changed(attribute, from, to);

            public bool ChangeTest(Expression<Func<xts_entity, Money>> attribute, decimal? from, decimal? to)
                => Changed(attribute, from, to);

            public bool ChangeTest(Expression<Func<xts_entity, OptionSetValue>> attribute, Enum from, Enum to)
                => Changed(attribute, from, to);

            public void ExecuteTest()
            {
                Assert.NotNull(Context);
                Assert.NotNull(Initial);
                Assert.NotNull(Service);
                Assert.NotNull(Wrapper);

                Assert.True(Equal(e => e.xts_optionsetvalue, CommandEnumTest.Twelve));
                Assert.True(Equal(e => e.xts_int, 345));
                Assert.True(Equal("xts_optionsetvalue", new OptionSetValue(12)));
                Assert.True(Equal("xts_int", 345));
                Assert.True(EqualsAny(e => e.xts_optionsetvalue, CommandEnumTest.Thirteen, CommandEnumTest.Twelve, CommandEnumTest.Eleven));
                Assert.Equal("PARENT", Get(e => e.xts_string));
                Assert.Equal("PARENT", Get<string>("xts_string"));
                Assert.Equal(1234m, GetValue(e => e.xts_money));
                Assert.Equal(345, GetValue(e => e.xts_int));

                Assert.Equal("Hello Formatted Value", GetFormattedValue(e => e.xts_attribute));
                Assert.Equal("1234-another-fv", GetFormattedValue("xts_anotherattribute"));

                AssertRelated();
                AssertAllRelated();
            }

            private void AssertRelated()
            {
                var relatedColumnSet = new ColumnSet<xts_relatedentity>(
                    e => e.Id,
                    e => e.xts_name
                );

                var relatedGeneric = GetRelated(e => e.xts_referenceid, relatedColumnSet);
                Assert.Equal("RELATED", relatedGeneric.Get(e => e.xts_name));

                var relatedWrapper = GetRelated<xts_relatedentity, EntityWrapper<xts_relatedentity>>(e => e.xts_referenceid, relatedColumnSet);
                Assert.Equal("RELATED", relatedWrapper.Get(e => e.xts_name));

                var relatedString = GetRelated("xts_referenceid", relatedColumnSet);
                Assert.Equal("RELATED", relatedString.Get<string>("xts_name"));
            }

            private void AssertAllRelated()
            {
                var childColumnSet = new ColumnSet<xts_relatedentity>(
                    e => e.Id,
                    e => e.xts_name
                );

                var childrenGenerics = GetAllRelated<xts_relatedentity>(e => e.xts_relatedid, childColumnSet).ToArray();
                Assert.Single(childrenGenerics);
                Assert.Equal("CHILD", childrenGenerics[0].Get(e => e.xts_name));

                var childrenWrappers = GetAllRelated<xts_relatedentity, EntityWrapper<xts_relatedentity>>(e => e.xts_relatedid, childColumnSet).ToArray();
                Assert.Single(childrenWrappers);
                Assert.Equal("CHILD", childrenWrappers[0].Get(e => e.xts_name));

                var childrenStrings = GetAllRelated(xts_relatedentity.EntityLogicalName, "xts_relatedid", childColumnSet).ToArray();
                Assert.Single(childrenStrings);
                Assert.Equal("CHILD", childrenStrings[0].Get<string>("xts_name"));
            }

            public string GetNameTest<TR>(Expression<Func<xts_entity, EntityReference>> relatedAttribute)
                where TR : Entity
                => GetName<TR>(relatedAttribute);
        }
    }
}
