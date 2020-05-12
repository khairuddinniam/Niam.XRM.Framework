using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework
{
    public static partial class Helper
    {
        public static T Execute<T>(this IOrganizationService service, OrganizationRequest request)
            where T : OrganizationResponse => (T) service.Execute(request);
        
        public static AddAppComponentsResponse Send(this IOrganizationService service, AddAppComponentsRequest request)
            => service.Execute<AddAppComponentsResponse>(request);
        
        public static AddChannelAccessProfilePrivilegesResponse Send(this IOrganizationService service, AddChannelAccessProfilePrivilegesRequest request)
            => service.Execute<AddChannelAccessProfilePrivilegesResponse>(request);
        
        public static AddItemCampaignActivityResponse Send(this IOrganizationService service, AddItemCampaignActivityRequest request)
            => service.Execute<AddItemCampaignActivityResponse>(request);
        
        public static AddItemCampaignResponse Send(this IOrganizationService service, AddItemCampaignRequest request)
            => service.Execute<AddItemCampaignResponse>(request);
        
        public static AddListMembersListResponse Send(this IOrganizationService service, AddListMembersListRequest request)
            => service.Execute<AddListMembersListResponse>(request);
        
        public static AddMemberListResponse Send(this IOrganizationService service, AddMemberListRequest request)
            => service.Execute<AddMemberListResponse>(request);
        
        public static AddMembersTeamResponse Send(this IOrganizationService service, AddMembersTeamRequest request)
            => service.Execute<AddMembersTeamResponse>(request);
        
        public static AddPrincipalToQueueResponse Send(this IOrganizationService service, AddPrincipalToQueueRequest request)
            => service.Execute<AddPrincipalToQueueResponse>(request);
        
        public static AddPrivilegesRoleResponse Send(this IOrganizationService service, AddPrivilegesRoleRequest request)
            => service.Execute<AddPrivilegesRoleResponse>(request);
        
        public static AddProductToKitResponse Send(this IOrganizationService service, AddProductToKitRequest request)
            => service.Execute<AddProductToKitResponse>(request);
        
        public static AddRecurrenceResponse Send(this IOrganizationService service, AddRecurrenceRequest request)
            => service.Execute<AddRecurrenceResponse>(request);
        
        public static AddSolutionComponentResponse Send(this IOrganizationService service, AddSolutionComponentRequest request)
            => service.Execute<AddSolutionComponentResponse>(request);
        
        public static AddSubstituteProductResponse Send(this IOrganizationService service, AddSubstituteProductRequest request)
            => service.Execute<AddSubstituteProductResponse>(request);
        
        public static AddToQueueResponse Send(this IOrganizationService service, AddToQueueRequest request)
            => service.Execute<AddToQueueResponse>(request);
        
        public static AddUserToRecordTeamResponse Send(this IOrganizationService service, AddUserToRecordTeamRequest request)
            => service.Execute<AddUserToRecordTeamResponse>(request);
        
        public static ApplyRecordCreationAndUpdateRuleResponse Send(this IOrganizationService service, ApplyRecordCreationAndUpdateRuleRequest request)
            => service.Execute<ApplyRecordCreationAndUpdateRuleResponse>(request);
        
        public static ApplyRoutingRuleResponse Send(this IOrganizationService service, ApplyRoutingRuleRequest request)
            => service.Execute<ApplyRoutingRuleResponse>(request);
        
        public static AssignResponse Send(this IOrganizationService service, AssignRequest request)
            => service.Execute<AssignResponse>(request);
        
        public static AssociateEntitiesResponse Send(this IOrganizationService service, AssociateEntitiesRequest request)
            => service.Execute<AssociateEntitiesResponse>(request);
        
        public static AutoMapEntityResponse Send(this IOrganizationService service, AutoMapEntityRequest request)
            => service.Execute<AutoMapEntityResponse>(request);
        
        public static BackgroundSendEmailResponse Send(this IOrganizationService service, BackgroundSendEmailRequest request)
            => service.Execute<BackgroundSendEmailResponse>(request);
        
        public static BookResponse Send(this IOrganizationService service, BookRequest request)
            => service.Execute<BookResponse>(request);
        
        public static BulkDeleteResponse Send(this IOrganizationService service, BulkDeleteRequest request)
            => service.Execute<BulkDeleteResponse>(request);
        
        public static BulkDetectDuplicatesResponse Send(this IOrganizationService service, BulkDetectDuplicatesRequest request)
            => service.Execute<BulkDetectDuplicatesResponse>(request);
        
        public static CalculateActualValueOpportunityResponse Send(this IOrganizationService service, CalculateActualValueOpportunityRequest request)
            => service.Execute<CalculateActualValueOpportunityResponse>(request);
        
        public static CalculatePriceResponse Send(this IOrganizationService service, CalculatePriceRequest request)
            => service.Execute<CalculatePriceResponse>(request);
        
        public static CalculateRollupFieldResponse Send(this IOrganizationService service, CalculateRollupFieldRequest request)
            => service.Execute<CalculateRollupFieldResponse>(request);
        
        public static CalculateTotalTimeIncidentResponse Send(this IOrganizationService service, CalculateTotalTimeIncidentRequest request)
            => service.Execute<CalculateTotalTimeIncidentResponse>(request);
        
        public static CancelContractResponse Send(this IOrganizationService service, CancelContractRequest request)
            => service.Execute<CancelContractResponse>(request);
        
        public static CancelSalesOrderResponse Send(this IOrganizationService service, CancelSalesOrderRequest request)
            => service.Execute<CancelSalesOrderResponse>(request);
        
        public static CheckIncomingEmailResponse Send(this IOrganizationService service, CheckIncomingEmailRequest request)
            => service.Execute<CheckIncomingEmailResponse>(request);
        
        public static CheckPromoteEmailResponse Send(this IOrganizationService service, CheckPromoteEmailRequest request)
            => service.Execute<CheckPromoteEmailResponse>(request);
        
        public static CloneAsPatchResponse Send(this IOrganizationService service, CloneAsPatchRequest request)
            => service.Execute<CloneAsPatchResponse>(request);
        
        public static CloneAsSolutionResponse Send(this IOrganizationService service, CloneAsSolutionRequest request)
            => service.Execute<CloneAsSolutionResponse>(request);
        
        public static CloneContractResponse Send(this IOrganizationService service, CloneContractRequest request)
            => service.Execute<CloneContractResponse>(request);
        
        public static CloneMobileOfflineProfileResponse Send(this IOrganizationService service, CloneMobileOfflineProfileRequest request)
            => service.Execute<CloneMobileOfflineProfileResponse>(request);
        
        public static CloneProductResponse Send(this IOrganizationService service, CloneProductRequest request)
            => service.Execute<CloneProductResponse>(request);
        
        public static CloseIncidentResponse Send(this IOrganizationService service, CloseIncidentRequest request)
            => service.Execute<CloseIncidentResponse>(request);
        
        public static CloseQuoteResponse Send(this IOrganizationService service, CloseQuoteRequest request)
            => service.Execute<CloseQuoteResponse>(request);
        
        public static CompoundCreateResponse Send(this IOrganizationService service, CompoundCreateRequest request)
            => service.Execute<CompoundCreateResponse>(request);
        
        public static CompoundUpdateDuplicateDetectionRuleResponse Send(this IOrganizationService service, CompoundUpdateDuplicateDetectionRuleRequest request)
            => service.Execute<CompoundUpdateDuplicateDetectionRuleResponse>(request);
        
        public static CompoundUpdateResponse Send(this IOrganizationService service, CompoundUpdateRequest request)
            => service.Execute<CompoundUpdateResponse>(request);
        
        public static ConvertKitToProductResponse Send(this IOrganizationService service, ConvertKitToProductRequest request)
            => service.Execute<ConvertKitToProductResponse>(request);
        
        public static ConvertOwnerTeamToAccessTeamResponse Send(this IOrganizationService service, ConvertOwnerTeamToAccessTeamRequest request)
            => service.Execute<ConvertOwnerTeamToAccessTeamResponse>(request);
        
        public static ConvertProductToKitResponse Send(this IOrganizationService service, ConvertProductToKitRequest request)
            => service.Execute<ConvertProductToKitResponse>(request);
        
        public static ConvertQuoteToSalesOrderResponse Send(this IOrganizationService service, ConvertQuoteToSalesOrderRequest request)
            => service.Execute<ConvertQuoteToSalesOrderResponse>(request);
        
        public static ConvertSalesOrderToInvoiceResponse Send(this IOrganizationService service, ConvertSalesOrderToInvoiceRequest request)
            => service.Execute<ConvertSalesOrderToInvoiceResponse>(request);
        
        public static CopyCampaignResponse Send(this IOrganizationService service, CopyCampaignRequest request)
            => service.Execute<CopyCampaignResponse>(request);
        
        public static CopyCampaignResponseResponse Send(this IOrganizationService service, CopyCampaignResponseRequest request)
            => service.Execute<CopyCampaignResponseResponse>(request);
        
        public static CopyDynamicListToStaticResponse Send(this IOrganizationService service, CopyDynamicListToStaticRequest request)
            => service.Execute<CopyDynamicListToStaticResponse>(request);
        
        public static CopyMembersListResponse Send(this IOrganizationService service, CopyMembersListRequest request)
            => service.Execute<CopyMembersListResponse>(request);
        
        public static CopySystemFormResponse Send(this IOrganizationService service, CopySystemFormRequest request)
            => service.Execute<CopySystemFormResponse>(request);
        
        public static CreateActivitiesListResponse Send(this IOrganizationService service, CreateActivitiesListRequest request)
            => service.Execute<CreateActivitiesListResponse>(request);
        
        public static CreateExceptionResponse Send(this IOrganizationService service, CreateExceptionRequest request)
            => service.Execute<CreateExceptionResponse>(request);
        
        public static CreateInstanceResponse Send(this IOrganizationService service, CreateInstanceRequest request)
            => service.Execute<CreateInstanceResponse>(request);
        
        public static CreateKnowledgeArticleTranslationResponse Send(this IOrganizationService service, CreateKnowledgeArticleTranslationRequest request)
            => service.Execute<CreateKnowledgeArticleTranslationResponse>(request);
        
        public static CreateKnowledgeArticleVersionResponse Send(this IOrganizationService service, CreateKnowledgeArticleVersionRequest request)
            => service.Execute<CreateKnowledgeArticleVersionResponse>(request);
        
        public static CreateWorkflowFromTemplateResponse Send(this IOrganizationService service, CreateWorkflowFromTemplateRequest request)
            => service.Execute<CreateWorkflowFromTemplateResponse>(request);
        
        public static DeleteAndPromoteResponse Send(this IOrganizationService service, DeleteAndPromoteRequest request)
            => service.Execute<DeleteAndPromoteResponse>(request);
        
        public static DeleteAuditDataResponse Send(this IOrganizationService service, DeleteAuditDataRequest request)
            => service.Execute<DeleteAuditDataResponse>(request);
        
        public static DeleteOpenInstancesResponse Send(this IOrganizationService service, DeleteOpenInstancesRequest request)
            => service.Execute<DeleteOpenInstancesResponse>(request);
        
        public static DeliverIncomingEmailResponse Send(this IOrganizationService service, DeliverIncomingEmailRequest request)
            => service.Execute<DeliverIncomingEmailResponse>(request);
        
        public static DeliverPromoteEmailResponse Send(this IOrganizationService service, DeliverPromoteEmailRequest request)
            => service.Execute<DeliverPromoteEmailResponse>(request);
        
        public static DeprovisionLanguageResponse Send(this IOrganizationService service, DeprovisionLanguageRequest request)
            => service.Execute<DeprovisionLanguageResponse>(request);
        
        public static DisassociateEntitiesResponse Send(this IOrganizationService service, DisassociateEntitiesRequest request)
            => service.Execute<DisassociateEntitiesResponse>(request);
        
        public static DistributeCampaignActivityResponse Send(this IOrganizationService service, DistributeCampaignActivityRequest request)
            => service.Execute<DistributeCampaignActivityResponse>(request);
        
        public static DownloadReportDefinitionResponse Send(this IOrganizationService service, DownloadReportDefinitionRequest request)
            => service.Execute<DownloadReportDefinitionResponse>(request);
        
        public static ExecuteByIdSavedQueryResponse Send(this IOrganizationService service, ExecuteByIdSavedQueryRequest request)
            => service.Execute<ExecuteByIdSavedQueryResponse>(request);
        
        public static ExecuteByIdUserQueryResponse Send(this IOrganizationService service, ExecuteByIdUserQueryRequest request)
            => service.Execute<ExecuteByIdUserQueryResponse>(request);
        
        public static ExecuteFetchResponse Send(this IOrganizationService service, ExecuteFetchRequest request)
            => service.Execute<ExecuteFetchResponse>(request);
        
        public static ExecuteWorkflowResponse Send(this IOrganizationService service, ExecuteWorkflowRequest request)
            => service.Execute<ExecuteWorkflowResponse>(request);
        
        public static ExpandCalendarResponse Send(this IOrganizationService service, ExpandCalendarRequest request)
            => service.Execute<ExpandCalendarResponse>(request);
        
        public static ExportFieldTranslationResponse Send(this IOrganizationService service, ExportFieldTranslationRequest request)
            => service.Execute<ExportFieldTranslationResponse>(request);
        
        public static ExportMappingsImportMapResponse Send(this IOrganizationService service, ExportMappingsImportMapRequest request)
            => service.Execute<ExportMappingsImportMapResponse>(request);
        
        public static ExportSolutionResponse Send(this IOrganizationService service, ExportSolutionRequest request)
            => service.Execute<ExportSolutionResponse>(request);
        
        public static ExportTranslationResponse Send(this IOrganizationService service, ExportTranslationRequest request)
            => service.Execute<ExportTranslationResponse>(request);
        
        public static FetchXmlToQueryExpressionResponse Send(this IOrganizationService service, FetchXmlToQueryExpressionRequest request)
            => service.Execute<FetchXmlToQueryExpressionResponse>(request);
        
        public static FindParentResourceGroupResponse Send(this IOrganizationService service, FindParentResourceGroupRequest request)
            => service.Execute<FindParentResourceGroupResponse>(request);
        
        public static FormatAddressResponse Send(this IOrganizationService service, FormatAddressRequest request)
            => service.Execute<FormatAddressResponse>(request);
        
        public static FulfillSalesOrderResponse Send(this IOrganizationService service, FulfillSalesOrderRequest request)
            => service.Execute<FulfillSalesOrderResponse>(request);
        
        public static FullTextSearchKnowledgeArticleResponse Send(this IOrganizationService service, FullTextSearchKnowledgeArticleRequest request)
            => service.Execute<FullTextSearchKnowledgeArticleResponse>(request);
        
        public static GenerateInvoiceFromOpportunityResponse Send(this IOrganizationService service, GenerateInvoiceFromOpportunityRequest request)
            => service.Execute<GenerateInvoiceFromOpportunityResponse>(request);
        
        public static GenerateQuoteFromOpportunityResponse Send(this IOrganizationService service, GenerateQuoteFromOpportunityRequest request)
            => service.Execute<GenerateQuoteFromOpportunityResponse>(request);
        
        public static GenerateSalesOrderFromOpportunityResponse Send(this IOrganizationService service, GenerateSalesOrderFromOpportunityRequest request)
            => service.Execute<GenerateSalesOrderFromOpportunityResponse>(request);
        
        public static GenerateSocialProfileResponse Send(this IOrganizationService service, GenerateSocialProfileRequest request)
            => service.Execute<GenerateSocialProfileResponse>(request);
        
        public static GetAllTimeZonesWithDisplayNameResponse Send(this IOrganizationService service, GetAllTimeZonesWithDisplayNameRequest request)
            => service.Execute<GetAllTimeZonesWithDisplayNameResponse>(request);
        
        public static GetDecryptionKeyResponse Send(this IOrganizationService service, GetDecryptionKeyRequest request)
            => service.Execute<GetDecryptionKeyResponse>(request);
        
        public static GetDefaultPriceLevelResponse Send(this IOrganizationService service, GetDefaultPriceLevelRequest request)
            => service.Execute<GetDefaultPriceLevelResponse>(request);
        
        public static GetDistinctValuesImportFileResponse Send(this IOrganizationService service, GetDistinctValuesImportFileRequest request)
            => service.Execute<GetDistinctValuesImportFileResponse>(request);
        
        public static GetHeaderColumnsImportFileResponse Send(this IOrganizationService service, GetHeaderColumnsImportFileRequest request)
            => service.Execute<GetHeaderColumnsImportFileResponse>(request);
        
        public static GetInvoiceProductsFromOpportunityResponse Send(this IOrganizationService service, GetInvoiceProductsFromOpportunityRequest request)
            => service.Execute<GetInvoiceProductsFromOpportunityResponse>(request);
        
        public static GetQuantityDecimalResponse Send(this IOrganizationService service, GetQuantityDecimalRequest request)
            => service.Execute<GetQuantityDecimalResponse>(request);
        
        public static GetQuoteProductsFromOpportunityResponse Send(this IOrganizationService service, GetQuoteProductsFromOpportunityRequest request)
            => service.Execute<GetQuoteProductsFromOpportunityResponse>(request);
        
        public static GetReportHistoryLimitResponse Send(this IOrganizationService service, GetReportHistoryLimitRequest request)
            => service.Execute<GetReportHistoryLimitResponse>(request);
        
        public static GetSalesOrderProductsFromOpportunityResponse Send(this IOrganizationService service, GetSalesOrderProductsFromOpportunityRequest request)
            => service.Execute<GetSalesOrderProductsFromOpportunityResponse>(request);
        
        public static GetTimeZoneCodeByLocalizedNameResponse Send(this IOrganizationService service, GetTimeZoneCodeByLocalizedNameRequest request)
            => service.Execute<GetTimeZoneCodeByLocalizedNameResponse>(request);
        
        public static GetTrackingTokenEmailResponse Send(this IOrganizationService service, GetTrackingTokenEmailRequest request)
            => service.Execute<GetTrackingTokenEmailResponse>(request);
        
        public static GrantAccessResponse Send(this IOrganizationService service, GrantAccessRequest request)
            => service.Execute<GrantAccessResponse>(request);
        
        public static ImportFieldTranslationResponse Send(this IOrganizationService service, ImportFieldTranslationRequest request)
            => service.Execute<ImportFieldTranslationResponse>(request);
        
        public static ImportMappingsImportMapResponse Send(this IOrganizationService service, ImportMappingsImportMapRequest request)
            => service.Execute<ImportMappingsImportMapResponse>(request);
        
        public static ImportRecordsImportResponse Send(this IOrganizationService service, ImportRecordsImportRequest request)
            => service.Execute<ImportRecordsImportResponse>(request);
        
        public static ImportSolutionResponse Send(this IOrganizationService service, ImportSolutionRequest request)
            => service.Execute<ImportSolutionResponse>(request);
        
        public static ImportTranslationResponse Send(this IOrganizationService service, ImportTranslationRequest request)
            => service.Execute<ImportTranslationResponse>(request);
        
        public static IncrementKnowledgeArticleViewCountResponse Send(this IOrganizationService service, IncrementKnowledgeArticleViewCountRequest request)
            => service.Execute<IncrementKnowledgeArticleViewCountResponse>(request);
        
        public static InitializeFromResponse Send(this IOrganizationService service, InitializeFromRequest request)
            => service.Execute<InitializeFromResponse>(request);
        
        public static InstallSampleDataResponse Send(this IOrganizationService service, InstallSampleDataRequest request)
            => service.Execute<InstallSampleDataResponse>(request);
        
        public static InstantiateFiltersResponse Send(this IOrganizationService service, InstantiateFiltersRequest request)
            => service.Execute<InstantiateFiltersResponse>(request);
        
        public static InstantiateTemplateResponse Send(this IOrganizationService service, InstantiateTemplateRequest request)
            => service.Execute<InstantiateTemplateResponse>(request);
        
        public static IsBackOfficeInstalledResponse Send(this IOrganizationService service, IsBackOfficeInstalledRequest request)
            => service.Execute<IsBackOfficeInstalledResponse>(request);
        
        public static IsComponentCustomizableResponse Send(this IOrganizationService service, IsComponentCustomizableRequest request)
            => service.Execute<IsComponentCustomizableResponse>(request);
        
        public static IsValidStateTransitionResponse Send(this IOrganizationService service, IsValidStateTransitionRequest request)
            => service.Execute<IsValidStateTransitionResponse>(request);
        
        public static LocalTimeFromUtcTimeResponse Send(this IOrganizationService service, LocalTimeFromUtcTimeRequest request)
            => service.Execute<LocalTimeFromUtcTimeResponse>(request);
        
        public static LockInvoicePricingResponse Send(this IOrganizationService service, LockInvoicePricingRequest request)
            => service.Execute<LockInvoicePricingResponse>(request);
        
        public static LockSalesOrderPricingResponse Send(this IOrganizationService service, LockSalesOrderPricingRequest request)
            => service.Execute<LockSalesOrderPricingResponse>(request);
        
        public static LoseOpportunityResponse Send(this IOrganizationService service, LoseOpportunityRequest request)
            => service.Execute<LoseOpportunityResponse>(request);
        
        public static MakeAvailableToOrganizationReportResponse Send(this IOrganizationService service, MakeAvailableToOrganizationReportRequest request)
            => service.Execute<MakeAvailableToOrganizationReportResponse>(request);
        
        public static MakeAvailableToOrganizationTemplateResponse Send(this IOrganizationService service, MakeAvailableToOrganizationTemplateRequest request)
            => service.Execute<MakeAvailableToOrganizationTemplateResponse>(request);
        
        public static MakeUnavailableToOrganizationReportResponse Send(this IOrganizationService service, MakeUnavailableToOrganizationReportRequest request)
            => service.Execute<MakeUnavailableToOrganizationReportResponse>(request);
        
        public static MakeUnavailableToOrganizationTemplateResponse Send(this IOrganizationService service, MakeUnavailableToOrganizationTemplateRequest request)
            => service.Execute<MakeUnavailableToOrganizationTemplateResponse>(request);
        
        public static MergeResponse Send(this IOrganizationService service, MergeRequest request)
            => service.Execute<MergeResponse>(request);
        
        public static ModifyAccessResponse Send(this IOrganizationService service, ModifyAccessRequest request)
            => service.Execute<ModifyAccessResponse>(request);
        
        public static ParseImportResponse Send(this IOrganizationService service, ParseImportRequest request)
            => service.Execute<ParseImportResponse>(request);
        
        public static PickFromQueueResponse Send(this IOrganizationService service, PickFromQueueRequest request)
            => service.Execute<PickFromQueueResponse>(request);
        
        public static ProcessInboundEmailResponse Send(this IOrganizationService service, ProcessInboundEmailRequest request)
            => service.Execute<ProcessInboundEmailResponse>(request);
        
        public static PropagateByExpressionResponse Send(this IOrganizationService service, PropagateByExpressionRequest request)
            => service.Execute<PropagateByExpressionResponse>(request);
        
        public static ProvisionLanguageResponse Send(this IOrganizationService service, ProvisionLanguageRequest request)
            => service.Execute<ProvisionLanguageResponse>(request);
        
        public static PublishAllXmlResponse Send(this IOrganizationService service, PublishAllXmlRequest request)
            => service.Execute<PublishAllXmlResponse>(request);
        
        public static PublishDuplicateRuleResponse Send(this IOrganizationService service, PublishDuplicateRuleRequest request)
            => service.Execute<PublishDuplicateRuleResponse>(request);
        
        public static PublishProductHierarchyResponse Send(this IOrganizationService service, PublishProductHierarchyRequest request)
            => service.Execute<PublishProductHierarchyResponse>(request);
        
        public static PublishThemeResponse Send(this IOrganizationService service, PublishThemeRequest request)
            => service.Execute<PublishThemeResponse>(request);
        
        public static PublishXmlResponse Send(this IOrganizationService service, PublishXmlRequest request)
            => service.Execute<PublishXmlResponse>(request);
        
        public static QualifyLeadResponse Send(this IOrganizationService service, QualifyLeadRequest request)
            => service.Execute<QualifyLeadResponse>(request);
        
        public static QualifyMemberListResponse Send(this IOrganizationService service, QualifyMemberListRequest request)
            => service.Execute<QualifyMemberListResponse>(request);
        
        public static QueryExpressionToFetchXmlResponse Send(this IOrganizationService service, QueryExpressionToFetchXmlRequest request)
            => service.Execute<QueryExpressionToFetchXmlResponse>(request);
        
        public static QueryMultipleSchedulesResponse Send(this IOrganizationService service, QueryMultipleSchedulesRequest request)
            => service.Execute<QueryMultipleSchedulesResponse>(request);
        
        public static QueryScheduleResponse Send(this IOrganizationService service, QueryScheduleRequest request)
            => service.Execute<QueryScheduleResponse>(request);
        
        public static ReassignObjectsOwnerResponse Send(this IOrganizationService service, ReassignObjectsOwnerRequest request)
            => service.Execute<ReassignObjectsOwnerResponse>(request);
        
        public static ReassignObjectsSystemUserResponse Send(this IOrganizationService service, ReassignObjectsSystemUserRequest request)
            => service.Execute<ReassignObjectsSystemUserResponse>(request);
        
        public static RecalculateResponse Send(this IOrganizationService service, RecalculateRequest request)
            => service.Execute<RecalculateResponse>(request);
        
        public static ReleaseToQueueResponse Send(this IOrganizationService service, ReleaseToQueueRequest request)
            => service.Execute<ReleaseToQueueResponse>(request);
        
        public static RemoveAppComponentsResponse Send(this IOrganizationService service, RemoveAppComponentsRequest request)
            => service.Execute<RemoveAppComponentsResponse>(request);
        
        public static RemoveFromQueueResponse Send(this IOrganizationService service, RemoveFromQueueRequest request)
            => service.Execute<RemoveFromQueueResponse>(request);
        
        public static RemoveItemCampaignActivityResponse Send(this IOrganizationService service, RemoveItemCampaignActivityRequest request)
            => service.Execute<RemoveItemCampaignActivityResponse>(request);
        
        public static RemoveItemCampaignResponse Send(this IOrganizationService service, RemoveItemCampaignRequest request)
            => service.Execute<RemoveItemCampaignResponse>(request);
        
        public static RemoveMemberListResponse Send(this IOrganizationService service, RemoveMemberListRequest request)
            => service.Execute<RemoveMemberListResponse>(request);
        
        public static RemoveMembersTeamResponse Send(this IOrganizationService service, RemoveMembersTeamRequest request)
            => service.Execute<RemoveMembersTeamResponse>(request);
        
        public static RemoveParentResponse Send(this IOrganizationService service, RemoveParentRequest request)
            => service.Execute<RemoveParentResponse>(request);
        
        public static RemovePrivilegeRoleResponse Send(this IOrganizationService service, RemovePrivilegeRoleRequest request)
            => service.Execute<RemovePrivilegeRoleResponse>(request);
        
        public static RemoveProductFromKitResponse Send(this IOrganizationService service, RemoveProductFromKitRequest request)
            => service.Execute<RemoveProductFromKitResponse>(request);
        
        public static RemoveRelatedResponse Send(this IOrganizationService service, RemoveRelatedRequest request)
            => service.Execute<RemoveRelatedResponse>(request);
        
        public static RemoveSolutionComponentResponse Send(this IOrganizationService service, RemoveSolutionComponentRequest request)
            => service.Execute<RemoveSolutionComponentResponse>(request);
        
        public static RemoveSubstituteProductResponse Send(this IOrganizationService service, RemoveSubstituteProductRequest request)
            => service.Execute<RemoveSubstituteProductResponse>(request);
        
        public static RemoveUserFromRecordTeamResponse Send(this IOrganizationService service, RemoveUserFromRecordTeamRequest request)
            => service.Execute<RemoveUserFromRecordTeamResponse>(request);
        
        public static RenewContractResponse Send(this IOrganizationService service, RenewContractRequest request)
            => service.Execute<RenewContractResponse>(request);
        
        public static RenewEntitlementResponse Send(this IOrganizationService service, RenewEntitlementRequest request)
            => service.Execute<RenewEntitlementResponse>(request);
        
        public static ReplacePrivilegesRoleResponse Send(this IOrganizationService service, ReplacePrivilegesRoleRequest request)
            => service.Execute<ReplacePrivilegesRoleResponse>(request);
        
        public static RescheduleResponse Send(this IOrganizationService service, RescheduleRequest request)
            => service.Execute<RescheduleResponse>(request);
        
        public static ResetUserFiltersResponse Send(this IOrganizationService service, ResetUserFiltersRequest request)
            => service.Execute<ResetUserFiltersResponse>(request);
        
        public static RetrieveAbsoluteAndSiteCollectionUrlResponse Send(this IOrganizationService service, RetrieveAbsoluteAndSiteCollectionUrlRequest request)
            => service.Execute<RetrieveAbsoluteAndSiteCollectionUrlResponse>(request);
        
        public static RetrieveActivePathResponse Send(this IOrganizationService service, RetrieveActivePathRequest request)
            => service.Execute<RetrieveActivePathResponse>(request);
        
        public static RetrieveAllChildUsersSystemUserResponse Send(this IOrganizationService service, RetrieveAllChildUsersSystemUserRequest request)
            => service.Execute<RetrieveAllChildUsersSystemUserResponse>(request);
        
        public static RetrieveAppComponentsResponse Send(this IOrganizationService service, RetrieveAppComponentsRequest request)
            => service.Execute<RetrieveAppComponentsResponse>(request);
        
        public static RetrieveApplicationRibbonResponse Send(this IOrganizationService service, RetrieveApplicationRibbonRequest request)
            => service.Execute<RetrieveApplicationRibbonResponse>(request);
        
        public static RetrieveAttributeChangeHistoryResponse Send(this IOrganizationService service, RetrieveAttributeChangeHistoryRequest request)
            => service.Execute<RetrieveAttributeChangeHistoryResponse>(request);
        
        public static RetrieveAuditDetailsResponse Send(this IOrganizationService service, RetrieveAuditDetailsRequest request)
            => service.Execute<RetrieveAuditDetailsResponse>(request);
        
        public static RetrieveAuditPartitionListResponse Send(this IOrganizationService service, RetrieveAuditPartitionListRequest request)
            => service.Execute<RetrieveAuditPartitionListResponse>(request);
        
        public static RetrieveAvailableLanguagesResponse Send(this IOrganizationService service, RetrieveAvailableLanguagesRequest request)
            => service.Execute<RetrieveAvailableLanguagesResponse>(request);
        
        public static RetrieveBusinessHierarchyBusinessUnitResponse Send(this IOrganizationService service, RetrieveBusinessHierarchyBusinessUnitRequest request)
            => service.Execute<RetrieveBusinessHierarchyBusinessUnitResponse>(request);
        
        public static RetrieveByGroupResourceResponse Send(this IOrganizationService service, RetrieveByGroupResourceRequest request)
            => service.Execute<RetrieveByGroupResourceResponse>(request);
        
        public static RetrieveByResourceResourceGroupResponse Send(this IOrganizationService service, RetrieveByResourceResourceGroupRequest request)
            => service.Execute<RetrieveByResourceResourceGroupResponse>(request);
        
        public static RetrieveByResourcesServiceResponse Send(this IOrganizationService service, RetrieveByResourcesServiceRequest request)
            => service.Execute<RetrieveByResourcesServiceResponse>(request);
        
        public static RetrieveByTopIncidentProductKbArticleResponse Send(this IOrganizationService service, RetrieveByTopIncidentProductKbArticleRequest request)
            => service.Execute<RetrieveByTopIncidentProductKbArticleResponse>(request);
        
        public static RetrieveByTopIncidentSubjectKbArticleResponse Send(this IOrganizationService service, RetrieveByTopIncidentSubjectKbArticleRequest request)
            => service.Execute<RetrieveByTopIncidentSubjectKbArticleResponse>(request);
        
        public static RetrieveChannelAccessProfilePrivilegesResponse Send(this IOrganizationService service, RetrieveChannelAccessProfilePrivilegesRequest request)
            => service.Execute<RetrieveChannelAccessProfilePrivilegesResponse>(request);
        
        public static RetrieveCurrentOrganizationResponse Send(this IOrganizationService service, RetrieveCurrentOrganizationRequest request)
            => service.Execute<RetrieveCurrentOrganizationResponse>(request);
        
        public static RetrieveDependenciesForDeleteResponse Send(this IOrganizationService service, RetrieveDependenciesForDeleteRequest request)
            => service.Execute<RetrieveDependenciesForDeleteResponse>(request);
        
        public static RetrieveDependenciesForUninstallResponse Send(this IOrganizationService service, RetrieveDependenciesForUninstallRequest request)
            => service.Execute<RetrieveDependenciesForUninstallResponse>(request);
        
        public static RetrieveDependentComponentsResponse Send(this IOrganizationService service, RetrieveDependentComponentsRequest request)
            => service.Execute<RetrieveDependentComponentsResponse>(request);
        
        public static RetrieveDeploymentLicenseTypeResponse Send(this IOrganizationService service, RetrieveDeploymentLicenseTypeRequest request)
            => service.Execute<RetrieveDeploymentLicenseTypeResponse>(request);
        
        public static RetrieveDeprovisionedLanguagesResponse Send(this IOrganizationService service, RetrieveDeprovisionedLanguagesRequest request)
            => service.Execute<RetrieveDeprovisionedLanguagesResponse>(request);
        
        public static RetrieveDuplicatesResponse Send(this IOrganizationService service, RetrieveDuplicatesRequest request)
            => service.Execute<RetrieveDuplicatesResponse>(request);
        
        public static RetrieveEntityRibbonResponse Send(this IOrganizationService service, RetrieveEntityRibbonRequest request)
            => service.Execute<RetrieveEntityRibbonResponse>(request);
        
        public static RetrieveExchangeAppointmentsResponse Send(this IOrganizationService service, RetrieveExchangeAppointmentsRequest request)
            => service.Execute<RetrieveExchangeAppointmentsResponse>(request);
        
        public static RetrieveExchangeRateResponse Send(this IOrganizationService service, RetrieveExchangeRateRequest request)
            => service.Execute<RetrieveExchangeRateResponse>(request);
        
        public static RetrieveFilteredFormsResponse Send(this IOrganizationService service, RetrieveFilteredFormsRequest request)
            => service.Execute<RetrieveFilteredFormsResponse>(request);
        
        public static RetrieveFormattedImportJobResultsResponse Send(this IOrganizationService service, RetrieveFormattedImportJobResultsRequest request)
            => service.Execute<RetrieveFormattedImportJobResultsResponse>(request);
        
        public static RetrieveFormXmlResponse Send(this IOrganizationService service, RetrieveFormXmlRequest request)
            => service.Execute<RetrieveFormXmlResponse>(request);
        
        public static RetrieveInstalledLanguagePacksResponse Send(this IOrganizationService service, RetrieveInstalledLanguagePacksRequest request)
            => service.Execute<RetrieveInstalledLanguagePacksResponse>(request);
        
        public static RetrieveInstalledLanguagePackVersionResponse Send(this IOrganizationService service, RetrieveInstalledLanguagePackVersionRequest request)
            => service.Execute<RetrieveInstalledLanguagePackVersionResponse>(request);
        
        public static RetrieveLicenseInfoResponse Send(this IOrganizationService service, RetrieveLicenseInfoRequest request)
            => service.Execute<RetrieveLicenseInfoResponse>(request);
        
        public static RetrieveLocLabelsResponse Send(this IOrganizationService service, RetrieveLocLabelsRequest request)
            => service.Execute<RetrieveLocLabelsResponse>(request);
        
        public static RetrieveMailboxTrackingFoldersResponse Send(this IOrganizationService service, RetrieveMailboxTrackingFoldersRequest request)
            => service.Execute<RetrieveMailboxTrackingFoldersResponse>(request);
        
        public static RetrieveMembersBulkOperationResponse Send(this IOrganizationService service, RetrieveMembersBulkOperationRequest request)
            => service.Execute<RetrieveMembersBulkOperationResponse>(request);
        
        public static RetrieveMembersTeamResponse Send(this IOrganizationService service, RetrieveMembersTeamRequest request)
            => service.Execute<RetrieveMembersTeamResponse>(request);
        
        public static RetrieveMissingComponentsResponse Send(this IOrganizationService service, RetrieveMissingComponentsRequest request)
            => service.Execute<RetrieveMissingComponentsResponse>(request);
        
        public static RetrieveMissingDependenciesResponse Send(this IOrganizationService service, RetrieveMissingDependenciesRequest request)
            => service.Execute<RetrieveMissingDependenciesResponse>(request);
        
        public static RetrieveOrganizationResourcesResponse Send(this IOrganizationService service, RetrieveOrganizationResourcesRequest request)
            => service.Execute<RetrieveOrganizationResourcesResponse>(request);
        
        public static RetrieveParentGroupsResourceGroupResponse Send(this IOrganizationService service, RetrieveParentGroupsResourceGroupRequest request)
            => service.Execute<RetrieveParentGroupsResourceGroupResponse>(request);
        
        public static RetrieveParsedDataImportFileResponse Send(this IOrganizationService service, RetrieveParsedDataImportFileRequest request)
            => service.Execute<RetrieveParsedDataImportFileResponse>(request);
        
        public static RetrievePersonalWallResponse Send(this IOrganizationService service, RetrievePersonalWallRequest request)
            => service.Execute<RetrievePersonalWallResponse>(request);
        
        public static RetrievePrincipalAccessResponse Send(this IOrganizationService service, RetrievePrincipalAccessRequest request)
            => service.Execute<RetrievePrincipalAccessResponse>(request);
        
        public static RetrievePrincipalAttributePrivilegesResponse Send(this IOrganizationService service, RetrievePrincipalAttributePrivilegesRequest request)
            => service.Execute<RetrievePrincipalAttributePrivilegesResponse>(request);
        
        public static RetrievePrincipalSyncAttributeMappingsResponse Send(this IOrganizationService service, RetrievePrincipalSyncAttributeMappingsRequest request)
            => service.Execute<RetrievePrincipalSyncAttributeMappingsResponse>(request);
        
        public static RetrievePrivilegeSetResponse Send(this IOrganizationService service, RetrievePrivilegeSetRequest request)
            => service.Execute<RetrievePrivilegeSetResponse>(request);
        
        public static RetrieveProcessInstancesResponse Send(this IOrganizationService service, RetrieveProcessInstancesRequest request)
            => service.Execute<RetrieveProcessInstancesResponse>(request);
        
        public static RetrieveProductPropertiesResponse Send(this IOrganizationService service, RetrieveProductPropertiesRequest request)
            => service.Execute<RetrieveProductPropertiesResponse>(request);
        
        public static RetrieveProvisionedLanguagePackVersionResponse Send(this IOrganizationService service, RetrieveProvisionedLanguagePackVersionRequest request)
            => service.Execute<RetrieveProvisionedLanguagePackVersionResponse>(request);
        
        public static RetrieveProvisionedLanguagesResponse Send(this IOrganizationService service, RetrieveProvisionedLanguagesRequest request)
            => service.Execute<RetrieveProvisionedLanguagesResponse>(request);
        
        public static RetrieveRecordChangeHistoryResponse Send(this IOrganizationService service, RetrieveRecordChangeHistoryRequest request)
            => service.Execute<RetrieveRecordChangeHistoryResponse>(request);
        
        public static RetrieveRecordWallResponse Send(this IOrganizationService service, RetrieveRecordWallRequest request)
            => service.Execute<RetrieveRecordWallResponse>(request);
        
        public static RetrieveRequiredComponentsResponse Send(this IOrganizationService service, RetrieveRequiredComponentsRequest request)
            => service.Execute<RetrieveRequiredComponentsResponse>(request);
        
        public static RetrieveRolePrivilegesRoleResponse Send(this IOrganizationService service, RetrieveRolePrivilegesRoleRequest request)
            => service.Execute<RetrieveRolePrivilegesRoleResponse>(request);
        
        public static RetrieveSharedPrincipalsAndAccessResponse Send(this IOrganizationService service, RetrieveSharedPrincipalsAndAccessRequest request)
            => service.Execute<RetrieveSharedPrincipalsAndAccessResponse>(request);
        
        public static RetrieveSubGroupsResourceGroupResponse Send(this IOrganizationService service, RetrieveSubGroupsResourceGroupRequest request)
            => service.Execute<RetrieveSubGroupsResourceGroupResponse>(request);
        
        public static RetrieveSubsidiaryTeamsBusinessUnitResponse Send(this IOrganizationService service, RetrieveSubsidiaryTeamsBusinessUnitRequest request)
            => service.Execute<RetrieveSubsidiaryTeamsBusinessUnitResponse>(request);
        
        public static RetrieveSubsidiaryUsersBusinessUnitResponse Send(this IOrganizationService service, RetrieveSubsidiaryUsersBusinessUnitRequest request)
            => service.Execute<RetrieveSubsidiaryUsersBusinessUnitResponse>(request);
        
        public static RetrieveTeamPrivilegesResponse Send(this IOrganizationService service, RetrieveTeamPrivilegesRequest request)
            => service.Execute<RetrieveTeamPrivilegesResponse>(request);
        
        public static RetrieveTeamsSystemUserResponse Send(this IOrganizationService service, RetrieveTeamsSystemUserRequest request)
            => service.Execute<RetrieveTeamsSystemUserResponse>(request);
        
        public static RetrieveTotalRecordCountResponse Send(this IOrganizationService service, RetrieveTotalRecordCountRequest request)
            => service.Execute<RetrieveTotalRecordCountResponse>(request);
        
        public static RetrieveUnpublishedMultipleResponse Send(this IOrganizationService service, RetrieveUnpublishedMultipleRequest request)
            => service.Execute<RetrieveUnpublishedMultipleResponse>(request);
        
        public static RetrieveUnpublishedResponse Send(this IOrganizationService service, RetrieveUnpublishedRequest request)
            => service.Execute<RetrieveUnpublishedResponse>(request);
        
        public static RetrieveUserPrivilegesResponse Send(this IOrganizationService service, RetrieveUserPrivilegesRequest request)
            => service.Execute<RetrieveUserPrivilegesResponse>(request);
        
        public static RetrieveUserQueuesResponse Send(this IOrganizationService service, RetrieveUserQueuesRequest request)
            => service.Execute<RetrieveUserQueuesResponse>(request);
        
        public static RetrieveUserSettingsSystemUserResponse Send(this IOrganizationService service, RetrieveUserSettingsSystemUserRequest request)
            => service.Execute<RetrieveUserSettingsSystemUserResponse>(request);
        
        public static RetrieveVersionResponse Send(this IOrganizationService service, RetrieveVersionRequest request)
            => service.Execute<RetrieveVersionResponse>(request);
        
        public static RevertProductResponse Send(this IOrganizationService service, RevertProductRequest request)
            => service.Execute<RevertProductResponse>(request);
        
        public static ReviseQuoteResponse Send(this IOrganizationService service, ReviseQuoteRequest request)
            => service.Execute<ReviseQuoteResponse>(request);
        
        public static RevokeAccessResponse Send(this IOrganizationService service, RevokeAccessRequest request)
            => service.Execute<RevokeAccessResponse>(request);
        
        public static RollupResponse Send(this IOrganizationService service, RollupRequest request)
            => service.Execute<RollupResponse>(request);
        
        public static RouteToResponse Send(this IOrganizationService service, RouteToRequest request)
            => service.Execute<RouteToResponse>(request);
        
        public static SearchByBodyKbArticleResponse Send(this IOrganizationService service, SearchByBodyKbArticleRequest request)
            => service.Execute<SearchByBodyKbArticleResponse>(request);
        
        public static SearchByKeywordsKbArticleResponse Send(this IOrganizationService service, SearchByKeywordsKbArticleRequest request)
            => service.Execute<SearchByKeywordsKbArticleResponse>(request);
        
        public static SearchByTitleKbArticleResponse Send(this IOrganizationService service, SearchByTitleKbArticleRequest request)
            => service.Execute<SearchByTitleKbArticleResponse>(request);
        
        public static SearchResponse Send(this IOrganizationService service, SearchRequest request)
            => service.Execute<SearchResponse>(request);
        
        public static SendBulkMailResponse Send(this IOrganizationService service, SendBulkMailRequest request)
            => service.Execute<SendBulkMailResponse>(request);
        
        public static SendEmailFromTemplateResponse Send(this IOrganizationService service, SendEmailFromTemplateRequest request)
            => service.Execute<SendEmailFromTemplateResponse>(request);
        
        public static SendEmailResponse Send(this IOrganizationService service, SendEmailRequest request)
            => service.Execute<SendEmailResponse>(request);
        
        public static SendFaxResponse Send(this IOrganizationService service, SendFaxRequest request)
            => service.Execute<SendFaxResponse>(request);
        
        public static SendTemplateResponse Send(this IOrganizationService service, SendTemplateRequest request)
            => service.Execute<SendTemplateResponse>(request);
        
        public static SetAutoNumberSeed1Response Send(this IOrganizationService service, SetAutoNumberSeed1Request request)
            => service.Execute<SetAutoNumberSeed1Response>(request);
        
        public static SetAutoNumberSeedResponse Send(this IOrganizationService service, SetAutoNumberSeedRequest request)
            => service.Execute<SetAutoNumberSeedResponse>(request);
        
        public static SetBusinessEquipmentResponse Send(this IOrganizationService service, SetBusinessEquipmentRequest request)
            => service.Execute<SetBusinessEquipmentResponse>(request);
        
        public static SetBusinessSystemUserResponse Send(this IOrganizationService service, SetBusinessSystemUserRequest request)
            => service.Execute<SetBusinessSystemUserResponse>(request);
        
        public static SetFeatureStatusResponse Send(this IOrganizationService service, SetFeatureStatusRequest request)
            => service.Execute<SetFeatureStatusResponse>(request);
        
        public static SetLocLabelsResponse Send(this IOrganizationService service, SetLocLabelsRequest request)
            => service.Execute<SetLocLabelsResponse>(request);
        
        public static SetParentBusinessUnitResponse Send(this IOrganizationService service, SetParentBusinessUnitRequest request)
            => service.Execute<SetParentBusinessUnitResponse>(request);
        
        public static SetParentSystemUserResponse Send(this IOrganizationService service, SetParentSystemUserRequest request)
            => service.Execute<SetParentSystemUserResponse>(request);
        
        public static SetParentTeamResponse Send(this IOrganizationService service, SetParentTeamRequest request)
            => service.Execute<SetParentTeamResponse>(request);
        
        public static SetProcessResponse Send(this IOrganizationService service, SetProcessRequest request)
            => service.Execute<SetProcessResponse>(request);
        
        public static SetRelatedResponse Send(this IOrganizationService service, SetRelatedRequest request)
            => service.Execute<SetRelatedResponse>(request);
        
        public static SetReportRelatedResponse Send(this IOrganizationService service, SetReportRelatedRequest request)
            => service.Execute<SetReportRelatedResponse>(request);
        
        public static SetStateResponse Send(this IOrganizationService service, SetStateRequest request)
            => service.Execute<SetStateResponse>(request);
        
        public static TransformImportResponse Send(this IOrganizationService service, TransformImportRequest request)
            => service.Execute<TransformImportResponse>(request);
        
        public static TriggerServiceEndpointCheckResponse Send(this IOrganizationService service, TriggerServiceEndpointCheckRequest request)
            => service.Execute<TriggerServiceEndpointCheckResponse>(request);
        
        public static UninstallSampleDataResponse Send(this IOrganizationService service, UninstallSampleDataRequest request)
            => service.Execute<UninstallSampleDataResponse>(request);
        
        public static UnlockInvoicePricingResponse Send(this IOrganizationService service, UnlockInvoicePricingRequest request)
            => service.Execute<UnlockInvoicePricingResponse>(request);
        
        public static UnlockSalesOrderPricingResponse Send(this IOrganizationService service, UnlockSalesOrderPricingRequest request)
            => service.Execute<UnlockSalesOrderPricingResponse>(request);
        
        public static UnpublishDuplicateRuleResponse Send(this IOrganizationService service, UnpublishDuplicateRuleRequest request)
            => service.Execute<UnpublishDuplicateRuleResponse>(request);
        
        public static UpdateFeatureConfigResponse Send(this IOrganizationService service, UpdateFeatureConfigRequest request)
            => service.Execute<UpdateFeatureConfigResponse>(request);
        
        public static UpdateProductPropertiesResponse Send(this IOrganizationService service, UpdateProductPropertiesRequest request)
            => service.Execute<UpdateProductPropertiesResponse>(request);
        
        public static UpdateSolutionComponentResponse Send(this IOrganizationService service, UpdateSolutionComponentRequest request)
            => service.Execute<UpdateSolutionComponentResponse>(request);
        
        public static UpdateUserSettingsSystemUserResponse Send(this IOrganizationService service, UpdateUserSettingsSystemUserRequest request)
            => service.Execute<UpdateUserSettingsSystemUserResponse>(request);
        
        public static UtcTimeFromLocalTimeResponse Send(this IOrganizationService service, UtcTimeFromLocalTimeRequest request)
            => service.Execute<UtcTimeFromLocalTimeResponse>(request);
        
        public static ValidateAppResponse Send(this IOrganizationService service, ValidateAppRequest request)
            => service.Execute<ValidateAppResponse>(request);
        
        public static ValidateRecurrenceRuleResponse Send(this IOrganizationService service, ValidateRecurrenceRuleRequest request)
            => service.Execute<ValidateRecurrenceRuleResponse>(request);
        
        public static ValidateResponse Send(this IOrganizationService service, ValidateRequest request)
            => service.Execute<ValidateResponse>(request);
        
        public static ValidateSavedQueryResponse Send(this IOrganizationService service, ValidateSavedQueryRequest request)
            => service.Execute<ValidateSavedQueryResponse>(request);
        
        public static WhoAmIResponse Send(this IOrganizationService service, WhoAmIRequest request)
            => service.Execute<WhoAmIResponse>(request);
        
        public static WinOpportunityResponse Send(this IOrganizationService service, WinOpportunityRequest request)
            => service.Execute<WinOpportunityResponse>(request);
        
        public static WinQuoteResponse Send(this IOrganizationService service, WinQuoteRequest request)
            => service.Execute<WinQuoteResponse>(request);
    }
}
