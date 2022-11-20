using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class ToEntityOrganizationServicePipeline : ICreatePipeline, IUpdatePipeline
{
    public Guid Handle(XrmCreateRequest request, Func<Guid> next)
    {
        request.Entity = request.Entity.ToEntity<Entity>();
        return next();
    }

    public Unit Handle(XrmUpdateRequest request, Func<Unit> next)
    {
        request.Entity = request.Entity.ToEntity<Entity>();
        return next();
    }
}