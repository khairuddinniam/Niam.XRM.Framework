using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework
{
    public static partial class Helper
    {
        public static T Execute<T>(this IOrganizationService service, OrganizationRequest request)
            where T : OrganizationResponse => (T) service.Execute(request);
        
        public static LoseOpportunityResponse Send(this IOrganizationService service, LoseOpportunityRequest request)
            => service.Execute<LoseOpportunityResponse>(request);

        public static QualifyLeadResponse Send(this IOrganizationService service, QualifyLeadRequest request)
            => service.Execute<QualifyLeadResponse>(request);

        public static WhoAmIResponse Send(this IOrganizationService service, WhoAmIRequest request)
            => service.Execute<WhoAmIResponse>(request);
        
        public static WinOpportunityResponse Send(this IOrganizationService service, WinOpportunityRequest request)
            => service.Execute<WinOpportunityResponse>(request);
    }
}
