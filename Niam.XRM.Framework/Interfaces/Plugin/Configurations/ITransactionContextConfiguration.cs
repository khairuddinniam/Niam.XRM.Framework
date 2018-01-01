using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;
using Niam.XRM.Framework.Plugin.Configurations;

namespace Niam.XRM.Framework.Interfaces.Plugin.Configurations
{
    public interface ITransactionContextConfiguration<T>
        where T : Entity
    {
        IList<IInputAction> InputActions { get; }
        IList<IReferenceAction> ReferenceActions { get; }
        IPluginBase Plugin { get; }
        ColumnSet<T> ColumnSet { get; set; }
        PluginLogOption LogOption { get; }
        string LogDirPath { get; }
    }
}
