using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework.Infrastructure
{
    public static class InstanceEntityWrapper<T, TW>
        where T : Entity
        where TW : IEntityWrapper<T>
    {
        public static TW Create(T entity, ITransactionContextBase context) 
            => (TW) Activator.CreateInstance(typeof(TW), entity, context);

        public static TW Create(IEntityAccessor<T> accessor, ITransactionContextBase context)
            => (TW) Activator.CreateInstance(typeof(TW), accessor, context);
    }

    public static class Instance<T>
    {
        public static T Create() => Activator.CreateInstance<T>();
    }
}
