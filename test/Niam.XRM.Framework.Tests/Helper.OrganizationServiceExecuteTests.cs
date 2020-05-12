using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Data;
using NSubstitute;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class HelperOrganizationServiceExecuteTests
    {
        [Fact]
        public void Can_cast_result_from_execute()
        {
            var service = Substitute.For<IOrganizationService>();
			var response = new RetrieveEntityResponse();
            service.Execute(Arg.Any<OrganizationRequest>()).Returns(response);
			
			var request = new RetrieveEntityRequest();
			Assert.Same(response, service.Execute<RetrieveEntityResponse>(request));
        }

        [Fact]
        public void Execute_correct_pairs()
        {
            var service = Substitute.For<IOrganizationService>();
            service.Execute(Arg.Any<OrganizationRequest>()).Returns((OrganizationResponse) null);

            QualifyLeadResponse qualifyLeadResponse = service.Send(new QualifyLeadRequest());
            WhoAmIResponse whoAmIResponse = service.Send(new WhoAmIRequest());
        }
    }
}
