using System.Collections.Generic;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Niam.XRM.Framework.Plugin.Strategy
{
    internal abstract class TargetEntityGetter
    {
        private static readonly SetState SetStateHandler = new SetState();
        private static readonly IDictionary<string, TargetEntityGetter> Handlers = new Dictionary<string, TargetEntityGetter>
        {
            [PluginMessage.Create] = new Create(),
            [PluginMessage.Update] = new Update(),
            [PluginMessage.Delete] = new Delete(),
            [PluginMessage.Assign] = new Assign(),
            [PluginMessage.SetState] = SetStateHandler,
            [PluginMessage.SetStateDynamicEntity] = SetStateHandler
        };
        
        public static TargetEntityGetter GetHandler(string message)
        {
            if (Handlers.TryGetValue(message, out var handler))
                return handler;

            throw new InvalidPluginExecutionException($"Message '{message}' doesn't have {nameof(TargetEntityGetter)} handler.");
        }

        public abstract Entity Get(IPluginExecutionContext context);

        private class Create : TargetEntityGetter
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.GetRequest<CreateRequest>().Target;
        }

        private class Update : TargetEntityGetter
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.GetRequest<UpdateRequest>().Target;
        }

        private class Delete : TargetEntityGetter
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.GetRequest<DeleteRequest>().Target.ToEntity();
        }

        private class Assign : TargetEntityGetter
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.GetRequest<AssignRequest>().Target.ToEntity();
        }

        private class SetState : TargetEntityGetter
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.GetRequest<SetStateRequest>().EntityMoniker.ToEntity();
        }
    }
}