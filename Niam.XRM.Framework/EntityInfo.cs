using System.Collections.Generic;
using System.Runtime.Serialization;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework
{
    [DataContract]
    public class EntityInfo : IEntityInfo
    {
        [IgnoreDataMember]
        public string LogicalName { get; set; }

        [IgnoreDataMember]
        public IDictionary<string, string> Attributes { get; set; }

        [IgnoreDataMember]
        public string PrimaryNameAttribute { get; set; }

        [DataMember(Name = "a", EmitDefaultValue = false)]
        public int? StateCodeActiveValue { get; set; }
    }
}