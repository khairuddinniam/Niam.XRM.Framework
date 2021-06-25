using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.TestHelper
{
    public static class TestExtensions
    {
        public static void SetRequest(this XrmFakedPluginExecutionContext context, OrganizationRequest request)
        {
            var clone = (OrganizationRequest) Activator.CreateInstance(request.GetType());
            var properties = request.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute<DataMemberAttribute>() == null)
                .Where(p => !p.GetIndexParameters().Any())
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var propertyInfo in properties)
                propertyInfo.SetValue(clone, propertyInfo.GetValue(request));

            context.MessageName = clone.RequestName;
            foreach (var parameter in clone.Parameters)
                context.InputParameters[parameter.Key] = parameter.Value;
        }
    }
}
