using System;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmUpdatePipeline : HandlerPipeline<XrmUpdateRequest, Unit>, IUpdatePipeline
{
    public XrmUpdatePipeline(Func<XrmUpdateRequest, Func<XrmUpdateRequest, Unit>, Unit> handler) : base(handler)
    {
    }
}