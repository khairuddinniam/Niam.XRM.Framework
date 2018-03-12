using System;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin
{
    public abstract class OperationBase : OperationBase<Entity>
    {
        protected OperationBase(ITransactionContext<Entity> context) : base(context)
        {
        }
    }

    public abstract class OperationBase<T> : OperationBase<T, EntityWrapper<T>>
        where T : Entity
    {
        protected OperationBase(ITransactionContext<T> context) : base(context)
        {
        }
    }

    public abstract class OperationBase<TE, TW> : CommandBase<TE, TW>, IOperation
        where TE : Entity
        where TW : EntityWrapper<TE>
    {
        protected TE Input => Context.Target.Entity;

        protected OperationBase(ITransactionContext<TE> context) : base(context)
        {
        }

        public void Execute() => ExecuteCore();

        protected sealed override void HandleExecuteCore() => HandleExecute();

        protected abstract void HandleExecute();
        
        protected void Set(string attributeName, object value) 
            => Context.Target.Set(attributeName, value);

        protected void Set(string attributeName, IAttributeValueProvider attributeValueProvider)
            => Context.Target.Set(attributeName, attributeValueProvider);

        protected void Set(string attributeName, IValueProvider valueProvider)
            => Context.Target.Set(attributeName, valueProvider);

        protected void Set<TV>(Expression<Func<TE, TV>> attribute, TV value) 
            => Context.Target.Set(attribute, value);

        protected void Set<TV>(Expression<Func<TE, TV>> attribute, IAttributeValueProvider<TE, TV> attributeValueProvider)
            => Context.Target.Set(attribute, attributeValueProvider);

        protected void Set<TV>(Expression<Func<TE, TV>> attribute, IValueProvider<TV> valueProvider)
            => Context.Target.Set(attribute, valueProvider);

        protected void Set<TV>(Expression<Func<TE, TV?>> attribute, IValueProvider<TV> valueProvider)
            where TV : struct => Context.Target.Set(attribute, valueProvider);

        protected void Set(Expression<Func<TE, OptionSetValue>> attribute, Enum value) 
            => Context.Target.Set(attribute, value);

        protected void Set(Expression<Func<TE, OptionSetValue>> attribute, int value) 
            => Context.Target.Set(attribute, value);

        protected void Set(Expression<Func<TE, Money>> attribute, decimal value) 
            => Context.Target.Set(attribute, value);

        protected void Set(Expression<Func<TE, Money>> attribute, IValueProvider<decimal> valueProvider)
            => Context.Target.Set(attribute, valueProvider);
    }
}
