using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;

namespace Niam.XRM.Framework.TestHelper
{
    public class TestPluginWithCommand<TE, TCommand> : PluginBase<TE>, IPlugin
        where TE : Entity
        where TCommand : ICommand
    {
        public TCommand Command { get; private set; }
        
        public Action<IPluginConfiguration<TE>> ConfigureAction { get; set; }
        
        public TestPluginWithCommand(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override void ExecuteCrmPlugin(IPluginContext<TE> context)
        {
            Command = (TCommand) Activator.CreateInstance(typeof(TCommand), new object[] { context });
            if (Command is IOperation operation)
                operation.Execute();
            else if (Command is IValidation validation)
                validation.Validate();
        }

        protected override void Configure(IPluginConfiguration<TE> config)
        {
            ConfigureAction?.Invoke(config);
        }
    }
}