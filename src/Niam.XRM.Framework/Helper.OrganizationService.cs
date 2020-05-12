using System;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Data;

namespace Niam.XRM.Framework
{
    public static partial class Helper
    {
        public static void Delete(this IOrganizationService service, EntityReference reference)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            service.Delete(reference.LogicalName, reference.Id);
        }

        public static EntityCollection RetrieveMultiple(this IOrganizationService service, string fetchXml)
            => service.RetrieveMultiple(new FetchExpression(fetchXml));

        public static string GetReferenceName<T>(this IOrganizationService service, EntityReference reference)
            where T : Entity
        {
            if (reference == null) return null;

            var entityName = Name<T>();
            if (entityName != reference.LogicalName)
                throw new InvalidOperationException($"Logical name from EntityReference: '{reference.LogicalName}' is not same as T: '{entityName}'.");

            return String.IsNullOrWhiteSpace(reference.Name) 
                ? GetReferenceName<T>(service, reference.Id) 
                : reference.Name;
        }

        public static string GetReferenceName<T>(this IOrganizationService service, Guid id)
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
        
        public static TE Retrieve<TE>(this IOrganizationService service, Guid id)
            where TE : Entity => service.Retrieve(id, new ColumnSet<TE>(true));

        public static TE Retrieve<TE>(this IOrganizationService service, Guid id, params Expression<Func<TE, object>>[] attributes)
            where TE : Entity => service.Retrieve(id, new ColumnSet<TE>(attributes));

        public static TE Retrieve<TE>(this IOrganizationService service, Guid id, ColumnSet<TE> columnSet)
            where TE : Entity => service.Retrieve(Name<TE>(), id, columnSet).ToEntity<TE>();
    }
}
