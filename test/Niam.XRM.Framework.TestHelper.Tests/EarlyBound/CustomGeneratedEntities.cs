using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Niam.XRM.Framework.TestHelper.Tests
{
    [System.Runtime.Serialization.DataContractAttribute]
    [EntityLogicalName("new_order")]
    public partial class new_order : Entity
    {
        public const string EntityLogicalName = "new_order";
        
        public new_order() : base(EntityLogicalName)
        {
        }
        
        public new_order(Guid id) : base(EntityLogicalName, id)
        {
        }

        [Column("new_orderid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["new_orderid"] = value;
            }
        }

        public readonly string new_name = null;
    }

    [System.Runtime.Serialization.DataContractAttribute]
    [EntityLogicalName("new_orderdetail")]
    public partial class new_orderdetail : Entity
    {
        public const string EntityLogicalName = "new_orderdetail";
        
        public new_orderdetail() : base(EntityLogicalName)
        {
        }
        
        public new_orderdetail(Guid id) : base(EntityLogicalName, id)
        {
        }

        [Column("new_orderdetailid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["new_orderdetailid"] = value;
            }
        }

        public readonly EntityReference new_orderid = null;
        public readonly EntityReference new_orderdetailsummaryid = null;
        public readonly int? new_quantity = null;
        public readonly Money new_priceperitem = null;
    }
    
    [System.Runtime.Serialization.DataContractAttribute]
    [EntityLogicalName("new_orderdetailsummary")]
    public partial class new_orderdetailsummary : Entity
    {
        public const string EntityLogicalName = "new_orderdetailsummary";
        
        public new_orderdetailsummary() : base(EntityLogicalName)
        {
        }
        
        public new_orderdetailsummary(Guid id) : base(EntityLogicalName, id)
        {
        }

        [Column("new_orderdetailsummaryid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["new_orderdetailsummaryid"] = value;
            }
        }

        public readonly EntityReference new_orderid = null;
        public readonly EntityReference new_orderdetailid = null;
        public readonly Money new_totalprice = null;
    }
}
