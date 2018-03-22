using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin.Actions;
using Niam.XRM.Framework.Plugin.Strategy;

namespace Niam.XRM.Framework.Plugin
{
    internal class TransactionContext<T> : TransactionContextBase<T>, ITransactionContext<T>, IDisposable
        where T : Entity
    {
        private readonly Lazy<TransactionContextEntity<T>> _target;
        private readonly Lazy<TransactionContextEntity<T>> _current;
        private readonly Lazy<EntityAccessor<T>> _initialEntity;
        
        public ITransactionContextEntity<T> Target => _target.Value;

        public ITransactionContextEntity<T> Current => _current.Value;

        internal EntityAccessor<T> Initial => _initialEntity.Value;

        IEntityGetter<T> ITransactionContext<T>.Initial => Initial;

        public TransactionContext(IServiceProvider serviceProvider, ITransactionContextConfiguration<T> config = null)
            : base(serviceProvider, config)
        {
            _target = new Lazy<TransactionContextEntity<T>>(GetTargetContextEntity);
            _current = new Lazy<TransactionContextEntity<T>>(GetCurrentContextEntity);
            _initialEntity = new Lazy<EntityAccessor<T>>(GetInitialEntity);
        }
        
        private TransactionContextEntity<T> GetTargetContextEntity()
        {
            var target = GetTargetEntity().ToEntity<T>();
            var txTarget = new TransactionContextEntity<T>(target);

            var actionContext = new TargetActionContext
            {
                TransactionContext = this,
                Target = txTarget
            };

            foreach (var action in Config.TargetActions)
            {
                if (action.CanExecute(actionContext))
                    action.Execute(actionContext);
            }

            return txTarget;
        }

        private Entity GetTargetEntity()
        {
            var message = PluginExecutionContext.MessageName;
            var handler = TargetEntityGetter.GetHandler(message);
            return handler.Get(PluginExecutionContext);
        }

        private TransactionContextEntity<T> GetCurrentContextEntity()
        {
            var current = GetCurrentEntity();
            var currentAccessor = new FormattedValueCurrentAccessor<T>(Target.Entity, current, this);
            var txCurrent = new TransactionContextEntity<T>(currentAccessor);

            var actionContext = new CurrentActionContext
            {
                TransactionContext = this,
                Target = Target,
                Current = txCurrent
            };

            foreach (var action in Config.CurrentActions)
            {
                if (action.CanExecute(actionContext))
                    action.Execute(actionContext);
            }

            return txCurrent;
        }

        private T GetCurrentEntity()
        {
            ColumnSet columnSet = Config.ColumnSet ?? new ColumnSet();
            if (columnSet.AllColumns) return Initial.Entity.Copy();
            if (!columnSet.Columns.Any()) return Target.Entity.Copy();
            
            var pairs = Target.Attributes.Keys.FullOuterJoin(columnSet.Columns, target => target, column => column,
                (target, column, key) => new { Target = target, Column = column });
            return pairs.Any(p => p.Target == null && p.Column != null) ? Initial.Entity.Copy() : Target.Entity.Copy();
        }

        private EntityAccessor<T> GetInitialEntity()
        {
            var message = PluginExecutionContext.MessageName;
            var handler = EntityGetter.GetHandler(message);
            return handler.Get(this, Config).ToEntityAccessor<T>();
        }

        public void Dispose()
        {
        }
    }
}
