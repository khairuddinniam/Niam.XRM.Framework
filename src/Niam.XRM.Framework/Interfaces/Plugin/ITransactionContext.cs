using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces.Plugin
{
    public interface ITransactionContext<out T> : ITransactionContextBase
        where T : Entity
    {
        ITransactionContextEntity<T> Input { get; }
        ITransactionContextEntity<T> Reference { get; }

        IEntityGetter<T> Initial { get; }
    }
}
