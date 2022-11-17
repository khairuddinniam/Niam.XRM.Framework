using System;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework.Plugin
{
    public class XrmCreatePipeline : IPipeline<XrmCreateRequest, Guid>
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
}