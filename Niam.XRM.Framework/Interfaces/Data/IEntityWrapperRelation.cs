using System;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces.Data
{
    public interface IEntityWrapperRelation<T>
        where T : Entity
    {
        IEntityWrapper<TE> GetRelated<TE>(Expression<Func<T, EntityReference>> relatedReference, IColumnSet<TE> relatedColumnSet)
            where TE : Entity;
    }
}
