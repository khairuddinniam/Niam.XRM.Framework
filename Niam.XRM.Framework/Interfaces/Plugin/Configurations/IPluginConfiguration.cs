﻿using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin.ServiceProviders;
using Niam.XRM.Framework.Plugin.Configurations;

namespace Niam.XRM.Framework.Interfaces.Plugin.Configurations
{
    public interface IPluginConfiguration<T> : ITransactionContextConfiguration<T>
        where T : Entity
    {
        IContainer Container { get; }
        IList<Func<IOrganizationService, IServiceProvider, IOrganizationService>> ServiceDecorators { get; }
    }
}
