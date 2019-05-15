using System;
using FakeXrmEasy.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework.TestHelper
{
    public class TestOrganizationService : IOrganizationService
    {
        private readonly IOrganizationService _service;
        private readonly TestDatabase _db;

        public TestOrganizationService(IOrganizationService service, TestDatabase db)
        {
            _service = service;
            _db = db;
        }

        public Guid Create(Entity entity)
        {
            var id = _service.Create(entity);
            var copy = entity.Copy();
            copy.Id = id;
            _db.Event.Created.Add(copy);
            return id;
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return _service.Retrieve(entityName, id, columnSet);
        }

        public void Update(Entity entity)
        {
            _service.Update(entity);
            _db.Event.Updated.Add(entity.Copy());
        }

        public void Delete(string entityName, Guid id)
        {
            _service.Delete(entityName, id);
            _db.Event.Deleted.Add(new EntityReference(entityName, id));
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return _service.Execute(request);
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _service.Associate(entityName, entityId, relationship, relatedEntities);
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship,
            EntityReferenceCollection relatedEntities)
        {
            _service.Disassociate(entityName, entityId, relationship, relatedEntities);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return _service.RetrieveMultiple(query);
        }
    }
}