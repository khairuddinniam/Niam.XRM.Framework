using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmExecutePipeline : IExecutePipeline
{
    private readonly Func<OrganizationRequest, Func<OrganizationResponse>, OrganizationResponse> _handler;

    public XrmExecutePipeline(Func<OrganizationRequest, Func<OrganizationResponse>, OrganizationResponse> handler)
    {
        _handler = handler;
    }
            
    public OrganizationResponse Handle(OrganizationRequest request, Func<OrganizationResponse> next)
    {
        return _handler(request, next);
    }
}