using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin;

public class XrmUpdateRequest
{
    public Entity Entity { get; }

    public XrmUpdateRequest(Entity entity)
    {
        Entity = entity;
    }    
}