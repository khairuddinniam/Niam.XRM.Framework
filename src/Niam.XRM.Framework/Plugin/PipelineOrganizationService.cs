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
            var handler = _pipelines.GetAll<IPipeline<XrmCreateRequest, Guid>>()
                .Reverse()
                .Aggregate((XrmCreateRequest req) => _service.Create(req.Entity), 
                    (next, pipeline) => (req) => pipeline.Handle(req, next));
            return handler(new XrmCreateRequest(entity));
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            var handler = _pipelines.GetAll<IPipeline<XrmRetrieveRequest, Entity>>()
                .Reverse()
                .Aggregate((XrmRetrieveRequest req) => _service.Retrieve(req.EntityName, req.Id, req.ColumnSet), 
                    (next, pipeline) => (req) => pipeline.Handle(req, next));
            return handler(new XrmRetrieveRequest(entityName, id, columnSet));
        }

        public void Update(Entity entity)
        {
            var handler = _pipelines.GetAll<IPipeline<XrmUpdateRequest, Unit>>()
                .Reverse()
                .Aggregate((XrmUpdateRequest req) =>
                    {
                        _service.Update(req.Entity);
                        return Unit.Value;
                    }, 
                    (next, pipeline) => (req) => pipeline.Handle(req, next));
            handler(new XrmUpdateRequest(entity));
        }

        public void Delete(string entityName, Guid id)
        {
            var handler = _pipelines.GetAll<IPipeline<XrmDeleteRequest, Unit>>()
                .Reverse()
                .Aggregate((XrmDeleteRequest req) =>
                    {
                        _service.Delete(req.EntityName, req.Id);
                        return Unit.Value;
                    }, 
                    (next, pipeline) => (req) => pipeline.Handle(req, next));

            handler(new XrmDeleteRequest(entityName, id));
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            var handler = _pipelines.GetAll<IPipeline<OrganizationRequest, OrganizationResponse>>()
                .Reverse()
                .Aggregate((OrganizationRequest req) => _service.Execute(req), 
                    (next, pipeline) => (req) => pipeline.Handle(req, next));
            return handler(request);
        }

        public void Associate(
            string entityName, 
            Guid entityId, 
            Relationship relationship, 
            EntityReferenceCollection relatedEntities)
        {
            var handler = _pipelines.GetAll<IPipeline<XrmAssociateRequest, Unit>>()
                .Reverse()
                .Aggregate((XrmAssociateRequest req) =>
                    {
                        _service.Associate(req.EntityName, req.EntityId, req.Relationship, req.RelatedEntities);
                        return Unit.Value;
                    }, 
                    (next, pipeline) => (req) => pipeline.Handle(req, next));

            handler(new XrmAssociateRequest(entityName, entityId, relationship, relatedEntities));
        }

        public void Disassociate(
            string entityName, 
            Guid entityId, 
            Relationship relationship,
            EntityReferenceCollection relatedEntities)
        {
            var handler = _pipelines.GetAll<IPipeline<XrmDisassociateRequest, Unit>>()
                .Reverse()
                .Aggregate((XrmDisassociateRequest req) =>
                    {
                        _service.Disassociate(entityName, entityId, relationship, relatedEntities);
                        return Unit.Value;
                    }, 
                    (next, pipeline) => (req) => pipeline.Handle(req, next));

            handler(new XrmDisassociateRequest(entityName, entityId, relationship, relatedEntities));
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            var handler = _pipelines.GetAll<IPipeline<XrmRetrieveMultipleRequest, EntityCollection>>()
                .Reverse()
                .Aggregate((XrmRetrieveMultipleRequest req) => _service.RetrieveMultiple(query), 
                    (next, pipeline) => (req) => pipeline.Handle(req, next));

            return handler(new XrmRetrieveMultipleRequest(query));
        }
    }
}