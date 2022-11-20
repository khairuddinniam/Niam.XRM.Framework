using System;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework.Plugin;

public class XrmRetrieveRequest
{
    public string EntityName { get; }
    public Guid Id { get; }
    public ColumnSet ColumnSet { get; }

    public XrmRetrieveRequest(string entityName, Guid id, ColumnSet columnSet)
    {
        EntityName = entityName;
        Id = id;
        ColumnSet = columnSet;
    }    
}