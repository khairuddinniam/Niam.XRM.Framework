using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Abstractions.Plugins;
using FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk.Metadata;

namespace Niam.XRM.Framework.TestHelper
{
    public class TestEvent : TestEvent<Entity>
    {
        public TestEvent(IXrmFakedContext xrmFakedContext, params Entity[] initialEntities) : base(xrmFakedContext, initialEntities)
        {
        }
    }

    public partial class TestEvent<TE> where TE : Entity
    {
        private readonly Entity[] _initialEntities;
        private readonly InternalXrmFakedContext _xrmFakedContext;

        public IXrmFakedContext FakedContext => _xrmFakedContext;

        public XrmFakedPluginExecutionContext PluginExecutionContext { get; }

        public IXrmFakedTracingService TracingService { get; }

        public TestDatabase Db => _xrmFakedContext.Db;

        public TestEvent(IXrmFakedContext xrmFakedContext, params Entity[] initialEntities)
        {
            _initialEntities = initialEntities.Select(e => e.ToEntity<Entity>()).ToArray();

            _xrmFakedContext = new InternalXrmFakedContext(xrmFakedContext);
            PluginExecutionContext = FakedContext.GetDefaultPluginContext();
            TracingService = FakedContext.GetTracingService();
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

        private TPlugin ExecutePlugin<TPlugin>(
            TPlugin plugin, OrganizationRequest request, int? stage = null) where TPlugin : IPlugin
        {
            PluginExecutionContext.SetRequest(request);
            PluginExecutionContext.Stage = stage ?? 20;

            PrepareXrmFakedContext();
            _xrmFakedContext.ExecutePluginWith(PluginExecutionContext, plugin);
            return plugin;
        }

        private void PrepareXrmFakedContext()
        {
            FakedContext.Initialize(_initialEntities);
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
                return (TP)constructorWithConfiguration.Invoke(new object[] { unsecure, secure });

            var defaultConstructor = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
            if (defaultConstructor != null)
                return (TP)defaultConstructor.Invoke(Array.Empty<object>());

            throw new ArgumentException("The plugin does not have constructor for passing in two configuration strings or constructor without arguments.");
        }

        private class InternalXrmFakedContext : IXrmFakedContext
        {
            private IXrmFakedContext Context { get; }

            public InternalXrmFakedContext(IXrmFakedContext context)
            {
                Context = context;
                Db = new TestDatabase(Context);
            }

            public TestDatabase Db { get; }

            public bool IsCustomEarlyBound { get; set; }

            public IOrganizationService GetOrganizationService()
            {
                var service = IsCustomEarlyBound
                    ? new ClearProxyOrganizationService(Context.GetOrganizationService(), Context)
                    : Context.GetOrganizationService();
                return new TestOrganizationService(service, Db);
            }

            public void SetProperty<T>(T property)
            {
                Context.SetProperty(property);
            }

            public T GetProperty<T>()
            {
                return Context.GetProperty<T>();
            }

            public bool HasProperty<T>()
            {
                return Context.HasProperty<T>();
            }

            public FakeXrmEasyLicense? LicenseContext
            {
                get => Context.LicenseContext;
                set => Context.LicenseContext = value;
            }

            public IXrmFakedTracingService GetTracingService()
            {
                return Context.GetTracingService();
            }

            public IQueryable<T> CreateQuery<T>() where T : Entity
            {
                return Context.CreateQuery<T>();
            }

            public IQueryable<Entity> CreateQuery(string logicalName)
            {
                return Context.CreateQuery(logicalName);
            }

            public T GetEntityById<T>(Guid id) where T : Entity
            {
                return Context.GetEntityById<T>(id);
            }

            public Entity GetEntityById(string logicalName, Guid id)
            {
                return Context.GetEntityById(logicalName, id);
            }

            public bool ContainsEntity(string logicalName, Guid id)
            {
                return Context.ContainsEntity(logicalName, id);
            }

            public void Initialize(IEnumerable<Entity> entities)
            {
                Context.Initialize(entities);
            }

            public void Initialize(Entity entity)
            {
                Context.Initialize(entity);
            }

            public void AddEntity(Entity e)
            {
                Context.AddEntity(e);
            }

            public void AddEntityWithDefaults(Entity e, bool clone = false)
            {
                Context.AddEntityWithDefaults(e, clone);
            }

            public Guid CreateEntity(Entity e)
            {
                return Context.CreateEntity(e);
            }

            public void UpdateEntity(Entity e)
            {
                Context.UpdateEntity(e);
            }

            public void DeleteEntity(EntityReference er)
            {
                Context.DeleteEntity(er);
            }

            public Type FindReflectedType(string logicalName)
            {
                return Context.FindReflectedType(logicalName);
            }

            public Type FindReflectedAttributeType(Type earlyBoundType, string sEntityName, string attributeName)
            {
                return Context.FindReflectedAttributeType(earlyBoundType, sEntityName, attributeName);
            }

            public void EnableProxyTypes(Assembly assembly)
            {
                Context.EnableProxyTypes(assembly);
            }

            public void InitializeMetadata(IEnumerable<EntityMetadata> entityMetadataList)
            {
                Context.InitializeMetadata(entityMetadataList);
            }

            public void InitializeMetadata(EntityMetadata entityMetadata)
            {
                Context.InitializeMetadata(entityMetadata);
            }

            public void InitializeMetadata(Assembly earlyBoundEntitiesAssembly)
            {
                Context.InitializeMetadata(earlyBoundEntitiesAssembly);
            }

            public IQueryable<EntityMetadata> CreateMetadataQuery()
            {
                return Context.CreateMetadataQuery();
            }

            public EntityMetadata GetEntityMetadataByName(string sLogicalName)
            {
                return Context.GetEntityMetadataByName(sLogicalName);
            }

            public void SetEntityMetadata(EntityMetadata em)
            {
                Context.SetEntityMetadata(em);
            }

            public void AddRelationship(string schemaname, XrmFakedRelationship relationship)
            {
                Context.AddRelationship(schemaname, relationship);
            }

            public void RemoveRelationship(string schemaname)
            {
                Context.RemoveRelationship(schemaname);
            }

            public XrmFakedRelationship GetRelationship(string schemaName)
            {
                return Context.GetRelationship(schemaName);
            }

            public Guid GetRecordUniqueId(EntityReference record, bool validate = true)
            {
                return Context.GetRecordUniqueId(record, validate);
            }

            public ICallerProperties CallerProperties
            {
                get => Context.CallerProperties;
                set => Context.CallerProperties = value;
            }

            public IXrmFakedPluginContextProperties PluginContextProperties
            {
                get => Context.PluginContextProperties;
                set => Context.PluginContextProperties = value;
            }

            public IEnumerable<Assembly> ProxyTypesAssemblies => Context.ProxyTypesAssemblies;

            public IEnumerable<XrmFakedRelationship> Relationships => Context.Relationships;
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
