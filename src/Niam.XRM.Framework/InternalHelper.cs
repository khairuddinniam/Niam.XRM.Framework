using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework
{
    internal static class InternalHelper
    {
        public static T Copy<T>(this T source, ColumnSet columnSet = null) where T : Entity
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var target = new Entity
            {
                Id = source.Id,
                LogicalName = source.LogicalName,
                EntityState = source.EntityState,
                RowVersion = source.RowVersion,
                Attributes = Copy(source.Attributes, columnSet),
                ExtensionData = source.ExtensionData
            };
            target.RelatedEntities.AddRange(source.RelatedEntities);
            target.FormattedValues.AddRange(source.FormattedValues);
            target.KeyAttributes.AddRange(source.KeyAttributes);

            return target.ToEntity<T>();
        }

        public static AttributeCollection Copy(AttributeCollection source, ColumnSet columnSet = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var attributeCopies = source.Select(Copy);
            if (columnSet != null && !columnSet.AllColumns)
            {
                attributeCopies = from copy in attributeCopies
                    join column in columnSet.Columns on copy.Key equals column
                    select copy;
            }

            var attributes = new AttributeCollection();
            attributes.AddRange(attributeCopies);
            return attributes;
        }

        public static KeyValuePair<string, object> Copy(KeyValuePair<string, object> source)
        {
            if (source.Value == null) return new KeyValuePair<string, object>(source.Key, source.Value);
            
            if (source.Value is EntityReference reference)
            {
                var copy = new EntityReference(reference.LogicalName, reference.Id) { Name = reference.Name };
                return new KeyValuePair<string, object>(source.Key, copy);
            }
            
            if (source.Value is OptionSetValue option)
            {
                var copy = new OptionSetValue(option.Value);
                return new KeyValuePair<string, object>(source.Key, copy);
            }
            
            if (source.Value is Money money)
            {
                var copy = new Money(money.Value);
                return new KeyValuePair<string, object>(source.Key, copy);
            }

            return new KeyValuePair<string, object>(source.Key, source.Value);
        }
    }
}
