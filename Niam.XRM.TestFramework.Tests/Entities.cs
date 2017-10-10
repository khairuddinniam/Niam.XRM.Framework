using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Niam.XRM.TestFramework.Tests
{
    [EntityLogicalName("xts_entity")]
    public partial class xts_entity : Entity
    {
        public const string EntityLogicalName = "xts_entity";
        public xts_entity() : base(EntityLogicalName)
        {
        }

        [Column("xts_entityid")]
        public override Guid Id { get { return base.Id; } set { base.Id = value; } }

        [Column("xts_column")]
        public readonly string xts_withcolumnattribute = null;
        public readonly EntityReference xts_attribute = null;
        public readonly OptionSetValue AttributeWithCaseChar = null;
        public readonly Money xts_money = null;
        public readonly EntityReference xts_relatedid = null;
        public readonly DateTime? xts_date = null;
        public readonly int? xts_int = null;
    }

    [EntityLogicalName("xts_relatedentity")]
    public partial class xts_relatedentity : Entity
    {
        public const string EntityLogicalName = "xts_relatedentity";
        public xts_relatedentity() : base(EntityLogicalName)
        {
        }

        [Column("xts_relatedentityid")]
        public override Guid Id { get { return base.Id; } set { base.Id = value; } }
        
        public readonly EntityReference xts_referenceid = null;
        public readonly OptionSetValue xts_option = null;
        public readonly Money xts_money = null;
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
}
