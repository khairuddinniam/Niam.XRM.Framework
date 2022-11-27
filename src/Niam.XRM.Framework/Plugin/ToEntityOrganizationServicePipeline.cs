using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class ToEntityOrganizationServicePipeline : ICreatePipeline, IUpdatePipeline
{
    public Guid Handle(XrmCreateRequest request, Func<XrmCreateRequest, Guid> next)
    {
        return next(new XrmCreateRequest(request.Entity.ToEntity<Entity>()));
    }

    public Unit Handle(XrmUpdateRequest request, Func<XrmUpdateRequest, Unit> next)
    {
        return next(new XrmUpdateRequest(request.Entity.ToEntity<Entity>()));
    }
}