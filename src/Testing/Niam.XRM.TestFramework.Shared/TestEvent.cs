using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;

namespace Niam.XRM.TestFramework
{
    public class TestEvent : TestEvent<Entity>
    {
    }

    public class TestEvent<TE> where TE : Entity
    {
        private readonly InternalXrmFakedContext _xrmFakedContext;

        public XrmFakedContext FakedContext => _xrmFakedContext;

        public TestPlugin<TE> Plugin { get; }

        public XrmFakedPluginExecutionContext PluginExecutionContext { get; }

        public XrmFakedTracingService TracingService { get; }

        public List<Entity> Db { get; }

        public TestEvent()
        {
            _xrmFakedContext = new InternalXrmFakedContext();

            Plugin = new TestPlugin<TE>();

            PluginExecutionContext = FakedContext.GetDefaultPluginContext();

            TracingService = FakedContext.GetFakeTracingService();

            Db = new List<Entity>();
        }

        public void ForRequest(OrganizationRequest request, int? stage = null)
        {
            PluginExecutionContext.SetRequest(request);
            PluginExecutionContext.Stage = stage ?? 20;
        }
        
        public IPluginContext<TE> CreatePluginContext()
        {
            var plugin = ExecutePlugin(CreateTestPlugin);
            return plugin.PluginContext;
        }

        private TestPlugin CreateTestPlugin()
           => new TestPlugin(Plugin.Configure, Plugin.UnsecureConfig, Plugin.SecureConfig);

        public TP ExecutePlugin<TP>(Func<TP> pluginFactory = null) where TP : IPlugin
        {
            PrepareXrmFakedContext();
            var plugin = (pluginFactory ?? CreatePlugin<TP>).Invoke();
            var serviceProvider = _xrmFakedContext.CreateServiceProvider(PluginExecutionContext);
            plugin.Execute(serviceProvider);
            return plugin;
        }

        private void PrepareXrmFakedContext()
        {
            FakedContext.Initialize(Db);
            var isCustomEarlyBound = typeof(TE).GetProperty("Id")?.GetCustomAttribute<ColumnAttribute>() != null;
            if (isCustomEarlyBound)
                FakedContext.ProxyTypesAssembly = null;
        }

        private TP CreatePlugin<TP>() where TP : IPlugin
        {
            var pluginType = typeof(TP);
            var constructors = pluginType.GetConstructors();

            var constructorWithConfiguration = constructors.FirstOrDefault(c => 
                c.GetParameters().Length == 2 && 
                c.GetParameters().All(param => param.ParameterType == typeof(string))
            );
            if (constructorWithConfiguration != null)
                return (TP) constructorWithConfiguration.Invoke(new object[] { Plugin.UnsecureConfig, Plugin.SecureConfig });

            var defaultConstructor = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
            if (defaultConstructor != null)
                return (TP) defaultConstructor.Invoke(new object[0]);
            
            throw new ArgumentException("The plugin does not have constructor for passing in two configuration strings or constructor without arguments.");
        }

        private class TestPlugin : PluginBase<TE>, IPlugin
        {
            private readonly Action<IPluginConfiguration<TE>> _configurePlugin;

            public IPluginContext<TE> PluginContext { get; private set; }
            
            public TestPlugin(
                Action<IPluginConfiguration<TE>> configurePlugin,
                string unsecure, string secure) : base(unsecure, secure)
            {
                _configurePlugin = configurePlugin;
            }

            protected override void Configure(IPluginConfiguration<TE> config)
            {
                _configurePlugin?.Invoke(config);
            }

            protected override void ExecuteCrmPlugin(IPluginContext<TE> context)
            {
                PluginContext = context;
            }
        }

        private class InternalXrmFakedContext : XrmFakedContext
        {
            public IServiceProvider CreateServiceProvider(XrmFakedPluginExecutionContext plugCtx)
                => GetFakedServiceProvider(plugCtx);
        }
    }
}
