using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using System.Diagnostics;

namespace Niam.XRM.Framework.Plugin
{
    /// <summary>
    /// Base class for all plug-in classes.
    /// </summary>    
    public abstract class PluginCoreBase : IPluginBase
    {
        private readonly Type _pluginType;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "PluginBase")]
        public string UnsecureConfig { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "PluginBase")]
        public string SecureConfig { get; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "PluginBase")]
        protected PluginCoreBase(string unsecure = null, string secure = null)
        {
            _pluginType = GetType();
            UnsecureConfig = unsecure;
            SecureConfig = secure;
        }

        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances. 
        /// The plug-in's Execute method should be written to be stateless as the constructor 
        /// is not called for every invocation of the plug-in. Also, multiple system threads 
        /// could execute the plug-in at the same time. All per invocation state information 
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Execute")]
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            // Construct the local plug-in context.
            var stopwatch = Stopwatch.StartNew();
            var tracingService = serviceProvider.GetService<ITracingService>();        
            tracingService.Trace("Entered {0}.Execute()", _pluginType);

            try
            {
                // Invoke the custom implementation 
                ExecuteCrmPlugin(serviceProvider);
                // now exit - if the derived plug-in has incorrectly registered overlapping event registrations,
                // guard against multiple executions.
                return;
            }
            catch (Exception e)
            {
                tracingService.Trace("Exception: {0}", e);
                tracingService.Trace("Stack Trace: {0}", e.StackTrace);
                throw new InvalidPluginExecutionException(e.Message, e);
            }
            finally
            {
                stopwatch.Stop();
                tracingService.Trace("Exiting {0}.Execute() [{1:g}]", _pluginType, stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Placeholder for a custom plug-in implementation. 
        /// </summary>
        /// <param name="serviceProvider">Context for the current plug-in.</param>
        protected abstract void ExecuteCrmPlugin(IServiceProvider serviceProvider);
    }
}