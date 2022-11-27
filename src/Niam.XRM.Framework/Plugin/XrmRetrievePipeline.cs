using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmRetrievePipeline : HandlerPipeline<XrmRetrieveRequest, Entity>, IRetrievePipeline
{
    public XrmRetrievePipeline(Func<XrmRetrieveRequest, Func<XrmRetrieveRequest, Entity>, Entity> handler) : base(handler)
    {
    }
}