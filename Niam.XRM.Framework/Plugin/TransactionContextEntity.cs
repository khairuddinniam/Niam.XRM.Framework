using System;
using System.ComponentModel;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.Framework.Interfaces.Plugin;
using AttributeCollection = Microsoft.Xrm.Sdk.AttributeCollection;

namespace Niam.XRM.Framework.Plugin
{
    public class TransactionContextEntity<T> : EntityAccessorWrapper<T>, ITransactionContextEntity<T>, IDisposable
        where T : Entity
    {
        private readonly Lazy<bool> _isEarlyBound;
        private EventHandlerList _eventHandlers = new EventHandlerList();
        
        public EventHandlerList EventHandlers
        {
            get => _eventHandlers;
            set
            {
                _eventHandlers = value;
                if (_isEarlyBound.Value)
                {
                    ((INotifyPropertyChanging) Entity).PropertyChanging += GetAttributeChangingEventHandler();
                    ((INotifyPropertyChanged) Entity).PropertyChanged += GetAttributeChangedEventHandler();
                }
            }
        }

        public Guid Id
        {
            get => Entity.Id;
            set => Entity.Id = value;
        }

        public string LogicalName
        {
            get => Entity.LogicalName;
            set => Entity.LogicalName = value;
        }

        public AttributeCollection Attributes
        {
            get => Entity.Attributes;
            set => Entity.Attributes = value;
        }

        public virtual event PropertyChangingEventHandler AttributeChanging
        {
            add
            {
                EventHandlers.AddHandler(TxContextEntity.AttributeChangingEventKey, value);
                if (_isEarlyBound.Value)
                    ((INotifyPropertyChanging) Entity).PropertyChanging += value;
            }

            remove
            {
                EventHandlers.RemoveHandler(TxContextEntity.AttributeChangingEventKey, value);
                if (_isEarlyBound.Value)
                    ((INotifyPropertyChanging) Entity).PropertyChanging -= value;
            }
        }

        public virtual event PropertyChangedEventHandler AttributeChanged
        {
            add
            {
                EventHandlers.AddHandler(TxContextEntity.AttributeChangedEventKey, value);
                if (_isEarlyBound.Value)
                    ((INotifyPropertyChanged) Entity).PropertyChanged += value;
            }

            remove
            {
                EventHandlers.RemoveHandler(TxContextEntity.AttributeChangedEventKey, value);
                if (_isEarlyBound.Value)
                    ((INotifyPropertyChanged) Entity).PropertyChanged -= value;
            }
        }
        
        public object this[string attributeName]
        {
            get => Get<object>(attributeName);
            set => Set(attributeName, value);
        }

        private void OnAttributeChanging(string attributeName)
        {
            var handler = GetAttributeChangingEventHandler();
            handler?.Invoke(Entity, new PropertyChangingEventArgs(attributeName));
        }

        protected PropertyChangingEventHandler GetAttributeChangingEventHandler() =>
            (PropertyChangingEventHandler)EventHandlers[TxContextEntity.AttributeChangingEventKey];

        private void OnAttributeChanged(string attributeName)
        {
            var handler = GetAttributeChangedEventHandler();
            handler?.Invoke(Entity, new PropertyChangedEventArgs(attributeName));
        }

        protected PropertyChangedEventHandler GetAttributeChangedEventHandler() =>
            (PropertyChangedEventHandler)EventHandlers[TxContextEntity.AttributeChangedEventKey];
        
        public TransactionContextEntity(T entity)
            : this(new EntityAccessor<T>(entity))
        {
        }

        public TransactionContextEntity(IEntityAccessor<T> accessor)
            : base(accessor)
        {
            _isEarlyBound = new Lazy<bool>(() => Entity.IsEarlyBoundEntity());
        }

        public override void Set(string attributeName, object value)
        {
            OnAttributeChanging(attributeName);
            base.Set(attributeName, value);
            OnAttributeChanged(attributeName);
        }
        
        public void Dispose()
        {
            _eventHandlers?.Dispose();
        }
    }
}
