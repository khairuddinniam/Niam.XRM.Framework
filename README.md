# Niam.XRM.Framework
Fast, easy and lightweight Microsoft Dynamics CRM Plugin Framework that support premise and online edition. One framework to rule them all.

## Supported Version

- Microsoft Dynamic CRM 2015
- Microsoft Dynamic CRM 2016
- Microsoft Dynamic CRM 365

## Installation

Using nuget package manager:

```
PM> Install-Package Niam.XRM.Framework
```

Please see the [Niam.Xrm.Framework nuget packages](https://www.nuget.org/packages/Niam.XRM.Framework) to see the version of supported [Microsoft.Crm.Sdk.CoreAssemblies](https://www.nuget.org/packages/Microsoft.CrmSdk.CoreAssemblies). This framework version number will follow the version number of Microsoft.Crm.Sdk.CoreAssemblies.

## Getting Started

This framework support late bound and early bound entities. For creating plugin we define into 3 parts:

1. ColumnSetConfig Class
1. Operation Class (Business related)
1. Plugin Class

### ColumnSetConfig Class

Configuration for entity target ColumnSet (On Update and Delete Operation).

``` csharp
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;

namespace XrmTutorial
{
    public static class ColumnSetConfig
    {
        public static void Configure(ITransactionContextConfiguration<Entity> config)
        {
            var columnSet = new ColumnSet(
                "new_name",
                "new_parentid",
                "new_transactiondate",
                "new_amount");
            config.ColumnSet = columnSet;
        }
    }
}
```

### Operation Class

Operation / Business class for your plugin.

``` csharp
using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace XrmTutorial
{
    public class SetDefaultValue : OperationBase
    {
        public SetDefaultValue(ITransactionContext<Entity> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var parentRef = Get<EntityReference>("new_parentid");
            var parentName = GetParentName(parentRef);
            Set("new_name", parentName + Get(e=>e.Id));
            Set("new_transactiondate", DateTime.UtcNow);
            Set("new_amount", new Money(1000m));
        }

        private string GetParentName(EntityReference parentRef)
        {
            if (parentRef == null) return "";
            var parent = Service.Retrieve("new_entity1", parentRef.Id, new ColumnSet("new_name"));
            return parent.GetAttributeValue<string>("new_name");
        }
    }
}
```

### Plugin Class

Plugin class for calling the ColumnSetConfig and Operation.

``` csharp
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;

namespace XrmTutorial
{
    public class PreOperationEntity1 : PluginBase, IPlugin
    {
        public PreOperationEntity1(string unsecure, string secure) : 
            base(unsecure, secure)
        {
        }

        protected override void Configure(IPluginConfiguration<Entity> config)
        {
            ColumnSetConfig.Configure(config.TransactionContext);
        }

        protected override void ExecuteCrmPlugin(ITransactionContext<Entity> context)
        {
            if (context.Input.ContainsAny("new_name"))
            {
                new SetDefaultValue(context).Execute();
            }
        }
    }
}
```