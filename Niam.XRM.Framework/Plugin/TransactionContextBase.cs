using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin.Configurations;

namespace Niam.XRM.Framework.Plugin
{
    internal abstract class TransactionContextBase<T> : ContextBase, ITransactionContextBase
        where T : Entity
    {
        private IOrganizationService _service;
        private IOrganizationService _adminService;
        private IOrganizationServiceFactory _serviceFactory;

        protected ITransactionContextConfiguration<T> Config { get; }

        public IPluginBase Plugin => Config.Plugin;

        public IOrganizationService Service => _service ?? (_service = ServiceFactory.CreateOrganizationService(PluginExecutionContext.UserId));

        public IOrganizationService SystemService => _adminService ?? (_adminService = ServiceFactory.CreateOrganizationService(null));

        public IOrganizationServiceFactory ServiceFactory => _serviceFactory ?? (_serviceFactory = ServiceProvider.GetService<IOrganizationServiceFactory>());

        protected TransactionContextBase(IServiceProvider serviceProvider, ITransactionContextConfiguration<T> config) 
            : base(serviceProvider)
        {
            Config = config ?? new TransactionContextConfiguration<T>();
        }
    }
}
