using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;

namespace Niam.XRM.Framework.TestHelper
{
    public class TestEvent : TestEvent<Entity>
    {
        public TestEvent(params Entity[] initialEntities) : base(initialEntities)
        {
        }
    }

    public partial class TestEvent<TE> where TE : Entity
    {
        private readonly TE[] _initialEntities;
        private readonly InternalXrmFakedContext _xrmFakedContext;

        public XrmFakedContext FakedContext => _xrmFakedContext;

        public XrmFakedPluginExecutionContext PluginExecutionContext { get; }

        public XrmFakedTracingService TracingService { get; }

        public TestDatabase Db => _xrmFakedContext.Db;

        public TestEvent(params TE[] initialEntities)
        {
            _initialEntities = initialEntities;
            _xrmFakedContext = new InternalXrmFakedContext();

            PluginExecutionContext = FakedContext.GetDefaultPluginContext();

            TracingService = FakedContext.GetFakeTracingService();
        }
        
        public TCommand ExecuteCommand<TCommand>(
            OrganizationRequest request, int? stage = null, Action<IPluginConfiguration<TE>> configure = null) 
            where TCommand : ICommand =>
            ExecuteCommand<TCommand>(null, null, request, stage);

        public TCommand ExecuteCommand<TCommand>(
            string unsecure, string secure, 
            OrganizationRequest request, int? stage = null, Action<IPluginConfiguration<TE>> configure = null) 
            where TCommand : ICommand
        {
            var plugin = CreatePlugin<TestPluginWithCommand<TE, TCommand>>(unsecure, secure);
            ExecutePlugin(plugin, request, stage);
            return plugin.Command;
        }
        
        public TPlugin ExecutePlugin<TPlugin>(OrganizationRequest request, int? stage = null) 
            where TPlugin : IPlugin =>
            ExecutePlugin<TPlugin>(null, null, request, stage);

        public TPlugin ExecutePlugin<TPlugin>(
            string unsecure, string secure, OrganizationRequest request, int? stage = null) where TPlugin : IPlugin
        {
            return ExecutePlugin(CreatePlugin<TPlugin>(unsecure, secure), request, stage);
        }
        
        private  TPlugin ExecutePlugin<TPlugin>(
            TPlugin plugin, OrganizationRequest request, int? stage = null) where TPlugin : IPlugin
        {
            PluginExecutionContext.SetRequest(request);
            PluginExecutionContext.Stage = stage ?? 20;
            
            PrepareXrmFakedContext();
            var serviceProvider = _xrmFakedContext.CreateServiceProvider(PluginExecutionContext);
            plugin.Execute(serviceProvider);
            return plugin;
        }

        private void PrepareXrmFakedContext()
        {
            FakedContext.Initialize(_initialEntities);
            var isCustomEarlyBound = typeof(TE).GetProperty("Id")?.GetCustomAttribute<ColumnAttribute>() != null;
            if (isCustomEarlyBound)
            {
                _xrmFakedContext.IsCustomEarlyBound = true;
                FakedContext.ProxyTypesAssembly = null;
            }
        }

        private TP CreatePlugin<TP>(string unsecure, string secure) where TP : IPlugin
        {
            var pluginType = typeof(TP);
            var constructors = pluginType.GetConstructors();

            var constructorWithConfiguration = constructors.FirstOrDefault(c => 
                c.GetParameters().Length == 2 && 
                c.GetParameters().All(param => param.ParameterType == typeof(string))
            );
            if (constructorWithConfiguration != null)
                return (TP) constructorWithConfiguration.Invoke(new object[] { unsecure, secure });

            var defaultConstructor = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
            if (defaultConstructor != null)
                return (TP) defaultConstructor.Invoke(new object[0]);
            
            throw new ArgumentException("The plugin does not have constructor for passing in two configuration strings or constructor without arguments.");
        }

        private class InternalXrmFakedContext : XrmFakedContext
        {
            public TestDatabase Db { get; }
            
            public bool IsCustomEarlyBound { get; set; }

            public InternalXrmFakedContext()
            {
                Db = new TestDatabase(this);
            }

            public IServiceProvider CreateServiceProvider(XrmFakedPluginExecutionContext plugCtx)
                => GetFakedServiceProvider(plugCtx);

            public override IOrganizationService GetOrganizationService()
            {
                var service = IsCustomEarlyBound 
                    ? new ClearProxyOrganizationService(base.GetOrganizationService(), this)
                    : base.GetOrganizationService();
                return new TestOrganizationService(service, Db);
            }
        }
    }

    public partial class TestEvent<TE> where TE : Entity
    {
        public TPlugin CreateEvent<TPlugin>(TE target, int? stage = null) where TPlugin : IPlugin => 
            CreateEvent<TPlugin>(null, null, target, stage);
        
        public TPlugin CreateEvent<TPlugin>(string unsecure, string secure, TE target, int? stage = null)
            where TPlugin : IPlugin =>
            ExecutePlugin<TPlugin>(unsecure, secure, new CreateRequest { Target = target }, stage);
        
        public TPlugin UpdateEvent<TPlugin>(TE target, int? stage = null) where TPlugin : IPlugin => 
            UpdateEvent<TPlugin>(null, null, target, stage);
        
        public TPlugin UpdateEvent<TPlugin>(string unsecure, string secure, TE target, int? stage = null)
            where TPlugin : IPlugin =>
            ExecutePlugin<TPlugin>(unsecure, secure, new UpdateRequest { Target = target }, stage);
        
        public TPlugin DeleteEvent<TPlugin>(
            EntityReference target, int? stage = null) where TPlugin : IPlugin => 
            DeleteEvent<TPlugin>(null, null, target, stage);
        
        public TPlugin DeleteEvent<TPlugin>(
            string unsecure, string secure, EntityReference target, int? stage = null)
            where TPlugin : IPlugin =>
            ExecutePlugin<TPlugin>(unsecure, secure, new DeleteRequest { Target = target }, stage);
        
        public TCommand CreateEventCommand<TCommand>(
            TE target, int? stage = null, Action<IPluginConfiguration<TE>> configure = null) where TCommand : ICommand => 
            CreateEventCommand<TCommand>(null, null, target, stage, configure);
        
        public TCommand CreateEventCommand<TCommand>(
            string unsecure, string secure, 
            TE target, int? stage = null, Action<IPluginConfiguration<TE>> configure = null)
            where TCommand : ICommand =>
            ExecuteCommand<TCommand>(unsecure, secure, new CreateRequest { Target = target }, stage, configure);
        
        public TCommand UpdateEventCommand<TCommand>(
            TE target, int? stage = null, Action<IPluginConfiguration<TE>> configure = null) where TCommand : ICommand => 
            UpdateEventCommand<TCommand>(null, null, target, stage, configure);
        
        public TCommand UpdateEventCommand<TCommand>(
            string unsecure, string secure, 
            TE target, int? stage = null, Action<IPluginConfiguration<TE>> configure = null)
            where TCommand : ICommand =>
            ExecuteCommand<TCommand>(unsecure, secure, new UpdateRequest { Target = target }, stage, configure);
        
        public TCommand DeleteEventCommand<TCommand>(
            EntityReference target, int? stage = null, Action<IPluginConfiguration<TE>> configure = null) where TCommand : ICommand => 
            DeleteEventCommand<TCommand>(null, null, target, stage, configure);
        
        public TCommand DeleteEventCommand<TCommand>(
            string unsecure, string secure, 
            EntityReference target, int? stage = null, Action<IPluginConfiguration<TE>> configure = null)
            where TCommand : ICommand =>
            ExecuteCommand<TCommand>(unsecure, secure, new DeleteRequest { Target = target }, stage, configure);
    }
}
