using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces.Plugin.Actions
{
    public interface ICurrentActionContext
    {
        ITransactionContextBase TransactionContext { get; }
        ITransactionContextEntity<Entity> Target { get; }
        ITransactionContextEntity<Entity> Current { get; }
    }
}