using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;

namespace Niam.XRM.Framework.Plugin.Configurations
{
    public class TransactionContextConfiguration<T> : ITransactionContextConfiguration<T>
        where T : Entity
    {
        public IList<IInputAction> InputActions { get; } = new List<IInputAction>(DefaultConfig.InputActions);

        public IList<IReferenceAction> ReferenceActions { get; } = new List<IReferenceAction>(DefaultConfig.ReferenceActions);

        public IPluginBase Plugin { get; }

        public ColumnSet<T> ColumnSet { get; set; } = new ColumnSet<T>();

        public TransactionContextConfiguration(IPluginBase plugin = null)
        {
            Plugin = plugin;
        }
    }
}