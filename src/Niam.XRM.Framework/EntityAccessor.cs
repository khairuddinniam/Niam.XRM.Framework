using System;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework
{
    public class EntityAccessor<T> : IEntityAccessor<T>
        where T : Entity
    {
        public T Entity { get; }

        public EntityAccessor(T entity)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
        }

        public virtual TV Get<TV>(string attributeName) 
            => Entity.Get<TV>(attributeName);

        public virtual string GetFormattedValue(string attributeName)
            => Entity.GetFormattedValue(attributeName);

        public virtual void Set(MemberInfo memberInfo, object value)
            => Entity.Set(memberInfo, value);

        public virtual void Set(string attributeName, object value)
            => Entity.Set(attributeName, value);

        public virtual void Set(string attributeName, IAttributeValueProvider attributeValueProvider)
            => Entity.Set(attributeName, attributeValueProvider);

        public virtual void Set(string attributeName, IValueProvider valueProvider)
            => Entity.Set(attributeName, valueProvider);

        public virtual void SetFormattedValue(string attributeName, string formattedValue)
            => Entity.SetFormattedValue(attributeName, formattedValue);
    }
}
