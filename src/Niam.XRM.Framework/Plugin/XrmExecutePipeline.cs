using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmExecutePipeline : HandlerPipeline<OrganizationRequest, OrganizationResponse>, IExecutePipeline
{
    public XrmExecutePipeline(
        Func<OrganizationRequest, Func<OrganizationRequest, OrganizationResponse>, OrganizationResponse> handler) : 
        base(handler)
    {
    }
}