using System;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin;

public class XrmDisassociateRequest
{
    public string EntityName { get; }
    public Guid EntityId { get; }
    public Relationship Relationship { get; }
    public EntityReferenceCollection RelatedEntities { get; }

    public XrmDisassociateRequest(string entityName, Guid entityId, Relationship relationship, 
        EntityReferenceCollection relatedEntities)
    {
        EntityName = entityName;
        EntityId = entityId;
        Relationship = relationship;
        RelatedEntities = relatedEntities;
    }    
}