using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Infrastructure;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.Framework.Interfaces.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin.Configurations;

namespace Niam.XRM.Framework.Plugin
{
    public class PluginContext<TE> : IPluginContext<TE> where TE : Entity
    {
        private readonly ITransactionContext<TE> _transactionContext;

        private readonly Lazy<IEntityWrapper<TE>> _currentWrapper;

        public PluginContext(ITransactionContext<TE> transactionContext)
        {
            _transactionContext = transactionContext ?? throw new ArgumentNullException(nameof(transactionContext));
            _currentWrapper = new Lazy<IEntityWrapper<TE>>(GetCurrentWrapper);
        }

        private EntityWrapper<TE> GetCurrentWrapper() 
            => InstanceEntityWrapper<TE, EntityWrapper<TE>>.Create(_transactionContext.Current.Entity, _transactionContext);

        public IPluginExecutionContext PluginExecutionContext => _transactionContext.PluginExecutionContext;

        public ITracingService TracingService => _transactionContext.TracingService;

        public IDictionary<string, object> SessionStorage => _transactionContext.SessionStorage;

        public IPluginBase Plugin => _transactionContext.Plugin;

        public IOrganizationService Service => _transactionContext.Service;

        public IOrganizationService SystemService => _transactionContext.SystemService;

        public IOrganizationServiceFactory ServiceFactory => _transactionContext.ServiceFactory;

        public IServiceProvider ServiceProvider => _transactionContext.ServiceProvider;

        public PluginLogOption LogOption => _transactionContext.LogOption;

        public ITransactionContextEntity<TE> Target => _transactionContext.Target;

        public ITransactionContextEntity<TE> Current => _transactionContext.Current;

        public IEntityGetter<TE> Initial => _transactionContext.Initial;

        public void Set(MemberInfo memberInfo, object value) => Target.Set(memberInfo, value);

        public void Set(string attributeName, object value) => Target.Set(attributeName, value);

        public void Set(string attributeName, IAttributeValueProvider attributeValueProvider) => Target.Set(attributeName, attributeValueProvider);

        public void Set(string attributeName, IValueProvider valueProvider) => Target.Set(attributeName, valueProvider);

        public void SetFormattedValue(string attributeName, string formattedValue) => Target.SetFormattedValue(attributeName, formattedValue);
        
        public TV Get<TV>(string attributeName) => Current.Get<TV>(attributeName);

        public string GetFormattedValue(string attributeName) => Current.GetFormattedValue(attributeName);

        public IEntityWrapper<TR> GetRelated<TR>(Expression<Func<TE, EntityReference>> relatedReference, 
            IColumnSet<TR> relatedColumnSet) 
            where TR : Entity => _currentWrapper.Value.GetRelated<TR>(relatedReference, relatedColumnSet);

        public TW GetRelated<TR, TW>(Expression<Func<TE, EntityReference>> relatedReference, IColumnSet<TR> relatedColumnSet) 
            where TR : Entity 
            where TW : class, IEntityWrapper<TR>
            => _currentWrapper.Value.GetRelated<TR, TW>(relatedReference, relatedColumnSet);

        public IEnumerable<IEntityWrapper<TR>> GetAllRelated<TR>(Expression<Func<TR, EntityReference>> relatedToParentAttribute, 
            IColumnSet<TR> relatedColumnSet) 
            where TR : Entity
            => _currentWrapper.Value.GetAllRelated<TR>(relatedToParentAttribute, relatedColumnSet);

        public IEnumerable<TW> GetAllRelated<TR, TW>(Expression<Func<TR, EntityReference>> relatedToParentAttribute, 
            IColumnSet<TR> relatedColumnSet) 
            where TR : Entity 
            where TW : class, IEntityWrapper<TR>
            => _currentWrapper.Value.GetAllRelated<TR, TW>(relatedToParentAttribute, relatedColumnSet);

        public IEntityWrapper<Entity> GetRelated(string relatedAttribute, ColumnSet relatedColumnSet)
            => _currentWrapper.Value.GetRelated(relatedAttribute, relatedColumnSet);

        public IEnumerable<IEntityWrapper<Entity>> GetAllRelated(string relatedEntityName, string relatedToParentAttribute, ColumnSet relatedColumnSet)
            => _currentWrapper.Value.GetAllRelated(relatedEntityName, relatedToParentAttribute, relatedColumnSet);

        public string GetReferenceName<TR>(Expression<Func<TE, EntityReference>> relatedReference) 
            where TR : Entity => _currentWrapper.Value.GetReferenceName<TR>(relatedReference);

        public IEntityWrapper<T> ToWrapper<T>() 
            where T : Entity => _currentWrapper.Value.ToWrapper<T>();

        public TW ToWrapper<T, TW>() 
            where T : Entity 
            where TW : class, IEntityWrapper<T>
            => _currentWrapper.Value.ToWrapper<T, TW>();
    }
}
