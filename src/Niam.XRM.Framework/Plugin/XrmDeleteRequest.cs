using System;

namespace Niam.XRM.Framework.Plugin;

public class XrmDeleteRequest
{
    public string EntityName { get; }
    public Guid Id { get; }

    public XrmDeleteRequest(string entityName, Guid id)
    {
        EntityName = entityName;
        Id = id;
    }    
}