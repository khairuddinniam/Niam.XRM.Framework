using System;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin.Metadata
{
    public static partial class MetadataExt
    {
        public static AttributeMetadata GetMetadata<T, TV>(this ITransactionContext<Entity> context, Expression<Func<T, TV>> attribute)
            where T : Entity => GetMetadata(context, Helper.Name<T>(), Helper.Name(attribute));

        public static AttributeMetadata GetMetadata<T>(this ITransactionContext<Entity> context, Expression<Func<T, object>> attribute)
           where T : Entity => GetMetadata(context, Helper.Name<T>(), Helper.Name(attribute));

        public static T GetMetadata<T>(this ITransactionContext<Entity> context, string entityLogicalName, string attributeName)
            where T : AttributeMetadata => (T) GetMetadata(context, entityLogicalName, attributeName);

        public static AttributeMetadata GetMetadata(this ITransactionContext<Entity> context, string entityLogicalName, string attributeName)
        {
            var attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityLogicalName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = false
            };

            var attributeResponse = (RetrieveAttributeResponse) context.Service.Execute(attributeRequest);
            return attributeResponse.AttributeMetadata;
        }

        public static EntityMetadata GetMetadata<T>(this ITransactionContext<Entity> context, EntityFilters filters = EntityFilters.Default)
            where T : Entity => GetMetadata(context, Helper.Name<T>(), filters);

        public static EntityMetadata GetMetadata(this ITransactionContext<Entity> context,
            string entityLogicalName, EntityFilters filters = EntityFilters.Default)
        {
            var entityRequest = new RetrieveEntityRequest
            {
                EntityFilters = filters,
                LogicalName = entityLogicalName,
                RetrieveAsIfPublished = false
            };

            var entityResponse = (RetrieveEntityResponse) context.Service.Execute(entityRequest);
            return entityResponse.EntityMetadata;
        }
    }
}
