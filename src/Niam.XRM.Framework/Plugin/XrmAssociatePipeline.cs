using System;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmAssociatePipeline : HandlerPipeline<XrmAssociateRequest, Unit>, IAssociatePipeline
{
    public XrmAssociatePipeline(Func<XrmAssociateRequest, Func<XrmAssociateRequest, Unit>, Unit> handler) :
        base(handler)
    {
    }
}