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
        protected TE Input => Context.Input.Entity;

        protected OperationBase(ITransactionContext<TE> context) : base(context)
        {
        }

        public void Execute() => ExecuteCore();

        protected override sealed void HandleExecuteCore() => HandleExecute();

        protected abstract void HandleExecute();
        
        protected void Set(string attributeName, object value) 
            => Context.Input.Set(attributeName, value);

        protected void Set(string attributeName, IAttributeValueProvider attributeValueProvider)
            => Context.Input.Set(attributeName, attributeValueProvider);

        protected void Set(string attributeName, IValueProvider valueProvider)
            => Context.Input.Set(attributeName, valueProvider);

        protected void Set<TV>(Expression<Func<TE, TV>> attribute, TV value) 
            => Context.Input.Set(attribute, value);

        protected void Set<TV>(Expression<Func<TE, TV>> attribute, IAttributeValueProvider<TE, TV> attributeValueProvider)
            => Context.Input.Set(attribute, attributeValueProvider);

        protected void Set<TV>(Expression<Func<TE, TV>> attribute, IValueProvider<TV> valueProvider)
            => Context.Input.Set(attribute, valueProvider);

        protected void Set<TV>(Expression<Func<TE, TV?>> attribute, IValueProvider<TV> valueProvider)
            where TV : struct => Context.Input.Set(attribute, valueProvider);

        protected void Set(Expression<Func<TE, OptionSetValue>> attribute, Enum value) 
            => Context.Input.Set(attribute, value);

        protected void Set(Expression<Func<TE, OptionSetValue>> attribute, int value) 
            => Context.Input.Set(attribute, value);

        protected void Set(Expression<Func<TE, Money>> attribute, decimal value) 
            => Context.Input.Set(attribute, value);

        protected void Set(Expression<Func<TE, Money>> attribute, IValueProvider<decimal> valueProvider)
            => Context.Input.Set(attribute, valueProvider);
    }
}
