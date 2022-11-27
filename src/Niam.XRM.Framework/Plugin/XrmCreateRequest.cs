using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin;

public class XrmCreateRequest
{
    public Entity Entity { get; }

    public XrmCreateRequest(Entity entity)
    {
        Entity = entity;
    }    
}