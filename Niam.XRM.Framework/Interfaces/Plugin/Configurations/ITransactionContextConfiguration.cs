using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;

namespace Niam.XRM.Framework.Interfaces.Plugin.Configurations
{
    public interface ITransactionContextConfiguration<T>
        where T : Entity
    {
        IList<IInputAction> InputActions { get; }
        IList<IReferenceAction> ReferenceActions { get; }
        IPluginBase Plugin { get; set; }
        ColumnSet<T> ColumnSet { get; set; } 
    }
}
