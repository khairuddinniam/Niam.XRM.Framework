using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework.Plugin
{
    public abstract class OrganizationServiceBase : IOrganizationService
    {
        private readonly IOrganizationService _service;

        protected OrganizationServiceBase(IOrganizationService service)
        {
            _service = service;
        }

        public virtual Guid Create(Entity entity) 
            => _service.Create(entity);

        public virtual Entity Retrieve(string entityName, Guid id, ColumnSet columnSet) 
            => _service.Retrieve(entityName, id, columnSet);

        public virtual void Update(Entity entity) 
            => _service.Update(entity);

        public virtual void Delete(string entityName, Guid id) 
            => _service.Delete(entityName, id);

        public virtual OrganizationResponse Execute(OrganizationRequest request) 
            => _service.Execute(request);

        public virtual void Associate(
            string entityName, Guid entityId, 
            Relationship relationship, EntityReferenceCollection relatedEntities) 
            => _service.Associate(entityName, entityId, relationship, relatedEntities);

        public virtual void Disassociate(
            string entityName, Guid entityId, 
            Relationship relationship, EntityReferenceCollection relatedEntities) 
            => _service.Disassociate(entityName, entityId, relationship, relatedEntities);

        public virtual EntityCollection RetrieveMultiple(QueryBase query) 
            => _service.RetrieveMultiple(query);
    }
}
