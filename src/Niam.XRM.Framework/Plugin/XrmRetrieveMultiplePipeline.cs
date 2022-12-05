using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmRetrieveMultiplePipeline : HandlerPipeline<XrmRetrieveMultipleRequest, EntityCollection>,
    IRetrieveMultiplePipeline
{
    public XrmRetrieveMultiplePipeline(
        Func<XrmRetrieveMultipleRequest, Func<XrmRetrieveMultipleRequest, EntityCollection>, EntityCollection> handler)
        : base(handler)
    {
    }
}