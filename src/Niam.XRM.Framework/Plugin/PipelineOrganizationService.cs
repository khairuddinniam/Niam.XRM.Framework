using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework.Plugin
{
    public class PipelineOrganizationService : IOrganizationService
    {
        private readonly IOrganizationService _service;
        private readonly SubscriptionCollection _pipelines;

        public PipelineOrganizationService(IOrganizationService service, SubscriptionCollection pipelines = null)
        {
            _service = service;
            _pipelines = pipelines ?? new SubscriptionCollection();
        }

        public IDisposable AddPipeline(IPipeline pipeline) => _pipelines.Add(pipeline);
        public void RemovePipeline(IPipeline pipeline) => _pipelines.Remove(pipeline);

        public Guid Create(Entity entity)
        {
            var request = new XrmCreateRequest(entity);
            var handler = _pipelines.GetAll<IPipeline<XrmCreateRequest, Guid>>()
                .Reverse()
                .Aggregate(() => _service.Create(request.Entity), 
                    (next, pipeline) => () => pipeline.Handle(request, next));
            return handler();
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            var request = new XrmRetrieveRequest(entityName, id, columnSet);
            var handler = _pipelines.GetAll<IPipeline<XrmRetrieveRequest, Entity>>()
                .Reverse()
                .Aggregate(() => _service.Retrieve(request.EntityName, request.Id, request.ColumnSet), 
                    (next, pipeline) => () => pipeline.Handle(request, next));
            return handler();
        }

        public void Update(Entity entity)
        {
            var request = new XrmUpdateRequest(entity);
            var handler = _pipelines.GetAll<IPipeline<XrmUpdateRequest, Unit>>()
                .Reverse()
                .Aggregate(() =>
                    {
                        _service.Update(request.Entity);
                        return Unit.Value;
                    }, 
                    (next, pipeline) => () => pipeline.Handle(request, next));
            handler();
        }

        public void Delete(string entityName, Guid id)
        {
            _service.Delete(entityName, id);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return _service.Execute(request);
        }

        public void Associate(
            string entityName, 
            Guid entityId, 
            Relationship relationship, 
            EntityReferenceCollection relatedEntities)
        {
            _service.Associate(entityName, entityId, relationship, relatedEntities);
        }

        public void Disassociate(
            string entityName, 
            Guid entityId, 
            Relationship relationship,
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