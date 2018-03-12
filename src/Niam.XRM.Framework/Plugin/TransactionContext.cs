using System;
using Microsoft.Xrm.Sdk;
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
        private TransactionContextEntity<T> _input;
        private TransactionContextEntity<T> _reference;
        private EntityAccessor<T> _initialEntity;
        
        public ITransactionContextEntity<T> Input => _input ?? (_input = GetInputContextEntity());

        public ITransactionContextEntity<T> Reference => _reference ?? (_reference = GetReferenceContextEntity());

        internal EntityAccessor<T> Initial => _initialEntity ?? (_initialEntity = GetInitialEntity());

        IEntityGetter<T> ITransactionContext<T>.Initial => Initial;

        public TransactionContext(IServiceProvider serviceProvider, ITransactionContextConfiguration<T> config = null)
            : base(serviceProvider, config)
        {
        }
        
        private TransactionContextEntity<T> GetInputContextEntity()
        {
            var input = GetInputEntity().ToEntity<T>();
            var txInput = new TransactionContextEntity<T>(input);

            var actionContext = new InputActionContext
            {
                TransactionContext = this,
                Input = txInput
            };

            foreach (var action in Config.InputActions)
            {
                if (action.CanExecute(actionContext))
                    action.Execute(actionContext);
            }

            return txInput;
        }

        private Entity GetInputEntity()
        {
            var message = PluginExecutionContext.MessageName;
            var handler = TargetEntityGetter.GetHandler(message);
            return handler.Get(PluginExecutionContext);
        }

        private TransactionContextEntity<T> GetReferenceContextEntity()
        {
            var reference = Initial.Entity.Copy();
            var referenceAccessor = new FormattedValueReferenceAccessor<T>(Input.Entity, reference, this);
            var txReference = new TransactionContextEntity<T>(referenceAccessor);

            var actionContext = new ReferenceActionContext
            {
                TransactionContext = this,
                Input = Input,
                Reference = txReference
            };

            foreach (var action in Config.ReferenceActions)
            {
                if (action.CanExecute(actionContext))
                    action.Execute(actionContext);
            }

            return txReference;
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
