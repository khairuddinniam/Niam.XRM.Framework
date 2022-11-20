using System;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmUpdatePipeline : IUpdatePipeline
{
    private readonly Func<XrmUpdateRequest, Func<Unit>, Unit> _handler;

    public XrmUpdatePipeline(Func<XrmUpdateRequest, Func<Unit>, Unit> handler)
    {
        _handler = handler;
    }
            
    public Unit Handle(XrmUpdateRequest request, Func<Unit> next)
    {
        return _handler(request, next);
    }
}