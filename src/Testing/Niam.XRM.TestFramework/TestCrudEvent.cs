using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Niam.XRM.TestFramework
{
    public class TestCrudEvent : TestCrudEvent<Entity>
    {
    }

    public class TestCrudEvent<TE> : TestEventBase<TE>
        where TE : Entity, new()
    {
        public TE Target { get; set; }

        public TE Initial { get; set; }

        public TestCrudEvent()
        {
            PluginExecutionContext.MessageName = "Update";
            PluginExecutionContext.Stage = 20;

            var id = Guid.NewGuid();
            Target = new TE { Id = id, LogicalName = "target" };
            Initial = new TE { Id = id, LogicalName = "target" };
        }

        protected override void Apply()
        {
            Validate();
            switch (PluginExecutionContext.MessageName)
            {
                case "Create":
                    SetRequest(new CreateRequest { Target = Target });
                    break;

                case "Update":
                    SetRequest(new UpdateRequest { Target = Target });
                    break;

                case "Delete":
                    SetRequest(new DeleteRequest { Target = Target.ToEntityReference() });
                    break;
            }

            var entities = Initial != null ? Db.Concat(new[] { Initial }) : Db;
            FakedContext.Initialize(entities);
        }

        private void Validate()
        {
            if (Target == null)
                throw new TestException("Target property must not null.");

            if ((PluginExecutionContext.MessageName == "Create" && PluginExecutionContext.Stage == 40) ||
                (PluginExecutionContext.MessageName == "Update" && PluginExecutionContext.Stage == 10) ||
                (PluginExecutionContext.MessageName == "Update" && PluginExecutionContext.Stage == 20) ||
                (PluginExecutionContext.MessageName == "Update" && PluginExecutionContext.Stage == 40) ||
                (PluginExecutionContext.MessageName == "Delete" && PluginExecutionContext.Stage == 10) ||
                (PluginExecutionContext.MessageName == "Delete" && PluginExecutionContext.Stage == 20))
            {
                if (Initial == null)
                    throw new TestException("Initial property must not null.");

                if (!Target.ToEntityReference().Equals(Initial.ToEntityReference()))
                    throw new TestException("Target entity name and id is not same as Initial entity name and id.");
            }
        }

        private void SetRequest(OrganizationRequest request)
        {
            foreach (var parameter in request.Parameters)
                PluginExecutionContext.InputParameters[parameter.Key] = parameter.Value;
        }
    }
}
