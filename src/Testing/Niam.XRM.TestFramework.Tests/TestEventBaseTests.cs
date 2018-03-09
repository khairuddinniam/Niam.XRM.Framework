using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.TestFramework.Tests
{
    public class TestEventBaseTests
    {
        [Fact]
        public void Can_execute_test_event()
        {
            var testEvent = new TestEvent
            {
                Plugin =
                {
                    UnsecureConfig = "unsecure",
                    SecureConfig = "secure"
                }
            };
            var context = testEvent.CreateTransactionContext();
            Assert.Equal("world", context.PluginExecutionContext.SharedVariables["hello"]);
            Assert.Equal("unsecure", context.Plugin.UnsecureConfig);
            Assert.Equal("secure", context.Plugin.SecureConfig);
        }

        private class TestEvent : TestEventBase
        {
            protected override void Apply()
            {
                PluginExecutionContext.SharedVariables["hello"] = "world";
            }
        }
        
        [Fact]
        public void Can_execute_plugin_with_default_constructor()
        {
            var plugin = new TestCrudEvent().ExecutePlugin<TestPluginDefaultConstructor>();

            Assert.Equal("execute-crm-plugin", plugin.Value);
        }

        private class TestPluginDefaultConstructor : PluginBase, IPlugin
        {
            public string Value { get; private set; }

            public TestPluginDefaultConstructor() : base(null, null)
            {
            }

            protected override void ExecuteCrmPlugin(ITransactionContext<Entity> context)
            {
                Value = "execute-crm-plugin";
            }
        }

        [Fact]
        public void Can_execute_plugin_with_configuration()
        {
            var testEvent = new TestEvent
            {
                Plugin =
                {
                    UnsecureConfig = "hello",
                    SecureConfig = "world"
                }
            };

            var plugin = testEvent.ExecutePlugin<TestPluginWithConfiguration>();
            Assert.Equal("hello", plugin.UnsecureConfig);
            Assert.Equal("world", plugin.SecureConfig);
        }

        private class TestPluginWithConfiguration : PluginBase, IPlugin
        {
            public TestPluginWithConfiguration(string unsecure, string secure) : base(unsecure, secure)
            {
            }

            protected override void ExecuteCrmPlugin(ITransactionContext<Entity> context)
            {
            }
        }

        [Fact]
        public void Invalid_when_executing_plugin_with_unknown_constructors()
        {
            var testEvent = new TestEvent();
            var ex = Assert.Throws<ArgumentException>(() => testEvent.ExecutePlugin<InvalidPlugin>());
            Assert.Equal("The plugin does not have constructor for passing in two configuration strings or constructor without arguments.", ex.Message);
        }

        private class InvalidPlugin : IPlugin
        {
            public InvalidPlugin(int number)
            {
            }

            public void Execute(IServiceProvider serviceProvider)
            {
            }
        }
    }
}
