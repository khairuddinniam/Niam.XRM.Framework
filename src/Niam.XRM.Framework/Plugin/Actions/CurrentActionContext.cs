using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;

namespace Niam.XRM.Framework.Plugin.Actions
{
    public class CurrentActionContext : ICurrentActionContext
    {
        public ITransactionContextBase TransactionContext { get; set; }
        public ITransactionContextEntity<Entity> Target { get; set; }
        public ITransactionContextEntity<Entity> Current { get; set; }
    }
}
