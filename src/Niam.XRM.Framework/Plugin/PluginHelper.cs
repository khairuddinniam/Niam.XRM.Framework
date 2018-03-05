using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;

namespace Niam.XRM.Framework.Plugin
{
    public static class PluginHelper
    {
        public static ITransactionContext<Entity> ToTransactionContext(this IServiceProvider serviceProvider, ITransactionContextConfiguration<Entity> config = null) =>
            serviceProvider.ToTransactionContext<Entity>(config);

        public static ITransactionContext<T> ToTransactionContext<T>(this IServiceProvider serviceProvider, ITransactionContextConfiguration<T> config = null)
            where T : Entity => new TransactionContext<T>(serviceProvider, config);
    }
}
