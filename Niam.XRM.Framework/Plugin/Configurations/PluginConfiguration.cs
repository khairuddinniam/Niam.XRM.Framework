using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders;

namespace Niam.XRM.Framework.Plugin.Configurations
{
    internal class PluginConfiguration<T> : IPluginConfiguration<T>
        where T : Entity
    {
        public IContainer Container { get; }
        public IList<Func<IOrganizationService, IServiceProvider, IOrganizationService>> ServiceDecorators { get; } 
            = new List<Func<IOrganizationService, IServiceProvider, IOrganizationService>>();
        public ITransactionContextConfiguration<T> TransactionContext { get; }

        public PluginConfiguration(IContainer container, ITransactionContextConfiguration<T> context)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            TransactionContext = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}