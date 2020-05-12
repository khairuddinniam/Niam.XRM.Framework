using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Niam.XRM.Framework.Tests
{
    public class HelperOrganizationServiceExecuteTests
    {
        private readonly ITestOutputHelper _output;

        public HelperOrganizationServiceExecuteTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
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
