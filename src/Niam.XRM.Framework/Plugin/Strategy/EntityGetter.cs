using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;

namespace Niam.XRM.Framework.Plugin.Strategy
{
    internal abstract class EntityGetter
    {
        private static readonly Common CommonHandler = new Common();
        private static readonly IDictionary<string, EntityGetter> Handlers = new Dictionary<string, EntityGetter>
        {
            [PluginMessage.Create] = new Create(),
            [PluginMessage.Update] = CommonHandler,
            [PluginMessage.Delete] = CommonHandler,
            [PluginMessage.Assign] = CommonHandler,
            [PluginMessage.SetState] = CommonHandler,
            [PluginMessage.SetStateDynamicEntity] = CommonHandler
        };

        public static EntityGetter GetHandler(string message)
        {
            if (Handlers.TryGetValue(message, out var handler))
                return handler;

            throw new InvalidPluginExecutionException($"Message '{message}' doesn't have {nameof(EntityGetter)} handler.");
        }

        public abstract Entity Get<T>(ITransactionContext<T> context, ITransactionContextConfiguration<T> config)
            where T : Entity;

        private class Create : EntityGetter
        {
            public override Entity Get<T>(ITransactionContext<T> context, ITransactionContextConfiguration<T> config)
            {
                var stage = context.PluginExecutionContext.Stage;
                return stage != (int) SdkMessageProcessingStepStage.Postoperation
                    ? new Entity(context.Input.LogicalName, context.Input.Id)
                    : CommonHandler.Get(context, config);
            }
        }

        private class Common : EntityGetter
        {
            public override Entity Get<T>(ITransactionContext<T> context, ITransactionContextConfiguration<T> config)
                => GetEntityFromImage(context) ?? GetEntityFromDatabase(context, config);

            private Entity GetEntityFromImage(IContextBase context)
            {
                var handler = ImageEntityGetter.GetHandler(context.PluginExecutionContext.Stage);
                return handler.Get(context.PluginExecutionContext);
            }

            private static Entity GetEntityFromDatabase<T>(ITransactionContext<T> context, ITransactionContextConfiguration<T> config)
                where T : Entity
            {
                var logicalName = context.Input.LogicalName;
                var id = context.Input.Id;
                return context.Service.Retrieve(logicalName, id, config.ColumnSet);
            }
        }
    }
}