using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Plugin;

namespace Niam.XRM.Framework.Interfaces.Plugin;

public interface IRetrieveMultiplePipeline : IPipeline<XrmRetrieveMultipleRequest, EntityCollection>
{
}