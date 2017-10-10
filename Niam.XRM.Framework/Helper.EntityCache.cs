using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Niam.XRM.Framework.Interfaces;

namespace Niam.XRM.Framework
{
    public static partial class Helper
    {
        internal static class EntityCache
        {
            internal static readonly ConcurrentDictionary<string,IEntityInfo> Infos 
                = new ConcurrentDictionary<string, IEntityInfo>();
        }

        private static class EntityCache<T> where T : Entity
        {
            internal static readonly IEntityInfo Info;

            static EntityCache()
            {
                var attributes = new Dictionary<string, string>();
                const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                var memberInfos = typeof(T).GetMembers(bindingFlags).ToArray();
                foreach (var mi in memberInfos)
                    attributes[mi.Name] = GetAttributeName(mi);

                var info = new EntityInfo
                {
                    LogicalName = GetEntityLogicalName(),
                    Attributes = attributes,
                    PrimaryNameAttribute = GetPrimaryNameAttribute(memberInfos, attributes),
                    StateCodeActiveValue = GetStateCodeActiveValue(memberInfos, attributes)
                };

                EntityCache.Infos.TryAdd(info.LogicalName, info);
                Info = info;
            }

            private static string GetEntityLogicalName()
            {
                var nameAttribute = typeof (T).GetCustomAttribute<EntityLogicalNameAttribute>();
                if (nameAttribute == null)
                    throw new InvalidOperationException("The type doesn't have EntityLogicalNameAttribute.");

                return nameAttribute.LogicalName;
            }

            private static string GetAttributeName(MemberInfo mi)
            {
                var columnAttribute = mi.GetCustomAttribute<ColumnAttribute>();
                var attributeName = columnAttribute != null ? columnAttribute.Name : mi.Name.ToLower();
                return attributeName;
            }

            private static string GetPrimaryNameAttribute(IEnumerable<MemberInfo> memberInfos, IDictionary<string, string> attributes)
            {
                var keyMemberInfo = memberInfos.FirstOrDefault(mi => mi.GetCustomAttribute<KeyAttribute>() != null);
                return keyMemberInfo != null ? attributes[keyMemberInfo.Name] : null;
            }

            private static int? GetStateCodeActiveValue(IEnumerable<MemberInfo> memberInfos, IDictionary<string, string> attributes)
            {
                var stateCodeAttribute = attributes.FirstOrDefault(p => p.Value == "statecode");
                if (stateCodeAttribute.Key == null) return null;

                var memberInfo = memberInfos.First(mi => mi.Name == stateCodeAttribute.Key);
                var descAttributes = memberInfo.GetCustomAttributes<DescriptionAttribute>().ToArray();
                if (!descAttributes.Any()) return null;

                var attributeInfo = descAttributes.First().Description.FromJson<EntityInfo>();
                return attributeInfo.StateCodeActiveValue;
            }
        }
    }
}
