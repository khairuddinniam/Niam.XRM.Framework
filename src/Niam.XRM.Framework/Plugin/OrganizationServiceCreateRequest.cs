using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin
{
    public class OrganizationServiceCreateRequest
    {
        public Entity Entity { get; }

        public OrganizationServiceCreateRequest(Entity entity)
        {
            Entity = entity;
        }    
    }
}