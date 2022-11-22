using System;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmDeletePipeline : IDeletePipeline
{
    private readonly Func<XrmDeleteRequest, Func<Unit>, Unit> _handler;

    public XrmDeletePipeline(Func<XrmDeleteRequest, Func<Unit>, Unit> handler)
    {
        _handler = handler;
    }
            
    public Unit Handle(XrmDeleteRequest request, Func<Unit> next)
    {
        return _handler(request, next);
    }
}