using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmRetrievePipeline : IRetrievePipeline
{
    private readonly Func<XrmRetrieveRequest, Func<Entity>, Entity> _handler;

    public XrmRetrievePipeline(Func<XrmRetrieveRequest, Func<Entity>, Entity> handler)
    {
        _handler = handler;
    }
            
    public Entity Handle(XrmRetrieveRequest request, Func<Entity> next)
    {
        return _handler(request, next);
    }
}