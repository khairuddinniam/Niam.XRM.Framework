using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces.Plugin
{
    public interface IContextBase
    {
        IPluginExecutionContext PluginExecutionContext { get; }
        ITracingService TracingService { get; }
        IDictionary<string, object> SessionStorage { get; }
    }
}
