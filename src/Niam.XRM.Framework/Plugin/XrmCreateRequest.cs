using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin;

public class XrmCreateRequest
{
    public Entity Entity { get; internal set; }

    public XrmCreateRequest(Entity entity)
    {
        Entity = entity;
    }    
}