using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;

namespace Niam.XRM.Framework.Plugin.Actions
{
    public class ReferenceActionContext : IReferenceActionContext
    {
        public ITransactionContextBase TransactionContext { get; set; }
        public ITransactionContextEntity<Entity> Input { get; set; }
        public ITransactionContextEntity<Entity> Reference { get; set; }
    }
}
