using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;
using System.Linq;

namespace Niam.XRM.Framework.Plugin.Actions
{
    public class CopyToCurrentAction : ICurrentAction
    {
        public bool CanExecute(ICurrentActionContext context) => true;

        public void Execute(ICurrentActionContext context)
        {
            var pluginContext = context.TransactionContext.PluginExecutionContext;
            var target = context.Target;
            var current = context.Current;
            if (pluginContext.Stage != (int) SdkMessageProcessingStepStage.Postoperation)
            {
                CopyAttributes(target.Attributes, current.Attributes);
                CopyFormattedValues(target.Entity.FormattedValues, current.Entity.FormattedValues);
            }
        }

        private static void CopyAttributes(AttributeCollection fromCollection, AttributeCollection toCollection)
        {
            foreach (var copy in fromCollection.Select(InternalHelper.Copy))
                toCollection[copy.Key] = copy.Value;
        }

        private static void CopyFormattedValues(FormattedValueCollection fromCollection, FormattedValueCollection toCollection)
        {
            foreach (var pair in fromCollection)
                toCollection[pair.Key] = pair.Value;
        }
    }
}
