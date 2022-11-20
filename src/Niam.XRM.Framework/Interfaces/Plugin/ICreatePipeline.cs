using System;
using Niam.XRM.Framework.Plugin;

namespace Niam.XRM.Framework.Interfaces.Plugin;

public interface ICreatePipeline : IPipeline<XrmCreateRequest, Guid>
{
}