using System;
using FakeXrmEasy;
using FakeXrmEasy.Abstractions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework.TestHelper
{
    internal class ClearProxyOrganizationService : IOrganizationService
    {
        private readonly IOrganizationService _service;
        private readonly IXrmFakedContext _xrmFakedContext;

        public ClearProxyOrganizationService(IOrganizationService service, IXrmFakedContext xrmFakedContext)
        {
            _service = service;
            _xrmFakedContext = xrmFakedContext;
        }

        public Guid Create(Entity entity)
        {
            return Do(() => _service.Create(entity));
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return Do(() => _service.Retrieve(entityName, id, columnSet));
        }

        public void Update(Entity entity)
        {
            Do(() => _service.Update(entity));
        }

        public void Delete(string entityName, Guid id)
        {
            Do(() => _service.Delete(entityName, id));
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return Do(() => _service.Execute(request));
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            Do(() => _service.Associate(entityName, entityId, relationship, relatedEntities));
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship,
            EntityReferenceCollection relatedEntities)
        {
            Do(() => _service.Disassociate(entityName, entityId, relationship, relatedEntities));
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return Do(() => _service.RetrieveMultiple(query));
        }

        private void Do(Action action)
        {
            action();
        }

        private T Do<T>(Func<T> func)
        {
            var result = func();
            return result;
        }
    }
}
