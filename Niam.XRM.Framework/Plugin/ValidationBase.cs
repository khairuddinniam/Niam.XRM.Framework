using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin
{
    public abstract class ValidationBase : ValidationBase<Entity>
    {
        protected ValidationBase(ITransactionContext<Entity> context) : base(context)
        {
        }
    }

    public abstract class ValidationBase<T> : ValidationBase<T, EntityWrapper<T>>
        where T : Entity
    {
        protected ValidationBase(ITransactionContext<T> context) : base(context)
        {
        }
    }

    public abstract class ValidationBase<TE, TW> : CommandBase<TE, TW>, IValidation
        where TE : Entity
        where TW : EntityWrapper<TE>
    {
        protected ValidationBase(ITransactionContext<TE> context) : base(context)
        {
        }

        public void Validate() => HandleValidate();

        protected abstract void HandleValidate();
    }
}
