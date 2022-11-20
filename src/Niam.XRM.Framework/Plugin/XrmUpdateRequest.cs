using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin;

public class XrmUpdateRequest
{
    public Entity Entity { get; internal set; }

    public XrmUpdateRequest(Entity entity)
    {
        Entity = entity;
    }    
}