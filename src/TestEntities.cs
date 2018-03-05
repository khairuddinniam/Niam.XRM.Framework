using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using EntityName = Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute;

namespace Niam.XRM.Framework.Tests
{
    public abstract class EntityBase : Entity
    {
        protected EntityBase(string entityName) : base(entityName)
        {
        }

        public readonly DateTime? CreatedOn = null;
        public readonly DateTime? ModifiedOn = null;
        public readonly int? VersionNumber = null;

        public readonly EntityReference CreatedBy = null;
        public readonly EntityReference CreatedOnBehalfBy = null;
        public readonly int? ImportSequenceNumber = null;
        public readonly EntityReference ModifiedBy = null;
        public readonly EntityReference ModifiedOnBehalfBy = null;
        public readonly DateTime? OverriddenCreatedOn = null;
        public readonly OptionSetValue statecode = null;
        public readonly OptionSetValue statuscode = null;
        public readonly int? TimeZoneRuleVersionNumber = null;
        public readonly int? UTCConversionTimeZoneCode = null;
    }

    public abstract class UserOrTeamEntity : EntityBase
    {
        protected UserOrTeamEntity(string entityName) : base(entityName)
        {
        }

        public readonly EntityReference OwningBusinessUnit = null;
        public readonly EntityReference OwningTeam = null;
        public readonly EntityReference OwningUser = null;
        public readonly EntityReference OwnerId = null;
    }

    public abstract class OrganizationEntity : EntityBase
    {
        protected OrganizationEntity(string entityName) : base(entityName)
        {
        }
    }

    [EntityName("usersettings")]
    public class UserSettings : Entity
    {
        public const string EntityLogicalName = "usersettings";
        public UserSettings() : base(EntityLogicalName)
        {
        }

        [Column("systemuserid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["systemuserid"] = value;
            }
        }

        public readonly int? OutlookSyncInterval = null;

        public readonly int? OfflineSyncInterval = null;

        public readonly int? AddressBookSyncInterval = null;

        public readonly OptionSetValue EntityFormMode = null;

        public readonly bool? SplitViewState = null;

        public readonly int? TrackingTokenId = null;

        public readonly string HomepageLayout = null;

        public readonly string UserProfile = null;

        public readonly int? DateFormatCode = null;

        public readonly string TimeSeparator = null;

        public readonly int? PagingLimit = null;

        public readonly int? TimeZoneDaylightDay = null;

        public readonly int? NextTrackingNumber = null;

        public readonly DateTime? CreatedOn = null;

        public readonly bool? IsDefaultCountryCodeCheckEnabled = null;

        public readonly int? TimeZoneDaylightYear = null;

        public readonly int? HelpLanguageId = null;

        public readonly OptionSetValue IncomingEmailFilteringMethod = null;

        public readonly int? UILanguageId = null;

        public readonly int? TimeZoneDaylightBias = null;

        public readonly string HomepageArea = null;

        public readonly int? LongDateFormatCode = null;

        public readonly string WorkdayStartTime = null;

        public readonly int? TimeZoneDaylightDayOfWeek = null;

        public readonly string DateSeparator = null;

        public readonly EntityReference ModifiedBy = null;

        public readonly int? AutoCreateContactOnPromote = null;

        public readonly bool? IsAutoDataCaptureEnabled = null;

        public readonly int? TimeZoneBias = null;

        public readonly int? PricingDecimalPrecision = null;

        public readonly int? DefaultCalendarView = null;

        public readonly int? ResourceBookingExchangeSyncVersion = null;

        public readonly string NumberSeparator = null;

        public readonly bool? ShowWeekNumber = null;

        public readonly string NumberGroupFormat = null;

        public readonly int? TimeZoneDaylightHour = null;

        public readonly bool? IsGuidedHelpEnabled = null;

        public readonly int? TimeZoneStandardDay = null;

        public readonly bool? IgnoreUnsolicitedEmail = null;

        public readonly bool? UseCrmFormForAppointment = null;

        public readonly Guid? BusinessUnitId = null;

        public readonly string AMDesignator = null;

        public readonly bool? AllowEmailCredentials = null;

        public readonly DateTime? ModifiedOn = null;

        public readonly OptionSetValue DefaultSearchExperience = null;

        public readonly bool? UseCrmFormForEmail = null;

        public readonly int? CurrencyFormatCode = null;

        public readonly int? TimeZoneDaylightMonth = null;

        public readonly int? TimeZoneCode = null;

        public readonly string CurrencySymbol = null;

        public readonly string DateFormatString = null;

        public readonly EntityReference CreatedOnBehalfBy = null;

        public readonly string WorkdayStopTime = null;

        public readonly Guid? SystemUserId = null;

        public readonly string DefaultCountryCode = null;

        public readonly bool? UseImageStrips = null;

        public readonly int? VersionNumber = null;

        public readonly string PersonalizationSettings = null;

        public readonly int? TimeZoneStandardMonth = null;

        public readonly int? TimeZoneStandardSecond = null;

        public readonly int? TimeZoneStandardYear = null;

        public readonly int? TimeZoneStandardMinute = null;

        public readonly bool? GetStartedPaneContentEnabled = null;

        public readonly EntityReference ModifiedOnBehalfBy = null;

        public readonly OptionSetValue ReportScriptErrors = null;

        public readonly int? NegativeFormatCode = null;

        public readonly int? FullNameConventionCode = null;

        public readonly DateTime? LastAlertsViewedTime = null;

        public readonly OptionSetValue DataValidationModeForExportToExcel = null;

        public readonly int? TimeZoneDaylightSecond = null;

        public readonly int? AdvancedFindStartupMode = null;

        public readonly EntityReference CreatedBy = null;

        public readonly int? CalendarType = null;

        public readonly int? TimeZoneStandardHour = null;

        public readonly int? LocaleId = null;

        public readonly string PMDesignator = null;

        public readonly bool? IsResourceBookingExchangeSyncEnabled = null;

        public readonly string EmailPassword = null;

        public readonly bool? IsAppsForCrmAlertDismissed = null;

        public readonly bool? IsDuplicateDetectionEnabledWhenGoingOnline = null;

        public readonly bool? IsSendAsAllowed = null;

        public readonly int? NegativeCurrencyFormatCode = null;

        public readonly bool? UseCrmFormForContact = null;

        public readonly int? TimeFormatCode = null;

        public readonly int? TimeZoneStandardDayOfWeek = null;

        public readonly string HomepageSubarea = null;

        public readonly int? TimeZoneStandardBias = null;

        public readonly Guid? DefaultDashboardId = null;

        public readonly string EmailUsername = null;

        public readonly OptionSetValue VisualizationPaneLayout = null;

        public readonly EntityReference TransactionCurrencyId = null;

        public readonly bool? UseCrmFormForTask = null;

        public readonly int? CurrencyDecimalPrecision = null;

        public readonly bool? SyncContactCompany = null;

        public readonly string DecimalSymbol = null;

        public readonly int? TimeZoneDaylightMinute = null;

        public readonly string TimeFormatString = null;

    }

    [EntityName("activityparty")]
    public class ActivityParty : Entity
    {
        public static class Options
        {
            public enum InstanceTypeCode
            {
                NotRecurring,
                RecurringMaster,
                RecurringInstance,
                RecurringException,
                RecurringFutureException
            }

            public enum ParticipationTypeMask
            {
                Sender = 1,
                ToRecipient,
                CCRecipient,
                BCCRecipient,
                RequiredAttendee,
                OptionalAttendee,
                Organizer,
                Regarding,
                Owner,
                Resource,
                Customer
            }
        }

        public const string EntityLogicalName = "activityparty";

        public readonly EntityReference OwnerId;

        public readonly Guid? ActivityPartyId;

        public readonly bool? DoNotPostalMail;

        public readonly DateTime? ScheduledEnd;

        public readonly bool? DoNotEmail;

        public readonly string AddressUsed;

        public readonly bool? IsPartyDeleted;

        public readonly bool? DoNotPhone;

        public readonly EntityReference PartyId;

        public readonly Guid? OwningUser;

        public readonly string ExchangeEntryId;

        public readonly bool? DoNotFax;

        public readonly int? AddressUsedEmailColumnNumber;

        public readonly EntityReference ActivityId;

        public readonly OptionSetValue InstanceTypeCode;

        public readonly DateTime? ScheduledStart;

        public readonly int? VersionNumber;

        public readonly Guid? OwningBusinessUnit;

        public readonly double? Effort;

        public readonly EntityReference ResourceSpecId;

        public readonly OptionSetValue ParticipationTypeMask;

        [Column("activitypartyid")]
        public override Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                base.Id = value;
                base["activitypartyid"] = value;
            }
        }

        public ActivityParty() : base("activityparty")
        {
        }
    }

    [EntityName("organization")]
    public class Organization : Entity
    {
        public static class Options
        {
            public enum DateFormatCode
            {

            }

            public enum FullNameConventionCode
            {
                LastNameFirstName,
                FirstName,
                LastNameFirstNameMiddleInitial,
                FirstNameMiddleInitialLastName,
                LastNameFirstNameMiddleName,
                FirstNameMiddleNameLastName,
                LastNameSpaceFirstName,
                LastNameNoSpaceFirstName
            }

            public enum ReportScriptErrors
            {
                NoPreferenceForSendingAnErrorReportToMicrosoftAboutMicrosoftDynamics365,
                AskMeForPermissionToSendAnErrorReportToMicrosoft,
                AutomaticallySendAnErrorReportToMicrosoftWithoutAskingMeForPermission,
                NeverSendAnErrorReportToMicrosoftAboutMicrosoftDynamics365
            }

            public enum DefaultRecurrenceEndRangeType
            {
                NoEndDate = 1,
                NumberOfOccurrences,
                EndByDate
            }

            public enum FiscalPeriodFormatPeriod
            {
                Quarter0 = 1,
                Q0,
                P0,
                Month0,
                M0,
                Semester0,
                MonthName
            }

            public enum CurrencyFormatCode
            {
                _123,
                _1231,
                _1232,
                _1233
            }

            public enum TimeFormatCode
            {

            }

            public enum PluginTraceLogSetting
            {
                Off,
                Exception,
                All
            }

            public enum SharePointDeploymentType
            {
                Online,
                OnPremises
            }

            public enum FiscalYearFormatYear
            {
                YYYY = 1,
                YY,
                GGYY
            }

            public enum EmailConnectionChannel
            {
                ServerSideSynchronization,
                MicrosoftDynamics365EmailRouter
            }

            public enum NegativeFormatCode
            {
                Brackets,
                Dash,
                DashPlusSpace,
                TrailingDash,
                SpacePlusTrailingDash
            }

            public enum FiscalYearFormatSuffix
            {
                FY = 1,
                FiscalYear
            }

            public enum WeekStartDayCode
            {

            }

            public enum YammerPostMethod
            {
                Public,
                Private
            }

            public enum CurrencyDisplayOption
            {
                CurrencySymbol,
                CurrencyCode
            }

            public enum ISVIntegrationCode
            {
                None,
                Web,
                OutlookWorkstationClient,
                WebOutlookWorkstationClient,
                OutlookLaptopClient,
                WebOutlookLaptopClient,
                Outlook,
                All
            }

            public enum DiscountCalculationMethod
            {
                LineItem,
                PerUnit
            }

            public enum FiscalYearFormatPrefix
            {
                FY = 1
            }
        }

        public const string EntityLogicalName = "organization";

        public readonly int? EmailSendPollingPeriod;

        public readonly int? MinAddressBookSyncInterval;

        public readonly bool? AllowOfflineScheduledSyncs;

        public readonly int? MinOfflineSyncInterval;

        public readonly bool? IsAuditEnabled;

        public readonly Guid? UserGroupId;

        public readonly string AzureSchedulerJobCollectionName;

        public readonly Guid? SqlAccessGroupId;

        public readonly int? CurrentCategoryNumber;

        public readonly OptionSetValue DateFormatCode;

        public readonly int? RiErrorStatus;

        public readonly int? MaxProductsInBundle;

        public readonly int? PastExpansionWindow;

        public readonly bool? IsPreviewEnabledForActionCard;

        public readonly bool? IsDefaultCountryCodeCheckEnabled;

        public readonly int? CurrencyDecimalPrecision;

        public readonly bool? GlobalAppendUrlParametersEnabled;

        public readonly int? UserAccessAuditingInterval;

        public readonly string ParsedTableColumnPrefix;

        public readonly string DefaultCountryCode;

        public readonly string QuotePrefix;

        public readonly string TimeFormatString;

        public readonly bool? IgnoreInternalEmail;

        public readonly bool? NotifyMailboxOwnerOfEmailServerLevelAlerts;

        public readonly Guid? SystemUserId;

        public readonly int? YearStartWeekCode;

        public readonly bool? IsFolderAutoCreatedonSP;

        public readonly int? LocaleId;

        public readonly bool? IsActivityAnalysisEnabled;

        public readonly string ExternalBaseUrl;

        public readonly bool? SuppressSLA;

        public readonly bool? AllowAutoResponseCreation;

        public readonly int? MaxConditionsForMobileOfflineFilters;

        public readonly bool? IsDuplicateDetectionEnabledForImport;

        public readonly int? MaxAppointmentDurationDays;

        public readonly bool? GetStartedPaneContentEnabled;

        public readonly int? MobileOfflineSyncInterval;

        public readonly Guid? BusinessClosureCalendarId;

        public readonly bool? PowerBiFeatureEnabled;

        public readonly OptionSetValue FullNameConventionCode;

        public readonly int? CurrentInvoiceNumber;

        public readonly int? MaxUploadFileSize;

        public readonly bool? UseReadForm;

        public readonly string CampaignPrefix;

        public readonly string CurrencySymbol;

        public readonly OptionSetValue ReportScriptErrors;

        public readonly OptionSetValue DefaultRecurrenceEndRangeType;

        public readonly Guid? ReportingGroupId;

        public readonly Guid? DelegatedAdminUserId;

        public readonly EntityReference BaseCurrencyId;

        public readonly bool? IsAppMode;

        public readonly string OrderPrefix;

        public readonly string OfficeGraphDelveUrl;

        public readonly OptionSetValue FiscalPeriodFormatPeriod;

        public readonly string PrivacyStatementUrl;

        public readonly int? VersionNumber;

        public readonly Guid? PrivilegeUserGroupId;

        public readonly bool? IsEmailMonitoringAllowed;

        public readonly OptionSetValue CurrencyFormatCode;

        public readonly string HighContrastThemeData;

        public readonly string DefaultThemeData;

        public readonly string DefaultEmailSettings;

        public readonly int? UniqueSpecifierLength;

        public readonly string WidgetProperties;

        public readonly bool? ShowWeekNumber;

        public readonly bool? AppDesignerExperienceEnabled;

        public readonly string DecimalSymbol;

        public readonly bool? AutoApplySLA;

        public readonly string AMDesignator;

        public readonly bool? CascadeStatusUpdate;

        public readonly int? LanguageCode;

        public readonly int? NextCustomObjectTypeCode;

        public readonly bool? IsConflictDetectionEnabledForMobileClient;

        public readonly string FiscalYearFormat;

        public readonly bool? EnablePricingOnCreate;

        public readonly bool? ShowKBArticleDeprecationNotification;

        public readonly int? TrackingTokenIdDigits;

        public readonly string GlobalHelpUrl;

        public readonly string ReferenceSiteMapXml;

        public readonly DateTime? CreatedOn;

        public readonly bool? IsDelveActionHubIntegrationEnabled;

        public readonly string NumberGroupFormat;

        public readonly int? FiscalPeriodType;

        public readonly bool? IsOfficeGraphEnabled;

        public readonly int? NextTrackingNumber;

        public readonly bool? GlobalHelpUrlEnabled;

        public readonly bool? AllowAutoUnsubscribeAcknowledgement;

        public readonly string BingMapsApiKey;

        public readonly int? CurrentKaNumber;

        public readonly bool? IsAutoDataCaptureEnabled;

        public readonly bool? UseLegacyRendering;

        public readonly int? PinpointLanguageCode;

        public readonly string InitialVersion;

        public readonly EntityReference ModifiedOnBehalfBy;

        public readonly bool? GrantAccessToNetworkService;

        public readonly int? MaxRecordsForExportToExcel;

        public readonly int? MaxRecordsForLookupFilters;

        public readonly bool? IsFullTextSearchEnabled;

        public readonly string ExternalPartyEntitySettings;

        public readonly bool? IsSOPIntegrationEnabled;

        public readonly OptionSetValue TimeFormatCode;

        public readonly bool? DisplayNavigationTour;

        public readonly bool? OrgInsightsEnabled;

        public readonly Guid? SampleDataImportId;

        public readonly int? MobileOfflineMinLicenseProd;

        public readonly EntityReference CreatedOnBehalfBy;

        public readonly string KMSettings;

        public readonly bool? GenerateAlertsForInformation;

        public readonly int? CurrentQuoteNumber;

        public readonly bool? SocialInsightsEnabled;

        public readonly bool? UseInbuiltRuleForDefaultPricelistSelection;

        public readonly int? HashDeltaSubjectCount;

        public readonly EntityReference AcknowledgementTemplateId;

        public readonly string OrgDbOrgSettings;

        public readonly DateTime? MetadataSyncLastTimeOfNeverExpiredDeletedObjects;

        public readonly string TrackingPrefix;

        public readonly OptionSetValue PluginTraceLogSetting;

        public readonly string TokenKey;

        public readonly int? MaxVerboseLoggingMailbox;

        public readonly string FiscalPeriodFormat;

        public readonly string KaPrefix;

        public readonly string SiteMapXml;

        public readonly string NumberFormat;

        public readonly bool? IsDisabled;

        public readonly int? ExpireChangeTrackingInDays;

        public readonly bool? IsAppointmentAttachmentSyncEnabled;

        public readonly Guid? SupportUserId;

        public readonly int? CurrentKbNumber;

        public readonly int? LongDateFormatCode;

        public readonly string SchemaNamePrefix;

        public readonly bool? IsFiscalPeriodMonthBased;

        public readonly int? TagMaxAggressiveCycles;

        public readonly int? MaxSupportedInternetExplorerVersion;

        public readonly int? MaximumDynamicPropertiesAllowed;

        public readonly string BulkOperationPrefix;

        public readonly bool? IsPresenceEnabled;

        public readonly bool? IsEmailServerProfileContentFilteringEnabled;

        public readonly int? MailboxPermanentIssueMinRange;

        public readonly bool? IsPreviewForAutoCaptureEnabled;

        public readonly int? BaseCurrencyPrecision;

        public readonly int? TrackingTokenIdBase;

        public readonly bool? UseSkypeProtocol;

        public readonly bool? IsMailboxForcedUnlockingEnabled;

        public readonly string CategoryPrefix;

        public readonly bool? UsePositionHierarchy;

        public readonly string ReportingGroupName;

        public readonly string HashFilterKeywords;

        public readonly bool? IsFolderBasedTrackingEnabled;

        public readonly OptionSetValue SharePointDeploymentType;

        public readonly int? CurrentParsedTableNumber;

        public readonly bool? EnableBingMapsIntegration;

        public readonly bool? QuickFindRecordLimitEnabled;

        public readonly bool? AllowUnresolvedPartiesOnEmailSend;

        public readonly bool? IsActionCardEnabled;

        public readonly string BlockedAttachments;

        public readonly bool? EnableLPAuthoring;

        public readonly string InvoicePrefix;

        public readonly bool? IsDuplicateDetectionEnabledForOnlineCreateUpdate;

        public readonly bool? AllowOutlookScheduledSyncs;

        public readonly bool? IsAssignedTasksSyncEnabled;

        public readonly bool? RestrictStatusUpdate;

        public readonly bool? IsMobileClientOnDemandSyncEnabled;

        public readonly OptionSetValue FiscalYearFormatYear;

        public readonly bool? RenderSecureIFrameForEmail;

        public readonly bool? GenerateAlertsForWarnings;

        public readonly OptionSetValue EmailConnectionChannel;

        public readonly string V3CalloutConfigHash;

        public readonly string DisabledReason;

        public readonly bool? CortanaProactiveExperienceEnabled;

        public readonly string DateSeparator;

        public readonly string CasePrefix;

        public readonly Guid? PrivReportingGroupId;

        public readonly string DateFormatString;

        public readonly bool? IsResourceBookingExchangeSyncEnabled;

        public readonly int? CalendarType;

        public readonly int? IncomingEmailExchangeEmailRetrievalBatchSize;

        public readonly OptionSetValue NegativeFormatCode;

        public readonly int? MaxFolderBasedTrackingMappings;

        public readonly bool? AutoApplyDefaultonCaseUpdate;

        public readonly int? FiscalYearDisplayCode;

        public readonly string ContractPrefix;

        public readonly int? RecurrenceExpansionSynchCreateMax;

        public readonly string PMDesignator;

        public readonly bool? SQMEnabled;

        public readonly bool? UnresolveEmailAddressIfMultipleMatch;

        public readonly bool? IsDuplicateDetectionEnabledForOfflineSync;

        public readonly int? GoalRollupExpiryTime;

        public readonly string PrivReportingGroupName;

        public readonly int? MinOutlookSyncInterval;

        public readonly int? CurrentImportSequenceNumber;

        public readonly string PostMessageWhitelistDomains;

        public readonly bool? TaskBasedFlowEnabled;

        public readonly Guid? IntegrationUserId;

        public readonly bool? SocialInsightsTermsAccepted;

        public readonly bool? OfficeAppsAutoDeploymentEnabled;

        public readonly bool? IsPreviewForEmailMonitoringAllowed;

        public readonly string KbPrefix;

        public readonly bool? FiscalSettingsUpdated;

        public readonly string TimeSeparator;

        public readonly bool? DisableSocialCare;

        public readonly bool? IsDuplicateDetectionEnabled;

        public readonly string ExternalPartyCorrelationKeys;

        public readonly bool? AllowAutoUnsubscribe;

        public readonly bool? GenerateAlertsForErrors;

        public readonly string SqlAccessGroupName;

        public readonly int? FutureExpansionWindow;

        public readonly int? MetadataSyncTimestamp;

        public readonly int? MaximumSLAKPIPerEntityWithActiveSLA;

        public readonly string SlaPauseStates;

        public readonly string BaseISOCurrencyCode;

        public readonly int? MobileOfflineMinLicenseTrial;

        public readonly bool? EnforceReadOnlyPlugins;

        public readonly byte[] EntityImage;

        public readonly int? CurrentCaseNumber;

        public readonly EntityReference DefaultMobileOfflineProfileId;

        public readonly bool? OOBPriceCalculationEnabled;

        public readonly int? TagPollingPeriod;

        public readonly bool? IsMobileOfflineEnabled;

        public readonly EntityReference ModifiedBy;

        public readonly string WebResourceHash;

        public readonly OptionSetValue FiscalYearFormatSuffix;

        public readonly int? RecurrenceDefaultNumberOfOccurrences;

        public readonly int? RecurrenceExpansionJobBatchInterval;

        public readonly int? RecurrenceExpansionJobBatchSize;

        public readonly bool? AutoApplyDefaultonCaseCreate;

        public readonly string ParsedTablePrefix;

        public readonly bool? IsDelegateAccessEnabled;

        public readonly bool? IsAutoSaveEnabled;

        public readonly string NumberSeparator;

        public readonly int? CurrentCampaignNumber;

        public readonly int? MaximumEntitiesWithActiveSLA;

        public readonly int? SortId;

        public readonly bool? AllowWebExcelExport;

        public readonly bool? AllowClientMessageBarAd;

        public readonly bool? AllowUsersSeeAppdownloadMessage;

        public readonly OptionSetValue WeekStartDayCode;

        public readonly DateTime? FiscalCalendarStart;

        public readonly int? MaxDepthForHierarchicalSecurityModel;

        public readonly int? CurrentOrderNumber;

        public readonly int? HashMaxCount;

        public readonly bool? AllowMarketingEmailExecution;

        public readonly int? MailboxIntermittentIssueMinRange;

        public readonly OptionSetValue YammerPostMethod;

        public readonly bool? IsMailboxInactiveBackoffEnabled;

        public readonly bool? IsHierarchicalSecurityModelEnabled;

        public readonly bool? EmailCorrelationEnabled;

        public readonly int? CurrentContractNumber;

        public readonly int? ExpireSubscriptionsInDays;

        public readonly int? MaxVerboseLoggingSyncCycles;

        public readonly string SignupOutlookDownloadFWLink;

        public readonly bool? TextAnalyticsEnabled;

        public readonly string FeatureSet;

        public readonly bool? IsRelationshipInsightsEnabled;

        public readonly string Picture;

        public readonly bool? IsUserAccessAuditEnabled;

        public readonly int? DaysSinceRecordLastModifiedMaxValue;

        public readonly int? GoalRollupFrequency;

        public readonly bool? ShareToPreviousOwnerOnAssign;

        public readonly int? MaximumActiveBusinessProcessFlowsAllowedPerEntity;

        public readonly bool? RequireApprovalForUserEmail;

        public readonly bool? RequireApprovalForQueueEmail;

        public readonly bool? AllowUserFormModePreference;

        [Key]
        public readonly string Name;

        public readonly string DefaultCrmCustomName;

        public readonly int? YammerGroupId;

        public readonly string SocialInsightsInstance;

        public readonly bool? CreateProductsWithoutParentInActiveState;

        public readonly bool? EnableMicrosoftFlowIntegration;

        public readonly Guid? OrganizationId;

        public readonly bool? YammerOAuthAccessTokenExpired;

        public readonly string ACIWebEndpointUrl;

        public readonly int? TimeZoneRuleVersionNumber;

        public readonly bool? IsExternalSearchIndexEnabled;

        public readonly int? TokenExpiry;

        public readonly bool? IsEnabledForAllRoles;

        public readonly OptionSetValue CurrencyDisplayOption;

        public readonly OptionSetValue ISVIntegrationCode;

        public readonly EntityReference DefaultEmailServerProfileId;

        public readonly OptionSetValue DiscountCalculationMethod;

        public readonly string BaseCurrencySymbol;

        public readonly bool? IsContactMailingAddressSyncEnabled;

        public readonly OptionSetValue FiscalYearFormatPrefix;

        public readonly bool? IsOneDriveEnabled;

        public readonly bool? AllowAddressBookSyncs;

        public readonly DateTime? ModifiedOn;

        public readonly string FiscalYearPeriodConnect;

        public readonly bool? EnableSmartMatching;

        public readonly EntityReference CreatedBy;

        public readonly int? HashMinAddressCount;

        public readonly bool? AllowEntityOnlyAudit;

        public readonly bool? ProductRecommendationsEnabled;

        public readonly int? NegativeCurrencyFormatCode;

        public readonly string YammerNetworkPermalink;

        public readonly int? MaximumTrackingNumber;

        public readonly int? PricingDecimalPrecision;

        public readonly int? CurrentBulkOperationNumber;

        public readonly int? UTCConversionTimeZoneCode;

        [Column("organizationid")]
        public override Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                base.Id = value;
                base["organizationid"] = value;
            }
        }

        public Organization() : base("organization")
        {
        }
    }

    [EntityLogicalName("new_testentity")]
    public class new_testentity : UserOrTeamEntity
    {
        public const string EntityLogicalName = "new_testentity";
        public new_testentity() : base(EntityLogicalName)
        {
        }

        [Column("new_testentityid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["new_testentityid"] = value;
            }
        }

        public readonly decimal? ExchangeRate = null;

        public readonly EntityReference new_extracostid = null;

        public readonly Money new_productioncost_Base = null;

        public readonly EntityReference TransactionCurrencyId = null;

        public readonly Guid? new_testentityId = null;

        public readonly Money new_taxamount_Base = null;

        public readonly string new_name = null;

        public readonly Money new_productioncost = null;

        public readonly Money new_taxamount = null;

        public readonly Money new_totalprice = null;

        public readonly Money new_totalprice_Base = null;

        public readonly string new_extracostdescription = null;

        public readonly Money new_extracostamount = null;

    }

    [EntityLogicalName("xts_entity")]
    public partial class xts_entity : Entity
    {
        public const string EntityLogicalName = "xts_entity";
        public const int EntityTypeCode = 1234;
        public xts_entity() : base(EntityLogicalName)
        {
        }

        [Column("xts_entityid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["xts_entityid"] = value;
            }
        }

        [Column("xts_column")]
        public readonly string xts_withcolumnattribute = null;
        public readonly EntityReference xts_attribute = null;
        public readonly OptionSetValue AttributeWithCaseChar = null;
        public readonly Money xts_money = null;
        public readonly Money xts_othermoney = null;
        public readonly Money xts_totalmoney = null;
        public readonly DateTime? xts_datetime = null;
        public readonly OptionSetValue xts_optionsetvalue = null;
        public readonly OptionSetValue xts_otheroptionsetvalue = null;
        public readonly EntityReference xts_referenceid = null;
        public readonly EntityReference xts_lookuptoheaderid = null;
        public readonly decimal? xts_decimal = null;
        public readonly decimal? xts_otherdecimal = null;
        public readonly float? xts_float = null;
        public readonly float? xts_otherfloat = null;
        public readonly int? xts_int = null;
        public readonly int? xts_otherint = null;
        public readonly string xts_string = null;
        public readonly string xts_otherstring = null;
        public readonly EntityCollection xts_activityparties = null;
    }

    [EntityLogicalName("xts_relatedentity")]
    public partial class xts_relatedentity : Entity
    {
        public const string EntityLogicalName = "xts_relatedentity";
        public const int EntityTypeCode = 2345;
        public xts_relatedentity() : base(EntityLogicalName)
        {
        }

        [Column("xts_entityid")]
        public override Guid Id { get { return base.Id; } set { base.Id = value; } }

        [Column("xts_column")]
        public readonly string xts_withcolumnattribute = null;
        public readonly string xts_headernumber = null;
        public readonly EntityReference xts_relatedid = null;
        public readonly OptionSetValue AttributeWithCaseChar = null;
        public readonly Money xts_money = null;
        public readonly string xts_name = null;
    }

    public partial class xts_derivedentity : xts_entity
    {
        public readonly EntityReference xts_derivedattribute = null;
    }

    [EntityLogicalName("xts_earlyboundentity")]
    public partial class xts_earlyboundentity : Entity, INotifyPropertyChanging, INotifyPropertyChanged
    {
        public const string EntityLogicalName = "xts_earlyboundentity";
        public const int EntityTypeCode = 2345;
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public xts_earlyboundentity() : base(EntityLogicalName)
        {
        }

        private void OnPropertyChanging(string propertyName)
        {
            var handler = PropertyChanging;
            if (handler != null)
                handler(this, new PropertyChangingEventArgs(propertyName));
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        [AttributeLogicalName("xts_earlyboundentityid")]
        public override Guid Id { get { return base.Id; } set { base.Id = value; } }

        [AttributeLogicalName("xts_withcolumnattribute")]
        public string xts_withcolumnattribute
        {
            get { return GetAttributeValue<string>("xts_withcolumnattribute"); }
            set
            {
                OnPropertyChanging("xts_withcolumnattribute");
                SetAttributeValue("xts_withcolumnattribute", value);
                OnPropertyChanged("xts_withcolumnattribute");
            }
        }

        [AttributeLogicalName("xts_attribute")]
        public EntityReference xts_attribute
        {
            get { return GetAttributeValue<EntityReference>("xts_attribute"); }
            set
            {
                OnPropertyChanging("xts_attribute");
                SetAttributeValue("xts_attribute", value);
                OnPropertyChanged("xts_attribute");
            }
        }

        [AttributeLogicalName("attributewithcasechar")]
        public OptionSetValue AttributeWithCaseChar
        {
            get { return GetAttributeValue<OptionSetValue>("attributewithcasechar"); }
            set
            {
                OnPropertyChanging("attributewithcasechar");
                SetAttributeValue("attributewithcasechar", value);
                OnPropertyChanged("attributewithcasechar");
            }
        }
    }

    [System.Runtime.Serialization.DataContractAttribute]
    [Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("xts_taxmaster")]
    public class xts_taxmaster : OrganizationEntity
    {
        public const string EntityLogicalName = "xts_taxmaster";
        public xts_taxmaster() : base(EntityLogicalName)
        {
        }

        [System.ComponentModel.DataAnnotations.Schema.Column("xts_taxmasterid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["xts_taxmasterid"] = value;
            }
        }

        public Guid? xts_taxmasterId = null;

        public EntityReference OrganizationId = null;

        public string xts_tax = null;

        public string xts_description = null;

        public OptionSetValue xts_basecalculation = null;

        public new static class Options
        {
            public enum xts_basecalculation
            {
                TaxInclusive = 1,
                TaxExclusive = 2

            }
            public enum statuscode
            {
                Active = 1,
                Inactive = 2

            }
            public enum statecode
            {
                Active = 0,
                Inactive = 1

            }

        }
    }
    [System.Runtime.Serialization.DataContractAttribute]
    [Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("xts_taxmasterdetail")]
    public class xts_taxmasterdetail : OrganizationEntity
    {
        public const string EntityLogicalName = "xts_taxmasterdetail";
        public xts_taxmasterdetail() : base(EntityLogicalName)
        {
        }

        [System.ComponentModel.DataAnnotations.Schema.Column("xts_taxmasterdetailid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["xts_taxmasterdetailid"] = value;
            }
        }

        public EntityReference xts_taxid = null;

        public Guid? xts_taxmasterdetailId = null;

        public EntityReference OrganizationId = null;

        public string xts_taxmasterdetailnumber = null;

        public DateTime? xts_effectivedate = null;

        public double? xts_taxrate = null;

        public new static class Options
        {
            public enum statuscode
            {
                Active = 1,
                Inactive = 2

            }
            public enum statecode
            {
                Active = 0,
                Inactive = 1

            }

        }
    }

    [System.Runtime.Serialization.DataContractAttribute]
    [Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("transactioncurrency")]
    public class TransactionCurrency : Entity
    {
        public const string EntityLogicalName = "transactioncurrency";
        public TransactionCurrency() : base(EntityLogicalName)
        {
        }

        [System.ComponentModel.DataAnnotations.Schema.Column("transactioncurrencyid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["transactioncurrencyid"] = value;
            }
        }

        public readonly string CurrencyName = null;

        public readonly DateTime? OverriddenCreatedOn = null;

        public readonly string ISOCurrencyCode = null;

        public readonly EntityReference CreatedOnBehalfBy = null;

        public readonly EntityReference ModifiedOnBehalfBy = null;

        public readonly int? VersionNumber = null;

        public readonly OptionSetValue StatusCode = null;

        public readonly int? CurrencyPrecision = null;

        public readonly decimal? ExchangeRate = null;

        public readonly byte[] EntityImage = null;

        public readonly int? ImportSequenceNumber = null;

        public readonly string CurrencySymbol = null;

        public readonly DateTime? CreatedOn = null;

        public readonly EntityReference OrganizationId = null;

        public readonly DateTime? ModifiedOn = null;

        public readonly EntityReference ModifiedBy = null;

        public readonly Guid? UniqueDscId = null;

        public readonly Guid? TransactionCurrencyId = null;

        public readonly OptionSetValue StateCode = null;

        public readonly EntityReference CreatedBy = null;

        public new static class Options
        {
            public enum StatusCode
            {
                Active = 1,
                Inactive = 2

            }
            public enum StateCode
            {
                Active = 0,
                Inactive = 1

            }

        }
    }

    [System.Runtime.Serialization.DataContractAttribute]
    [Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("xts_generalsetup")]
    public class xts_generalsetup : OrganizationEntity
    {
        public const string EntityLogicalName = "xts_generalsetup";
        public xts_generalsetup() : base(EntityLogicalName)
        {
        }

        [System.ComponentModel.DataAnnotations.Schema.Column("xts_generalsetupid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["xts_generalsetupid"] = value;
            }
        }

        public readonly DateTime? xts_allowtransactionto = null;

        public readonly Guid? xts_generalsetupId = null;

        public readonly EntityReference OrganizationId = null;

        public readonly byte[] EntityImage = null;
        [System.ComponentModel.DataAnnotations.Schema.Column("xts_generalsetup")]
        public readonly string xts_generalsetup1 = null;

        public readonly DateTime? xts_allowtransactionfrom = null;

        public new static class Options
        {
            public enum statuscode
            {
                Active = 1,
                Inactive = 2

            }
            public enum statecode
            {
                Active = 0,
                Inactive = 1

            }

        }
    }

    [EntityLogicalName("xts_keytest")]
    public partial class xts_keytest : Entity
    {
        public const string EntityLogicalName = "xts_keytest";
        public const int EntityTypeCode = 1234;
        public xts_keytest() : base(EntityLogicalName)
        {
        }

        [Column("xts_keytestid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["xts_keytestid"] = value;
            }
        }

        [Column("primarynameattributekey")]
        [Key]
        public readonly string xts_key = null;

        public readonly Money xts_money = null;
        public readonly Money xts_othermoney = null;
        public readonly Money xts_totalmoney = null;
        public readonly DateTime? xts_datetime = null;
        public readonly OptionSetValue xts_optionsetvalue = null;
        public readonly OptionSetValue xts_otheroptionsetvalue = null;
        public readonly EntityReference xts_referenceid = null;
        public readonly EntityReference xts_lookuptoheaderid = null;
    }

    [EntityLogicalName("xts_nokeytest")]
    public partial class xts_nokeytest : Entity
    {
        public const string EntityLogicalName = "xts_nokeytest";
        public const int EntityTypeCode = 1234;
        public xts_nokeytest() : base(EntityLogicalName)
        {
        }

        [Column("xts_nokeytestid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["xts_nokeytestid"] = value;
            }
        }

        [Column("PrimaryNameAttributeKey")]
        public readonly string xts_key = null;
    }

    [EntityLogicalName("xts_activestatecodetest")]
    public partial class xts_activestatecodetest : Entity
    {
        public const string EntityLogicalName = "xts_activestatecodetest";
        public const int EntityTypeCode = 1234;
        public xts_activestatecodetest() : base(EntityLogicalName)
        {
        }

        [Column("xts_activestatecodetestid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["xts_activestatecodetestid"] = value;
            }
        }

        [Column("PrimaryNameAttributeKey")]
        [Key]
        public readonly string xts_key = null;
        [System.ComponentModel.DescriptionAttribute("{\"a\":12}")]
        public readonly OptionSetValue statecode = null;
    }
}
