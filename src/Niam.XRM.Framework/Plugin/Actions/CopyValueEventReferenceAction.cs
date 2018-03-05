using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;

namespace Niam.XRM.Framework.Plugin.Actions
{
    public class CopyValueEventReferenceAction : IReferenceAction
    {
        public bool CanExecute(IReferenceActionContext context) => true;

        public void Execute(IReferenceActionContext context)
        {
            var input = context.Input;
            var reference = context.Reference;
            input.AttributeChanged += (entity, e) =>
            {
                var source = (Entity) entity;
                reference[e.AttributeName] = source[e.AttributeName];
                var sourceFormattedValue = source.GetFormattedValue(e.AttributeName);
                reference.SetFormattedValue(e.AttributeName, sourceFormattedValue);
            };
        }
    }
}
