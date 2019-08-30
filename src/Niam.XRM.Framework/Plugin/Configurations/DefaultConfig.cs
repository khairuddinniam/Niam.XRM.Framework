using System;
using System.Collections.Generic;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;
using Niam.XRM.Framework.Plugin.Actions;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Plugin.ServiceProviders;

namespace Niam.XRM.Framework.Plugin.Configurations
{
    internal static class DefaultConfig
    {
        public static IList<ITargetAction> TargetActions { get; } = new List<ITargetAction>
        {
            new UpdateFormattedValueEventTargetAction()
        };

        public static IList<ICurrentAction> CurrentActions { get; } = new List<ICurrentAction>
        {
            new CopyToCurrentAction(),
            new CopyValueEventCurrentAction()
        };

        public static void PluginConfigureServiceFactory<T>(IPluginConfiguration<T> config)
            where T : Entity
        {
            var crmServiceFactory = config.Container.Resolve<IOrganizationServiceFactory>();
            var serviceProvider = config.Container.Resolve<IServiceProvider>();
            var serviceFactory = new ServiceFactory(crmServiceFactory, serviceProvider, config.ServiceDecorators);
            config.Container.Register<IOrganizationServiceFactory>(serviceFactory);
        }

        public static void PluginConfigureLogging<T>(IPluginConfiguration<T> config)
            where T : Entity
        {
            config.ServiceDecorators.Add(ToEntityService.Decorate);

            switch (config.LogOption)
            {
                case PluginLogOption.Crm:
                    config.ServiceDecorators.Add(LogOrganizationService.Decorate);
                    break;

                case PluginLogOption.File:
                    config.ServiceDecorators.Add(LogOrganizationService.Decorate);
                    config.Container.Register<ITracingService>(c => new FilePluginTracingService(config.LogDirPath, c.Resolve<IPluginExecutionContext>()));
                    break;
            }
        }
    }
}