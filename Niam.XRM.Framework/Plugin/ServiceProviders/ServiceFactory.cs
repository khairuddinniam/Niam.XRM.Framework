using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin.ServiceProviders
{
    internal class ServiceFactory : IOrganizationServiceFactory
    {
        private readonly IServiceProvider _provider;
        private readonly IList<Func<IOrganizationService, IServiceProvider, IOrganizationService>> _serviceDecorators;
        private readonly IOrganizationServiceFactory _factory;

        public ServiceFactory(IOrganizationServiceFactory factory, 
            IServiceProvider provider, 
            IList<Func<IOrganizationService, IServiceProvider, IOrganizationService>> serviceDecorators)
        {
            _factory = factory;
            _provider = provider;
            _serviceDecorators = serviceDecorators;
        }

        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            var service = _factory.CreateOrganizationService(userId);
            return _serviceDecorators.Aggregate(service, (current, decorator) => decorator(current, _provider));
        }
    }
}
