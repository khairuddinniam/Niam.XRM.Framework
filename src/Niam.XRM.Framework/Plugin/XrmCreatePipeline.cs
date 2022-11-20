using System;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmCreatePipeline : ICreatePipeline
{
    private readonly Func<XrmCreateRequest, Func<Guid>, Guid> _handler;

    public XrmCreatePipeline(Func<XrmCreateRequest, Func<Guid>, Guid> handler)
    {
        _handler = handler;
    }
            
    public Guid Handle(XrmCreateRequest request, Func<Guid> next)
    {
        return _handler(request, next);
    }
}