using System;
using System.CodeDom.Compiler;
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
                var dataMap = new Dictionary<string, string>();
                var propertyMap = new Dictionary<string, PropertyInfo>();
                const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                var memberInfos = typeof(T).GetMembers(bindingFlags)
                    .Where(mi => mi.MemberType == MemberTypes.Field || mi.MemberType == MemberTypes.Property)
                    .ToArray();

                foreach (var mi in memberInfos)
                {
                    var memberName = mi.Name;
                    var attributeName = GetAttributeName(mi);
                    dataMap[memberName] = attributeName;
                    dataMap[attributeName] = memberName;

                    if (mi.MemberType == MemberTypes.Property)
                        propertyMap[memberName] = (PropertyInfo) mi;
                }
                
                var info = typeof(T) == typeof(Entity)
                    ? new EntityInfo { LogicalName = "CRM_SDK_ENTITY" } 
                    : new EntityInfo
                    {
                        IsCrmSvcUtilGenerated = GetIsCrmSvcUtilGenerated(),
                        LogicalName = GetEntityLogicalName(),
                        DataMap = dataMap,
                        Properties = propertyMap,
                        PrimaryNameAttribute = GetPrimaryNameAttribute(memberInfos, dataMap),
                        StateCodeActiveValue = GetStateCodeActiveValue(memberInfos, dataMap)
                    };

                EntityCache.Infos.TryAdd(info.LogicalName, info);
                Info = info;
            }

            private static bool GetIsCrmSvcUtilGenerated()
            {
                var attribute = typeof(T).GetCustomAttribute<GeneratedCodeAttribute>();
                return attribute?.Tool == "CrmSvcUtil";
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
                var logicalNameAttribute = mi.GetCustomAttribute<AttributeLogicalNameAttribute>();
                if (logicalNameAttribute != null)
                    return logicalNameAttribute.LogicalName;

                var columnAttribute = mi.GetCustomAttribute<ColumnAttribute>();
                var attributeName = columnAttribute != null ? columnAttribute.Name : mi.Name.ToLower();
                return attributeName;
            }

            private static string GetPrimaryNameAttribute(IEnumerable<MemberInfo> memberInfos, IDictionary<string, string> dataMap)
            {
                var keyMemberInfo = memberInfos.FirstOrDefault(mi => mi.GetCustomAttribute<KeyAttribute>() != null);
                return keyMemberInfo != null ? dataMap[keyMemberInfo.Name] : null;
            }

            private static int? GetStateCodeActiveValue(IEnumerable<MemberInfo> memberInfos, IDictionary<string, string> dataMap)
            {
                if (!dataMap.TryGetValue("statecode", out var stateCodeMemberName)) return null;

                var memberInfo = memberInfos.First(mi => mi.Name == stateCodeMemberName);
                var descAttributes = memberInfo.GetCustomAttributes<DescriptionAttribute>().ToArray();
                if (!descAttributes.Any()) return null;

                var attributeInfo = descAttributes.First().Description.FromJson<EntityInfo>();
                return attributeInfo.StateCodeActiveValue;
            }
        }
    }
}
