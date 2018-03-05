using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces.Plugin.Actions
{
    public interface IReferenceActionContext
    {
        ITransactionContextBase TransactionContext { get; }
        ITransactionContextEntity<Entity> Input { get; }
        ITransactionContextEntity<Entity> Reference { get; }
    }
}