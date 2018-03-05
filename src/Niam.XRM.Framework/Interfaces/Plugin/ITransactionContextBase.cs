using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Plugin.Configurations;

namespace Niam.XRM.Framework.Interfaces.Plugin
{
    public interface ITransactionContextBase : IContextBase
    {
        IPluginBase Plugin { get; }
        IOrganizationService Service { get; }
        IOrganizationService SystemService { get; }
        IOrganizationServiceFactory ServiceFactory { get; }
        IServiceProvider ServiceProvider { get; }
        PluginLogOption LogOption { get; }
    }
}
