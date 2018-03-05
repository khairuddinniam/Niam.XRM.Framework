using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;
using System.Linq;

namespace Niam.XRM.Framework.Plugin.Actions
{
    public class CopyToReferenceAction : IReferenceAction
    {
        public bool CanExecute(IReferenceActionContext context) => true;

        public void Execute(IReferenceActionContext context)
        {
            var pluginContext = context.TransactionContext.PluginExecutionContext;
            var input = context.Input;
            var reference = context.Reference;
            if (pluginContext.Stage != (int) SdkMessageProcessingStepStage.Postoperation)
            {
                CopyAttributes(input.Attributes, reference.Attributes);
                CopyFormattedValues(input.Entity.FormattedValues, reference.Entity.FormattedValues);
            }
        }

        private static void CopyAttributes(AttributeCollection source, AttributeCollection target)
        {
            foreach (var copy in source.Select(InternalHelper.Copy))
                target[copy.Key] = copy.Value;
        }

        private static void CopyFormattedValues(FormattedValueCollection source, FormattedValueCollection target)
        {
            foreach (var pair in source)
                target[pair.Key] = pair.Value;
        }
    }
}
