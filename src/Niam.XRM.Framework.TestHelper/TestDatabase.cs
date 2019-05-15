using System;
using System.Collections.Generic;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.TestHelper
{
    public class TestDatabase
    {
        private readonly XrmFakedContext _xrmFakedContext;
        
        public DbEvent Event { get; } = new DbEvent();

        public Dictionary<string, Dictionary<Guid, Entity>> Data => _xrmFakedContext.Data;
        
        public TestDatabase(XrmFakedContext xrmFakedContext)
        {
            _xrmFakedContext = xrmFakedContext;
        }
        
        public Entity Get(Guid id) => Get<Entity>(id);
        
        public Entity Get(EntityReference reference) => Get<Entity>(reference);

        public TEntity Get<TEntity>(Guid id) where TEntity : Entity
        {
            foreach (var entities in Data.Values)
            {
                if (entities.TryGetValue(id, out var entity))
                    return (TEntity) entity;
            }

            return null;
        }
            

        public TEntity Get<TEntity>(EntityReference reference) where TEntity : Entity
        {
            if (!Data.TryGetValue(reference.LogicalName, out var entities))
                return null;

            return entities.TryGetValue(reference.Id, out var entity) ? (TEntity) entity : null;
        }

        public class DbEvent
        {
            public IList<Entity> Created { get; } = new List<Entity>();
        
            public IList<Entity> Updated { get; } = new List<Entity>();
        
            public IList<EntityReference> Deleted { get; } = new List<EntityReference>();
        }
    }
}