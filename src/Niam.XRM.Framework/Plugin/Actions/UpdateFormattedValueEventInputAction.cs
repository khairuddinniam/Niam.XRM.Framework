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

        private static void RemoveFormattedValue(object entity, AttributeChangingEventArgs e)
        {
            var source = (Entity) entity;
            source.FormattedValues.Remove(e.AttributeName);
        }

        private static void UpdateFormattedValue(object entity, AttributeChangedEventArgs e)
        {
            var source = (Entity) entity;
            var value = source[e.AttributeName];
            if (value is EntityReference reference && reference.Name != null)
                source.FormattedValues[e.AttributeName] = reference.Name;
        }
    }
}
