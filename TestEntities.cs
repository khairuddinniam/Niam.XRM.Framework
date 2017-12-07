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
