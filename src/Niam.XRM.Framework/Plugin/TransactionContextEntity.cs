using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.Framework.Interfaces.Plugin;
using AttributeCollection = Microsoft.Xrm.Sdk.AttributeCollection;

namespace Niam.XRM.Framework.Plugin
{
    public class TransactionContextEntity<T> : EntityAccessorWrapper<T>, ITransactionContextEntity<T>
        where T : Entity
    {
        private IEntityInfo EntityInfo { get; } = Helper.Info<T>();

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

        public event AttributeChangingEventHandler AttributeChanging;

        public event AttributeChangedEventHandler AttributeChanged;

        public event AttributeChangingEventHandler FormattedValueChanging;

        public event AttributeChangedEventHandler FormattedValueChanged;

        public object this[string attributeName]
        {
            get => Get<object>(attributeName);
            set => Set(attributeName, value);
        }
        
        public TransactionContextEntity(T entity)
            : this(new EntityAccessor<T>(entity))
        {
        }

        public TransactionContextEntity(IEntityAccessor<T> accessor)
            : base(accessor)
        {
            if (EntityInfo.IsCrmSvcUtilGenerated)
            {
                ((INotifyPropertyChanging) Entity).PropertyChanging += (sender, e) =>
                {
                    var attributeName = Helper.Info<T>().GetAttributeName(e.PropertyName);
                    AttributeChanging?.Invoke(sender, new AttributeChangingEventArgs(attributeName, e.PropertyName));
                };

                ((INotifyPropertyChanged) Entity).PropertyChanged += (sender, e) =>
                {
                    var attributeName = Helper.Info<T>().GetAttributeName(e.PropertyName);
                    AttributeChanged?.Invoke(sender, new AttributeChangedEventArgs(attributeName, e.PropertyName));
                };
            }
        }

        public override void Set(MemberInfo memberInfo, object value)
        {
            if (EntityInfo.IsCrmSvcUtilGenerated)
                base.Set(memberInfo, value);
            else
                Set(EntityInfo.GetAttributeName(memberInfo.Name), value);
        }

        public override void Set(string attributeName, object value)
        {
            AttributeChanging?.Invoke(Entity, new AttributeChangingEventArgs<T>(attributeName));
            base.Set(attributeName, value);
            AttributeChanged?.Invoke(Entity, new AttributeChangedEventArgs<T>(attributeName));
        }

        public override void SetFormattedValue(string attributeName, string formattedValue)
        {
            FormattedValueChanging?.Invoke(Entity, new AttributeChangingEventArgs<T>(attributeName));
            base.SetFormattedValue(attributeName, formattedValue);
            FormattedValueChanged?.Invoke(Entity, new AttributeChangedEventArgs<T>(attributeName));
        }
    }
}
