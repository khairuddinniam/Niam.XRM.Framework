using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;

namespace Niam.XRM.TestFramework
{
    public class TestPlugin<TE> where TE : Entity
    {
        public string UnsecureConfig { get; set; }

        public string SecureConfig { get; set; }

        public Action<IPluginConfiguration<TE>> Configure { get; set; }
    }
}
