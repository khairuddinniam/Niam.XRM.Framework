using System;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin;

public class XrmCreatePipeline : HandlerPipeline<XrmCreateRequest, Guid>, ICreatePipeline
{
    public XrmCreatePipeline(Func<XrmCreateRequest, Func<XrmCreateRequest, Guid>, Guid> handler) : base(handler)
    {
    }
}