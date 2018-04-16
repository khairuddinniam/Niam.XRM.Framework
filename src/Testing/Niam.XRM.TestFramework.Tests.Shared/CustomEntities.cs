using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niam.XRM.TestFramework.Tests
{
    [System.Runtime.Serialization.DataContractAttribute]
    [EntityLogicalName("new_customentity")]
    public partial class new_customentity : Entity
    {
        public const string EntityLogicalName = "new_customentity";
        public const int EntityTypeCode = 1234;
        public new_customentity() : base(EntityLogicalName)
        {
        }

        [Column("new_customentityid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["new_customentityid"] = value;
            }
        }

        public readonly Money new_baseprice = null;
        public readonly Money new_totalprice = null;
        public readonly EntityReference new_taxid = null;
    }

    [System.Runtime.Serialization.DataContractAttribute]
    [EntityLogicalName("new_tax")]
    public partial class new_tax : Entity
    {
        public const string EntityLogicalName = "new_tax";
        public const int EntityTypeCode = 4567;
        public new_tax() : base(EntityLogicalName)
        {
        }

        [Column("new_taxid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["new_taxid"] = value;
            }
        }

        public readonly Money new_taxamount = null;
    }

    [System.Runtime.Serialization.DataContractAttribute]
    [EntityLogicalName("lead")]
    public partial class Lead : Entity
    {
        public const string EntityLogicalName = "lead";
        public Lead() : base(EntityLogicalName)
        {
        }

        [Column("leadid")]
        public override Guid Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                this["leadid"] = value;
            }
        }

        public readonly string Subject = null;
    }
}
