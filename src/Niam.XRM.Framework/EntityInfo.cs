using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework
{
    [DataContract]
    public class EntityInfo : IEntityInfo
    {
        [IgnoreDataMember]
        public bool IsCrmSvcUtilGenerated { get; set; }

        [IgnoreDataMember]
        public string LogicalName { get; set; }

        [IgnoreDataMember]
        public IDictionary<string, string> DataMap { get; set; } = new Dictionary<string, string>();

        [IgnoreDataMember]
        public string PrimaryNameAttribute { get; set; }

        [DataMember(Name = "a", EmitDefaultValue = false)]
        public int? StateCodeActiveValue { get; set; }

        [IgnoreDataMember]
        public IDictionary<string, PropertyInfo> Properties { get; set; } = new Dictionary<string, PropertyInfo>();

        public string GetAttributeName(string memberName) => 
            DataMap.TryGetValue(memberName, out var result) ? result : null;

        public string GetMemberName(string attributeName) =>
            DataMap.TryGetValue(attributeName, out var result) ? result : null;

        public PropertyInfo GetPropertyInfo(string propertyName) => Properties[propertyName];
    }
}