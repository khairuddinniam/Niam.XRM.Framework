using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework.Plugin;

public class XrmRetrieveMultipleRequest
{
    public QueryBase Query { get; }

    public XrmRetrieveMultipleRequest(QueryBase query)
    {
        Query = query;
    }
}