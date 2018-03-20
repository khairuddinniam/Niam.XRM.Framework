using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;

namespace Niam.XRM.Framework.Plugin.Actions
{
    public class CopyValueEventCurrentAction : ICurrentAction
    {
        public bool CanExecute(ICurrentActionContext context) => true;

        public void Execute(ICurrentActionContext context)
        {
            var target = context.Target;
            var current = context.Current;
            target.AttributeChanged += (entity, e) =>
            {
                var source = (Entity) entity;
                current[e.AttributeName] = source[e.AttributeName];
                var sourceFormattedValue = source.GetFormattedValue(e.AttributeName);
                current.SetFormattedValue(e.AttributeName, sourceFormattedValue);
            };

            target.FormattedValueChanged += (entity, e) =>
            {
                var source = (Entity) entity;
                var sourceFormattedValue = source.GetFormattedValue(e.AttributeName);
                current.SetFormattedValue(e.AttributeName, sourceFormattedValue);
            };
        }
    }
}
