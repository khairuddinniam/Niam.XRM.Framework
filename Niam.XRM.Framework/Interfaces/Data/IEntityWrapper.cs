using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework.Interfaces.Data
{
    public interface IEntityWrapper<T> : IEntityWrapperRelation<T>, IEntityAccessor<T>
        where T : Entity
    {
        Guid Id { get; }
        string LogicalName { get; }
        EntityReference ToEntityReference();
        void Delete();

        string GetName<TR>(Expression<Func<T, EntityReference>> relatedReference)
            where TR : Entity;
        
        IEntityWrapper<TE> ToWrapper<TE>() where TE : Entity;

        TW ToWrapper<TE, TW>()
            where TE : Entity
            where TW : class, IEntityWrapper<TE>;
        
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