using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework
{
    public class EntityAccessorWrapper<T> : IEntityAccessor<T>
        where T : Entity
    {
        private readonly IEntityAccessor<T> _accessor;

        public T Entity => _accessor.Entity;

        public EntityAccessorWrapper(IEntityAccessor<T> accessor)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }
        
        public virtual TV Get<TV>(string attributeName) 
            => _accessor.Get<TV>(attributeName);

        public virtual string GetFormattedValue(string attributeName) 
            => _accessor.GetFormattedValue(attributeName);

        public virtual void Set(string attributeName, object value) 
            => _accessor.Set(attributeName, value);

        public virtual void Set(string attributeName, IAttributeValueProvider attributeValueProvider) 
            => _accessor.Set(attributeName, attributeValueProvider);

        public virtual void Set(string attributeName, IValueProvider valueProvider) 
            => _accessor.Set(attributeName, valueProvider);

        public virtual void SetFormattedValue(string attributeName, string formattedValue) 
            => _accessor.SetFormattedValue(attributeName, formattedValue);
    }
}
