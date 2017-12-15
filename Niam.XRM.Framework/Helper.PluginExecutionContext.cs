using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework
{
    public static partial class Helper
    {
        public static IPluginExecutionContext GetAncestor(this IPluginExecutionContext context)
        {
            var ancestor = context;
            while (ancestor.ParentContext != null)
                ancestor = ancestor.ParentContext;

            return ancestor;
        }

        public static T GetShared<T>(this IPluginExecutionContext context, string key)
        {
            return context.SharedVariables.TryGetValue(key, out var value)
                ? (T) value
                : default (T);
        }
    }
}
