using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces.Plugin.Actions
{
    public interface IInputActionContext
    {
        ITransactionContextBase TransactionContext { get; }
        ITransactionContextEntity<Entity> Input { get; }
    }
}