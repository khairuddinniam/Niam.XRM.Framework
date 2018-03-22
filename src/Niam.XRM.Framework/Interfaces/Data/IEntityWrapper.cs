using System;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces.Data
{
    public interface IEntityWrapperBase<T> : IEntityWrapperRelation<T>
        where T : Entity
    {
        string GetReferenceName<TR>(Expression<Func<T, EntityReference>> relatedReference)
            where TR : Entity;

        IEntityWrapper<TE> ToWrapper<TE>() where TE : Entity;

        TW ToWrapper<TE, TW>()
            where TE : Entity
            where TW : class, IEntityWrapper<TE>;
    }

    public interface IEntityWrapper<T> : IEntityWrapperBase<T>, IEntityAccessor<T>
        where T : Entity
    {
        Guid Id { get; }
        string LogicalName { get; }
        EntityReference ToEntityReference();
        void Delete();
    }
}