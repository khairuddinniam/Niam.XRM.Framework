using System;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces
{
    public interface IAttributeValueProvider
    {
        object GetValueFor(string targetAttribute);
    }

    public interface IAttributeValueProvider<T, TV> : IAttributeValueProvider
        where T : Entity
    {
        TV GetValueFor(Expression<Func<T, TV>> targetAttribute);
    }
}
