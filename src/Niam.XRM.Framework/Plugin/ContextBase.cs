using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;

namespace Niam.XRM.Framework.Plugin
{
    internal abstract class ContextBase : IContextBase
    {
        private readonly Lazy<ITracingService> _tracingService;
        private readonly Lazy<IPluginExecutionContext> _pluginExecutionContext;

        public IServiceProvider ServiceProvider { get; }

        public ITracingService TracingService => _tracingService.Value;

        public IPluginExecutionContext PluginExecutionContext => _pluginExecutionContext.Value;

        public IDictionary<string, object> SessionStorage { get; } = new Dictionary<string, object>();

        protected ContextBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _tracingService = new Lazy<ITracingService>(() => ServiceProvider.GetService<ITracingService>());
            _pluginExecutionContext = new Lazy<IPluginExecutionContext>(() => ServiceProvider.GetService<IPluginExecutionContext>());
        }
    }
}
