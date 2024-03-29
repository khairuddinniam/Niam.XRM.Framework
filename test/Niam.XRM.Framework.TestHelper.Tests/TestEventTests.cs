﻿using System;
using FakeXrmEasy;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using FakeXrmEasy.Middleware.Pipeline;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.Framework.TestHelper.Tests
{
    public class TestEventTests
    {
        private IXrmFakedContext Context => (XrmFakedContext)MiddlewareBuilder
            .New()
            .AddCrud()
            .AddFakeMessageExecutors()
            .AddPipelineSimulation()
            .UseCrud()
            .UseMessages()
            .SetLicense(FakeXrmEasyLicense.NonCommercial)
            .Build();

        [Fact]
        public void Can_execute_plugin_with_default_constructor()
        {
            var plugin = new TestEvent(Context).ExecutePlugin<TestPluginDefaultConstructor>(new OrganizationRequest());

            Assert.Equal("execute-crm-plugin", plugin.Value);
        }

        private class TestPluginDefaultConstructor : PluginBase, IPlugin
        {
            public string Value { get; private set; }

            public TestPluginDefaultConstructor() : base(null, null)
            {
            }

            protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
            {
                Value = "execute-crm-plugin";
            }
        }

        [Fact]
        public void Can_execute_plugin_with_configuration()
        {
            var plugin = new TestEvent(Context).ExecutePlugin<TestPluginWithConfiguration>(
                "hello", "world", new OrganizationRequest());
            Assert.Equal("hello", plugin.UnsecureConfig);
            Assert.Equal("world", plugin.SecureConfig);
        }

        private class TestPluginWithConfiguration : PluginBase, IPlugin
        {
            public TestPluginWithConfiguration(string unsecure, string secure) : base(unsecure, secure)
            {
            }

            protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
            {
            }
        }

        [Fact]
        public void Invalid_when_executing_plugin_with_unknown_constructors()
        {
            var testEvent = new TestEvent(Context);
            var ex = Assert.Throws<ArgumentException>(() => testEvent.ExecutePlugin<InvalidPlugin>(new OrganizationRequest()));
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
