﻿using Microsoft.Xrm.Sdk;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
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

                var filteredAttributes =
                    from attr in value.Attributes
                    join col in _pluginConfig.TransactionContext.ColumnSet.Columns
                        on attr.Key equals col
                    select attr;
                foreach (var attribute in filteredAttributes)
                    Context.Reference[attribute.Key] = attribute.Value;

                reference.RelatedEntities.AddRange(value.RelatedEntities);
                reference.FormattedValues.AddRange(value.FormattedValues);
                reference.KeyAttributes.AddRange(value.KeyAttributes);
            }
        }

        private void ExecuteConfigure()
        {
            if (_configureExecuted) return;

            var contextConfig = Substitute.For<ITransactionContextConfiguration<TEntity>>();
            contextConfig.ColumnSet = new ColumnSet<TEntity>();
            contextConfig.Plugin.Returns(ci => Context.Plugin);
            var config = Substitute.For<IPluginConfiguration<TEntity>>();
            config.TransactionContext.Returns(contextConfig);
            _pluginConfig = config;
            Configure(config);

            _configureExecuted = true;
        }
        
        protected virtual void Configure(IPluginConfiguration<TEntity> config)
        {
        }
    }
}
