using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;

namespace Niam.XRM.Framework.Plugin.Actions
{
    public class TargetActionContext : ITargetActionContext
    {
        public ITransactionContextBase TransactionContext { get; set; }
        public ITransactionContextEntity<Entity> Target { get; set; }
    }
}
