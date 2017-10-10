using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin.Strategy
{
    internal abstract class InputEntityGetterBase
    {
        public abstract Entity Get(IPluginExecutionContext context);
    }
}
