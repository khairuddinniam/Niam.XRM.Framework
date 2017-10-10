using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Interfaces
{
    public interface IEntitySetter<out T>
        where T : Entity
    {
        void Set(string attributeName, object value);

        // Need to implement Set overload method with paramater IAttributeValueProvider or IValueProvider here, 
        // not as extension methods, because compiler prioritize void Set(string attributeName, object value); method.
        void Set(string attributeName, IAttributeValueProvider attributeValueProvider);
        void Set(string attributeName, IValueProvider valueProvider);
        void SetFormattedValue(string attributeName, string formattedValue);
    }
}
