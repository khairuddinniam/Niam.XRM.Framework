using System;
using System.Collections.Generic;
using FakeXrmEasy.Abstractions;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.TestHelper
{
    public class TestDatabase
    {
        private readonly IXrmFakedContext _xrmFakedContext;
        
        public DbEvent Event { get; } = new DbEvent();
        
        public TestDatabase(IXrmFakedContext xrmFakedContext)
        {
            _xrmFakedContext = xrmFakedContext;
        }
        
        public Entity Get(Guid id) => Get<Entity>(id);
        
        public Entity Get(EntityReference reference) => Get<Entity>(reference);

        public TEntity Get<TEntity>(Guid id) where TEntity : Entity
        {
            return _xrmFakedContext.GetEntityById<TEntity>(id);
        }
            

        public TEntity Get<TEntity>(EntityReference reference) where TEntity : Entity
        {
            return _xrmFakedContext.GetEntityById(reference.LogicalName, reference.Id).ToEntity<TEntity>();
        }

        public class DbEvent
        {
            public IList<Entity> Created { get; } = new List<Entity>();
        
            public IList<Entity> Updated { get; } = new List<Entity>();
        
            public IList<EntityReference> Deleted { get; } = new List<EntityReference>();
        }
    }
}