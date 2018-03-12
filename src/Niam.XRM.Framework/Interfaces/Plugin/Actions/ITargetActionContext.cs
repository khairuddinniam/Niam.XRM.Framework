using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces.Plugin.Actions
{
    public interface ITargetActionContext
    {
        ITransactionContextBase TransactionContext { get; }
        ITransactionContextEntity<Entity> Target { get; }
    }
}