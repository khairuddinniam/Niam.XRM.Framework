using System.Collections.Generic;

namespace Niam.XRM.Framework.Interfaces
{
    public interface IEntityInfo
    {
        string LogicalName { get; }
        string PrimaryNameAttribute { get; }
        IDictionary<string, string> Attributes { get; }
        int? StateCodeActiveValue { get; }
    }
}