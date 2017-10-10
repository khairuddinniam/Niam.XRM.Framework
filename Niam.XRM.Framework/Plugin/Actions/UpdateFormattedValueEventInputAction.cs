using System.ComponentModel;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;

namespace Niam.XRM.Framework.Plugin.Actions
{
    public class UpdateFormattedValueEventInputAction : IInputAction
    {
        public bool CanExecute(IInputActionContext context) => true;

        public void Execute(IInputActionContext context)
        {
            context.Input.AttributeChanging += RemoveFormattedValue;
            context.Input.AttributeChanged += UpdateFormattedValue;
        }

        private static void RemoveFormattedValue(object entity, PropertyChangingEventArgs e)
        {
            var source = (Entity) entity;
            source.FormattedValues.Remove(e.PropertyName);
        }

        private static void UpdateFormattedValue(object entity, PropertyChangedEventArgs e)
        {
            var source = (Entity) entity;
            var value = source[e.PropertyName];
            if (value is EntityReference reference && reference.Name != null)
                source.FormattedValues[e.PropertyName] = reference.Name;
        }
    }
}
