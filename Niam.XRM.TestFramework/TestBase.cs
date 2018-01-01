using Microsoft.Xrm.Sdk;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;

namespace Niam.XRM.TestFramework
{
    public abstract class TestBase : TestBase<Entity>
    {
    }

    public abstract class TestBase<TEntity> : TestBase<TEntity, ITransactionContext<TEntity>> 
        where TEntity : Entity
    {
    }

    public abstract class TestBase<TEntity, TContext> 
        where TEntity : Entity
        where TContext : class, ITransactionContext<TEntity>
    {
        private TContext _context;
        private bool _configureExecuted;
        private IPluginConfiguration<TEntity> _pluginConfig;

        protected TestHelper Test { get; } = new TestHelper();
        protected IDictionary<string, Entity> Db => Test.Db;

        protected TContext Context => _context ?? (_context = CreateTransactionContext());

        protected virtual TContext CreateTransactionContext() 
            => Test.CreateTransactionContext<TEntity, TContext>();

        protected TEntity Input => Context.Input.Entity;

        protected TEntity Reference
        {
            set
            {
                ExecuteConfigure();
                Context.Input.Entity.Id = value.Id;

                var reference = Context.Reference.Entity;
                reference.Id = value.Id;
                reference.EntityState = value.EntityState;
                reference.RowVersion = value.RowVersion;
                
                reference.Attributes.Clear();
                reference.RelatedEntities.Clear();
                reference.FormattedValues.Clear();
                reference.KeyAttributes.Clear();
                
                foreach (var attribute in GetFilteredAttributes(value))
                    Context.Reference[attribute.Key] = attribute.Value;

                reference.RelatedEntities.AddRange(value.RelatedEntities);
                reference.FormattedValues.AddRange(value.FormattedValues);
                reference.KeyAttributes.AddRange(value.KeyAttributes);
            }
        }

        private IEnumerable<KeyValuePair<string, object>> GetFilteredAttributes(TEntity reference)
        {
            ColumnSet columnSet = _pluginConfig.ColumnSet;
            if (columnSet.AllColumns) return reference.Attributes;

            var filteredAttributes =
                from attr in reference.Attributes
                join col in columnSet.Columns
                    on attr.Key equals col
                select attr;

            return filteredAttributes;
        }

        private void ExecuteConfigure()
        {
            if (_configureExecuted) return;
            
            var config = Substitute.For<IPluginConfiguration<TEntity>>();
            config.ColumnSet = new ColumnSet<TEntity>();
            config.Plugin.Returns(ci => Context.Plugin);
            _pluginConfig = config;
            Configure(config);

            _configureExecuted = true;
        }
        
        protected virtual void Configure(IPluginConfiguration<TEntity> config)
        {
        }
    }
}
