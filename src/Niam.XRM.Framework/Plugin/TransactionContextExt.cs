using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin
{
    public static class TransactionContextExt
    {
        public static InvalidPluginExecutionException ErrorMessage(this IContextBase context, string message,
            params object[] args)
        {
            var errorMessage = String.Format(message, args);
            return new InvalidPluginExecutionException(errorMessage);
        }

        public static void Trace(this IContextBase context, string format, params object[] args)
            => context.TracingService.Trace(format, args);
    }
}
