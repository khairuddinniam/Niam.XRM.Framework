using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces.Plugin
{
    public interface ITransactionContext<out T> : ITransactionContextBase
        where T : Entity
    {
        ITransactionContextEntity<T> Target { get; }
        ITransactionContextEntity<T> Current { get; }

        IEntityGetter<T> Initial { get; }
    }
}
