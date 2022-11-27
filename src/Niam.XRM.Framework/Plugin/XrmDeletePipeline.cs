using System;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmDeletePipeline : HandlerPipeline<XrmDeleteRequest, Unit>, IDeletePipeline
{
    public XrmDeletePipeline(Func<XrmDeleteRequest, Func<XrmDeleteRequest, Unit>, Unit> handler) : base(handler)
    {
    }
}