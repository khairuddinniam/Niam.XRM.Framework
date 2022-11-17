using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework.Plugin
{
    public class PipelineOrganizationService : IOrganizationService
    {
        private readonly IOrganizationService _service;

        public List<IPipeline<XrmCreateRequest, Guid>> CreatePipelines { get; } =
            new List<IPipeline<XrmCreateRequest, Guid>>();

        public PipelineOrganizationService(IOrganizationService service)
        {
            _service = service;
        }

        public Guid Create(Entity entity)
        {
            var request = new XrmCreateRequest(entity);
            var handler = CreatePipelines
                .AsEnumerable()
                .Reverse()
                .Aggregate(new Func<Guid>(() => _service.Create(request.Entity)), 
                    (next, pipeline) => () => pipeline.Handle(request, next));
            return handler();
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return _service.Retrieve(entityName, id, columnSet);
        }

        public void Update(Entity entity)
        {
            _service.Update(entity);
        }

        public void Delete(string entityName, Guid id)
        {
            _service.Delete(entityName, id);
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