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
            private static readonly ConcurrentDictionary<Type, IEntityInfo> Infos 
                = new ConcurrentDictionary<Type, IEntityInfo>();

            public static void Clear() => Infos.Clear();

            public static bool TryGetValue(string entityName, out IEntityInfo value)
            {
                value = Infos.Values.FirstOrDefault(i => i.LogicalName == entityName);
                return value != null;
            }
            
            public static IEntityInfo GetOrAddInfo(Type entityType)
            {
                if (Infos.TryGetValue(entityType, out var cached)) return cached;

                var dataMap = new Dictionary<string, string>();
                var propertyMap = new Dictionary<string, PropertyInfo>();
                const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                var memberInfos = entityType.GetMembers(bindingFlags)
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

                var info = entityType == typeof(Entity)
                    ? new EntityInfo { LogicalName = "CRM_SDK_ENTITY" }
                    : new EntityInfo
                    {
                        IsCrmSvcUtilGenerated = GetIsCrmSvcUtilGenerated(entityType),
                        LogicalName = GetEntityLogicalName(entityType),
                        DataMap = dataMap,
                        Properties = propertyMap,
                        PrimaryNameAttribute = GetPrimaryNameAttribute(memberInfos, dataMap),
                        StateCodeActiveValue = GetStateCodeActiveValue(memberInfos, dataMap)
                    };

                Infos.TryAdd(entityType, info);

                return info;
            }

            private static bool GetIsCrmSvcUtilGenerated(Type entityType)
            {
                var attribute = entityType.GetCustomAttribute<GeneratedCodeAttribute>();
                return attribute?.Tool == "CrmSvcUtil";
            }

            private static string GetEntityLogicalName(Type entityType)
            {
                var nameAttribute = entityType.GetCustomAttribute<EntityLogicalNameAttribute>();
                return nameAttribute?.LogicalName;
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

        private static class EntityCache<T> where T : Entity
        {
            internal static readonly IEntityInfo Info;

            static EntityCache()
            {
                Info = EntityCache.GetOrAddInfo(typeof(T));
                if (Info == null)
                    throw new InvalidOperationException("The type doesn't have EntityLogicalNameAttribute.");
            }
        }
    }
}
