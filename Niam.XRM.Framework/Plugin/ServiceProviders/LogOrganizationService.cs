using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework.Plugin.ServiceProviders
{
    public class LogOrganizationService : IOrganizationService
    {
        private static readonly object VoidValue = new object();
        private readonly IOrganizationService _service;
        private readonly ITracingService _tracingService;
        private readonly DataContractJsonSerializerSettings _jsonSettings = new DataContractJsonSerializerSettings
        {
            EmitTypeInformation = EmitTypeInformation.Never
        };

        public LogOrganizationService(IOrganizationService service, ITracingService tracingService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tracingService = tracingService ?? throw new ArgumentNullException(nameof(tracingService));
        }

        public Guid Create(Entity entity)
        {
            const string name = "OrganizationService.Create";
            var request = entity.ToJson(_jsonSettings);
            return Do(name, request, () => _service.Create(entity), id => id.ToString("D"));
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            const string name = "OrganizationService.Retrieve";
            var request = new LogRetrieveRequest
            {
                EntityName = entityName,
                Id = id,
                ColumnSet = columnSet
            }.ToJson(_jsonSettings);
            return Do(name, request, () => _service.Retrieve(entityName, id, columnSet), entity => entity.ToJson(_jsonSettings));
        }

        [DataContract]
        private class LogRetrieveRequest
        {
            [DataMember(Order = 1)]
            public string EntityName { get; set; }

            [DataMember(Order = 2)]
            public Guid Id { get; set; }

            [DataMember(Order = 3)]
            public ColumnSet ColumnSet { get; set; }
        }
        
        public void Update(Entity entity)
        {
            const string name = "OrganizationService.Update";
            var request = entity.ToJson(_jsonSettings);
            Do(name, request, () => _service.Update(entity));
        }

        public void Delete(string entityName, Guid id)
        {
            const string name = "OrganizationService.Delete";
            var request = new LogDeleteRequest
            {
                EntityName = entityName,
                Id = id
            }.ToJson(_jsonSettings);
            Do(name, request, () => _service.Delete(entityName, id));
        }

        [DataContract]
        private class LogDeleteRequest
        {
            [DataMember(Order = 1)]
            public string EntityName { get; set; }

            [DataMember(Order = 2)]
            public Guid Id { get; set; }
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            const string name = "OrganizationService.Execute";
            var logRequest = request.ToJson(_jsonSettings);
            return Do(name, logRequest, () => _service.Execute(request), response => response.ToJson(_jsonSettings));
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            const string name = "OrganizationService.Associate";
            var request = new LogAssociationRequest
            {
                EntityName = entityName,
                EntityId = entityId,
                Relationship = relationship,
                RelatedEntities = relatedEntities
            }.ToJson(_jsonSettings);
            Do(name, request, () => _service.Associate(entityName, entityId, relationship, relatedEntities));
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship,
            EntityReferenceCollection relatedEntities)
        {
            const string name = "OrganizationService.Disassociate";
            var request = new LogAssociationRequest
            {
                EntityName = entityName,
                EntityId = entityId,
                Relationship = relationship,
                RelatedEntities = relatedEntities
            }.ToJson(_jsonSettings);
            Do(name, request, () => _service.Disassociate(entityName, entityId, relationship, relatedEntities));
        }

        [DataContract]
        private class LogAssociationRequest
        {
            [DataMember(Order = 1)]
            public string EntityName { get; set; }

            [DataMember(Order = 2)]
            public Guid EntityId { get; set; }

            [DataMember(Order = 3)]
            public Relationship Relationship { get; set; }

            [DataMember(Order = 4)]
            public EntityReferenceCollection RelatedEntities { get; set; }
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            const string name = "OrganizationService.RetrieveMultiple";
            var request = query.ToJson(_jsonSettings);
            return Do(name, request, () => _service.RetrieveMultiple(query), collection => collection.ToJson(_jsonSettings));
        }

        private void Do(string name, string request, Action action) => Do(name, request, () =>
        {
            action();
            return VoidValue;
        }, response => null);

        private T Do<T>(string name, string request, Func<T> operation, Func<T, string> responseFn)
        {
            _tracingService.Trace(name);
            _tracingService.Trace("Request:");
            _tracingService.Trace(request);
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var result = operation();
                stopwatch.Stop();
                var response = responseFn(result);
                if (response != null)
                {
                    _tracingService.Trace("Response:");
                    _tracingService.Trace(response);
                }
                else
                {
                    _tracingService.Trace("Response: -");
                }

                _tracingService.Trace($"{name} [{stopwatch.Elapsed:g}]");

                return result;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        private void LogError(Exception ex)
        {
            _tracingService.Trace("The application terminated with an error.");

            if (ex is FaultException<OrganizationServiceFault>)
                LogOrganizationServiceFault((FaultException<OrganizationServiceFault>) ex);
            else if (ex.InnerException is FaultException<OrganizationServiceFault>)
                LogOrganizationServiceFault((FaultException<OrganizationServiceFault>) ex.InnerException);
            else
                LogException(ex);
        }

        private void LogException(Exception ex)
        {
            _tracingService.Trace($"Message: {ex.Message}");
            _tracingService.Trace($"Stack Trace: {ex.StackTrace}");
            var innerFault = ex.InnerException?.Message != null ? " - " : ex.InnerException?.Message;
            _tracingService.Trace($"Inner Fault: {innerFault}");
        }

        private void LogOrganizationServiceFault(FaultException<OrganizationServiceFault> ex)
        {
            _tracingService.Trace($"Timestamp: {ex.Detail.Timestamp}");
            _tracingService.Trace($"Code: {ex.Detail.ErrorCode}");
            _tracingService.Trace($"Message: {ex.Detail.Message}");
            _tracingService.Trace($"Trace: {ex.Detail.TraceText}");
        }
    }
}
