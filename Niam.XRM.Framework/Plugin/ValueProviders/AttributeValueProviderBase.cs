using System;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework.Plugin.ValueProviders
{
    public abstract class AttributeValueProviderBase<T, TV> : IAttributeValueProvider<T, TV>
        where T : Entity
    {
        public TV GetValueFor(Expression<Func<T, TV>> targetAttribute)
            => HandleGetValueFor(Helper.Name(targetAttribute));

        public object GetValueFor(string targetAttribute)
            => HandleGetValueFor(targetAttribute);

        protected abstract TV HandleGetValueFor(string targetAttribute);
    }
}
