using System.Collections.Generic;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;
using Niam.XRM.Framework.Plugin.Actions;

namespace Niam.XRM.Framework.Plugin.Configurations
{
    internal static class DefaultConfig
    {
        public static IList<IInputAction> InputActions { get; } = new List<IInputAction>
        {
            new UpdateFormattedValueEventInputAction()
        };

        public static IList<IReferenceAction> ReferenceActions { get; } = new List<IReferenceAction>
        {
            new CopyToReferenceAction(),
            new CopyValueEventReferenceAction()
        };
    }
}