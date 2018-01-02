using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Plugin.ServiceProviders;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.ServiceProviders
{
    public class ServiceFactoryTests
    {
        [Fact]
        public void Can_decorate_organization_service()
        {
            var provider = Substitute.For<IServiceProvider>();

            var originalService = Substitute.For<IOrganizationService>();
            var factory = Substitute.For<IOrganizationServiceFactory>();
            factory.CreateOrganizationService(Arg.Any<Guid?>())
                .Returns(originalService);

            var serviceDecorators = new List<Func<IOrganizationService, IServiceProvider, IOrganizationService>>();
            var fxFactory = new ServiceFactory(factory, provider, serviceDecorators);

            var firstDecoratorService = Substitute.For<IOrganizationService>();
            var firstDecoratorTimesCalled = 0;
            Func<IOrganizationService, IServiceProvider, IOrganizationService> firstDecorator =
                (service, context) =>
                {
                    Assert.Same(originalService, service);
                    firstDecoratorTimesCalled++;
                    return firstDecoratorService;
                };

            serviceDecorators.Add(firstDecorator);
            var createdService = fxFactory.CreateOrganizationService(Guid.NewGuid());
            Assert.Same(firstDecoratorService, createdService);
            Assert.Equal(1, firstDecoratorTimesCalled);

            var secondDecoratorService = Substitute.For<IOrganizationService>();
            var secondDecoratorTimesCalled = 0;
            Func<IOrganizationService, IServiceProvider, IOrganizationService> secondDecorator = 
                (service, context) =>
                {
                    Assert.Same(firstDecoratorService, service);
                    secondDecoratorTimesCalled++;
                    return secondDecoratorService;
                };

            serviceDecorators.Add(secondDecorator);
            createdService = fxFactory.CreateOrganizationService(Guid.NewGuid());
            Assert.Same(secondDecoratorService, createdService);
            Assert.Equal(2, firstDecoratorTimesCalled);
            Assert.Equal(1, secondDecoratorTimesCalled);
        }
    }
}
