using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework.Plugin.ServiceProviders
{
    public class ToEntityService : IOrganizationService
    {
        private readonly IOrganizationService _service;
        private readonly ITracingService _tracingService;

        public ToEntityService(IOrganizationService service, ITracingService tracingService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tracingService = tracingService ?? throw new ArgumentNullException(nameof(tracingService));
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _service.Associate(entityName, entityId, relationship, relatedEntities);
        }

        public Guid Create(Entity entity)
        {
            return _service.Create(entity.ToEntity<Entity>());
        }

        public void Delete(string entityName, Guid id)
        {
            _service.Delete(entityName, id);
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _service.Disassociate(entityName, entityId, relationship, relatedEntities);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return _service.Execute(request);
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return _service.Retrieve(entityName, id, columnSet);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return _service.RetrieveMultiple(query);
        }

        public void Update(Entity entity)
        {
            _service.Update(entity.ToEntity<Entity>());
        }

        public static IOrganizationService Decorate(IOrganizationService service, IServiceProvider serviceProvider) =>
            new ToEntityService(service, serviceProvider.GetService<ITracingService>());
    }
}
