using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework
{
    public static partial class Helper
    {
        public static void Delete(this IOrganizationService service, EntityReference reference)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            service.Delete(reference.LogicalName, reference.Id);
        }

        public static T Execute<T>(this IOrganizationService service, OrganizationRequest request)
            where T : OrganizationResponse => (T) service.Execute(request);

        public static string GetName<T>(this IOrganizationService service, EntityReference reference)
            where T : Entity
        {
            if (reference == null) return null;

            var entityName = Name<T>();
            if (entityName != reference.LogicalName)
                throw new InvalidOperationException($"Logical name from EntityReference: '{reference.LogicalName}' is not same as T: '{entityName}'.");

            return String.IsNullOrWhiteSpace(reference.Name) 
                ? GetName<T>(service, reference.Id) 
                : reference.Name;
        }

        public static string GetName<T>(this IOrganizationService service, Guid id)
            where T : Entity
        {
            var entityInfo = Info<T>();
            var primaryNameAttribute = entityInfo.PrimaryNameAttribute ?? GetPrimaryAttribute(service, entityInfo.LogicalName);
            var columnSet = new ColumnSet(primaryNameAttribute);
            var entity = service.Retrieve(entityInfo.LogicalName, id, columnSet);
            return entity.Get<string>(primaryNameAttribute);
        }

        internal static string GetPrimaryAttribute(this IOrganizationService service, string entityName)
        {
            var info = Info(entityName);
            if (!String.IsNullOrWhiteSpace(info?.PrimaryNameAttribute))
                return info.PrimaryNameAttribute;

            var request = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Entity,
                LogicalName = entityName
            };
            var response = service.Execute<RetrieveEntityResponse>(request);
            return response.EntityMetadata.PrimaryNameAttribute.ToLowerInvariant();
        }
    }
}
