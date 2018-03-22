using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Infrastructure;
using Niam.XRM.Framework.Interfaces.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework.Data
{
    public class EntityWrapper<T> : EntityAccessorWrapper<T>, IEntityWrapper<T>
        where T : Entity
    {
        protected ITransactionContextBase Context { get; }
        protected IOrganizationService Service => Context.Service;
        
        public Guid Id => Entity.Id;
        public string LogicalName => Entity.LogicalName;

        public EntityWrapper(T entity, ITransactionContextBase context)
            : this(new EntityAccessor<T>(entity), context)
        {
        }

        public EntityWrapper(IEntityAccessor<T> accessor, ITransactionContextBase context)
            : base(accessor)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public EntityReference ToEntityReference() => Entity.ToEntityReference();

        public IEntityWrapper<TR> GetRelated<TR>(Expression<Func<T, EntityReference>> relatedReference, IColumnSet<TR> relatedColumnSet)
            where TR : Entity =>
            GetRelated<TR, EntityWrapper<TR>>(relatedReference, relatedColumnSet);

        public TW GetRelated<TR, TW>(Expression<Func<T, EntityReference>> relatedReference, IColumnSet<TR> relatedColumnSet)
            where TR : Entity
            where TW : class, IEntityWrapper<TR> =>
            GetRelated(Helper.Name(relatedReference), relatedColumnSet?.XrmColumnSet)?.ToWrapper<TR, TW>();

        public IEnumerable<IEntityWrapper<TR>> GetAllRelated<TR>(Expression<Func<TR, EntityReference>> relatedToParentAttribute,
            IColumnSet<TR> relatedColumnSet)
            where TR : Entity =>
            GetAllRelated<TR, EntityWrapper<TR>>(relatedToParentAttribute, relatedColumnSet);

        public IEnumerable<TW> GetAllRelated<TR, TW>(Expression<Func<TR, EntityReference>> relatedToParentAttribute,
            IColumnSet<TR> relatedColumnSet)
            where TR : Entity
            where TW : class, IEntityWrapper<TR> =>
            GetAllRelated(Helper.Name<TR>(), Helper.Name(relatedToParentAttribute), relatedColumnSet?.XrmColumnSet)
                .Select(w => w.ToWrapper<TR, TW>())
                .ToArray();

        public IEntityWrapper<Entity> GetRelated(string relatedAttribute, ColumnSet relatedColumnSet)
        {
            var relatedEntity = GetRelatedCore(relatedAttribute, relatedColumnSet);
            return relatedEntity != null
                ? new EntityWrapper<Entity>(relatedEntity, Context)
                : null;
        }

        public IEnumerable<IEntityWrapper<Entity>> GetAllRelated(string relatedEntityName, string relatedToParentAttribute, ColumnSet relatedColumnSet)
        {
            var relatedEntities = GetAllRelatedCore(relatedEntityName, relatedToParentAttribute, relatedColumnSet);
            var results = relatedEntities.Select(e => new EntityWrapper<Entity>(e, Context)).ToArray();
            return results;
        }

        public void Delete() => Service.Delete(Entity.LogicalName, Entity.Id);

        public string GetReferenceName<TR>(Expression<Func<T, EntityReference>> relatedReference) 
            where TR : Entity
        {
            var attributeName = Helper.Name(relatedReference);
            var reference = Get<EntityReference>(attributeName);
            return Service.GetReferenceName<TR>(reference);
        }

        public IEntityWrapper<TE> ToWrapper<TE>()
            where TE : Entity => ToWrapper<TE, EntityWrapper<TE>>();

        public TW ToWrapper<TE, TW>()
            where TE : Entity
            where TW : class, IEntityWrapper<TE>
        {
            var sameWrapper = this as TW;
            if (sameWrapper != null) return sameWrapper;

            var targetEntity = Entity.ToEntity<TE>();
            var targetWrapper = InstanceEntityWrapper<TE, TW>.Create(targetEntity, Context);
            return targetWrapper;
        }

        protected Entity GetRelatedCore(string relatedAttribute, ColumnSet relatedColumnSet)
        {
            var relatedReference = Get<EntityReference>(relatedAttribute);
            return relatedReference != null
                ? Service.Retrieve(relatedReference.LogicalName, relatedReference.Id, relatedColumnSet ?? new ColumnSet())
                : null;
        }

        protected Entity[] GetAllRelatedCore(string relatedEntityName, string relatedToParentAttribute, ColumnSet relatedColumnSet)
        {
            var query = new QueryExpression(relatedEntityName)
            {
                ColumnSet = relatedColumnSet ?? new ColumnSet()
            };

            query.Criteria.AddCondition(relatedToParentAttribute, ConditionOperator.Equal, Id);

            return Service.RetrieveMultiple(query).Entities.ToArray();
        }
    }
}