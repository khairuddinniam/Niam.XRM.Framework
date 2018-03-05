using System.Reflection;

namespace Niam.XRM.Framework.Interfaces
{
    public interface IEntityInfo
    {
        bool IsCrmSvcUtilGenerated { get; }
        string LogicalName { get; }
        string PrimaryNameAttribute { get; }
        int? StateCodeActiveValue { get; }
        string GetAttributeName(string memberName);
        string GetMemberName(string attributeName);
        PropertyInfo GetPropertyInfo(string propertyName);
    }
}