using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin.Strategy
{
    internal abstract class ImageEntityGetterBase
    {
        public abstract Entity Get(IPluginExecutionContext context);
    }
}
