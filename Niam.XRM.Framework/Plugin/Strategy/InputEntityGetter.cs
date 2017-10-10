using System.Collections.Generic;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Niam.XRM.Framework.Plugin.Strategy
{
    internal static class InputEntityGetter
    {
        private static readonly SetState SetStateHandler = new SetState();
        private static readonly IDictionary<string, InputEntityGetterBase> Handlers = new Dictionary<string, InputEntityGetterBase>
        {
            [PluginMessage.Create] = new Create(),
            [PluginMessage.Update] = new Update(),
            [PluginMessage.Delete] = new Delete(),
            [PluginMessage.Assign] = new Assign(),
            [PluginMessage.SetState] = SetStateHandler,
            [PluginMessage.SetStateDynamicEntity] = SetStateHandler
        };
        
        public static InputEntityGetterBase GetHandler(string message)
        {
            InputEntityGetterBase handler;
            if (Handlers.TryGetValue(message, out handler))
                return handler;

            throw new InvalidPluginExecutionException($"Message '{message}' doesn't have {nameof(InputEntityGetterBase)} handler.");
        }

        private class Create : InputEntityGetterBase
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.GetRequest<CreateRequest>().Target;
        }

        private class Update : InputEntityGetterBase
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.GetRequest<UpdateRequest>().Target;
        }

        private class Delete : InputEntityGetterBase
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.GetRequest<DeleteRequest>().Target.ToEntity();
        }

        private class Assign : InputEntityGetterBase
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.GetRequest<AssignRequest>().Target.ToEntity();
        }

        private class SetState : InputEntityGetterBase
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.GetRequest<SetStateRequest>().EntityMoniker.ToEntity();
        }
    }
}