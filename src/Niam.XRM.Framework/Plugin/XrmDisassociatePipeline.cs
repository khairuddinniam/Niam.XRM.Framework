using System;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmDisassociatePipeline : HandlerPipeline<XrmDisassociateRequest, Unit>, IDisassociatePipeline
{
    public XrmDisassociatePipeline(Func<XrmDisassociateRequest, Func<XrmDisassociateRequest, Unit>, Unit> handler) :
        base(handler)
    {
    }
}