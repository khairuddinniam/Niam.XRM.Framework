using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class OrganizationServiceBaseTest
    {
        [Fact]
        public void Decorate_members()
        {
            var service = Substitute.For<IOrganizationService>();
            var decorator = new DecoratorService(service);

            service.Received(0).Create(Arg.Any<Entity>());
            decorator.Create(new Entity());
            service.Received(1).Create(Arg.Any<Entity>());

            service.Received(0).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());
            decorator.Retrieve("entity_name", Guid.NewGuid(), new ColumnSet());
            service.Received(1).Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>());

            service.Received(0).Update(Arg.Any<Entity>());
            decorator.Update(new Entity());
            service.Received(1).Update(Arg.Any<Entity>());

            service.Received(0).Delete(Arg.Any<string>(), Arg.Any<Guid>());
            decorator.Delete("entity_name", Guid.NewGuid());
            service.Received(1).Delete(Arg.Any<string>(), Arg.Any<Guid>());

            service.Received(0).Execute(Arg.Any<OrganizationRequest>());
            decorator.Execute(new OrganizationRequest());
            service.Received(1).Execute(Arg.Any<OrganizationRequest>());

            service.Received(0).Associate(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<Relationship>(), Arg.Any<EntityReferenceCollection>());
            decorator.Associate("entity_name", Guid.NewGuid(), new Relationship(), new EntityReferenceCollection());
            service.Received(1).Associate(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<Relationship>(), Arg.Any<EntityReferenceCollection>());

            service.Received(0).Disassociate(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<Relationship>(), Arg.Any<EntityReferenceCollection>());
            decorator.Disassociate("entity_name", Guid.NewGuid(), new Relationship(), new EntityReferenceCollection());
            service.Received(1).Disassociate(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<Relationship>(), Arg.Any<EntityReferenceCollection>());

            service.Received(0).RetrieveMultiple(Arg.Any<QueryBase>());
            decorator.RetrieveMultiple(new QueryExpression());
            service.Received(1).RetrieveMultiple(Arg.Any<QueryBase>());
        }

        private class DecoratorService : OrganizationServiceBase
        {
            public DecoratorService(IOrganizationService service) : base(service)
            {
            }
        }
    }
}
