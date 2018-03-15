using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework.Interfaces.Data
{
    public interface IEntityWrapperRelation<T>
        where T : Entity
    {
        IEntityWrapper<TE> GetRelated<TE>(Expression<Func<T, EntityReference>> relatedReference, IColumnSet<TE> relatedColumnSet)
            where TE : Entity;

        TW GetRelated<TR, TW>(Expression<Func<T, EntityReference>> relatedReference, IColumnSet<TR> relatedColumnSet)
            where TR : Entity
            where TW : class, IEntityWrapper<TR>;

        IEnumerable<IEntityWrapper<TR>> GetAllRelated<TR>(
            Expression<Func<TR, EntityReference>> relatedToParentAttribute,
            IColumnSet<TR> relatedColumnSet)
            where TR : Entity;

        IEnumerable<TW> GetAllRelated<TR, TW>(Expression<Func<TR, EntityReference>> relatedToParentAttribute,
            IColumnSet<TR> relatedColumnSet)
            where TR : Entity
            where TW : class, IEntityWrapper<TR>;

        IEntityWrapper<Entity> GetRelated(string relatedAttribute, ColumnSet relatedColumnSet);

        IEnumerable<IEntityWrapper<Entity>> GetAllRelated(string relatedEntityName, string relatedToParentAttribute, ColumnSet relatedColumnSet);
    }
}
