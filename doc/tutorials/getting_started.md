#Tutorial: Simple Plugin With Niam.Xrm.Framework
This tutorial covers how you create simple Plugin with Test Driven Development.

This framework works to help you create plugin with ease. Our purpose is create a framework that wrap Dynamic CRM technology without creating a big gap on it. So our function is proven to be inline with Dynamic CRM Api.

The tutorial will split into 2 types:
*Late Bound
*Early Bound

##What You Will Learn
In this tutorial, we will show you how to making plugins using the framework. We try our best to give common scenario that you mostly will use in real life. With this sample, we hope you can get to know our framework and use it in your environment.

###Plugin Anatomy
*Plugin: a class that inherit BasePlugin and IPlugin.
*Business/Operation: a class that inherit OperationBase class. This class represent business logic.

##Late Bound Plugin

###Create Plugin Project
On your Dynamic 365 Plugin Project, prepare all your project first. Insall Niam.Xrm.Framework nuget package, sign your project.

####Business: LoadProductData

Now we creating our first business which will applied base price and name for Product choosen. Right click on your project, add new class and name it *LoadProductData*.

```csharp
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Xrm.Example.LateBound.Business.SalesOrderLine
{
    public class LoadProductData : OperationBase
    {
        public LoadProductData(ITransactionContext<Entity> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var productColumnSet = new ColumnSet("name", "price");
            var product = GetRelated("ins_productid", productColumnSet);

            Set("ins_linenumber", product.Get<string>("name"));
            Set("ins_priceamount", product.Get<Money>("price"));
        }
    }
}
```
Our business class derived from OperationBase class. So when you create business class, you only need to implement this class.

In Sales Order Line, we have a lookup to product. The logic is very straight foward. We need to get the product, and get the base price into *ins_priceamount*. So when you want to get related data (lookup), you can query related data using *GetRelated* function:

```csharp
var productColumnSet = new ColumnSet("name", "price");
var product = GetRelated("ins_productid", productColumnSet);
```
You can config your column set, weather you want to get all the data or define specific attributes.

```csharp
Set("ins_linenumber", product.Get<string>("name"));
Set("ins_priceamount", product.Get<Money>("price"));
```
For setting data into *Context.InputParameter["Target"]*, you can use *Set* function with parameter are attribute name and the value. In this code, you can see how we get the value. *product.Get<string>("name")* will return value for name of product that we select. Same with *product.Get<Money>*("price")* will return price of the product.

####Business: CalculateTotalPrice
Create new class with the name *CalculateTotalPrice*. This class will calculate the total price with simple formula: price multiply with quantity.

```csharp
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Xrm.Example.LateBound.Business.SalesOrderLine
{
    public class CalculateTotalPrice : OperationBase
    {
        public CalculateTotalPrice(ITransactionContext<Entity> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var totalPrice = Get<Money>("ins_priceamount").GetValueOrDefault() * 
                             Get<int?>("ins_qty").GetValueOrDefault();
            Set("ins_totalprice", new Money(totalPrice));
        }
    }
}
```
The logic is still the same compare to the *LoadProductData*, we get the data using *Get<DataType>* function and then we can calulate and store the calculation into the variable. Then set it into total price.

####Business: UpdateSalesOrder
The last business is *UpdateSalesOrder* class. This class responsible to Sum the total price and then update it into *SalesOder.TotalPrice*.

```csharp
using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Xrm.Example.LateBound.Business.SalesOrderLine
{
    public class UpdateSalesOrder : OperationBase
    {
        public UpdateSalesOrder(ITransactionContext<Entity> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var salesOrderRef = Get<EntityReference>("ins_salesorderid");
            if (salesOrderRef == null) return;

            var lineTotalPriceSum = GetLineTotalPriceSum(salesOrderRef.Id);
            var salesOrder = new Entity("ins_salesorder")
            {
                Id = salesOrderRef.Id
            };
            salesOrder.Set("ins_totalprice", new Money(lineTotalPriceSum));
            Service.Update(salesOrder);
        }

        private decimal GetLineTotalPriceSum(Guid salesOrderId)
        {
            var excludeSelfCondition = Context.PluginExecutionContext.MessageName == "Delete"
                ? $"<condition attribute='ins_salesorderlineid' operator='ne' value='{Id}' />"
                : "";

            var fetchXml = String.Join("",
                "<fetch mapping='logical' aggregate='true'>",
                    "<entity name='ins_salesorderline'>",
                        "<attribute name='ins_totalprice' aggregate='sum' alias='sum_totalprice' />",
                        "<filter type='and'>",
                            $"<condition attribute='ins_salesorderid' operator='eq' value='{salesOrderId}' />",
                            excludeSelfCondition,
                        "</filter>",
                    "</entity>",
                "</fetch>"
            );

            var result = Service.RetrieveMultiple(fetchXml).Entities.FirstOrDefault();
            if (result == null) return 0m;

            return result.GetAliasedValue<Money>("sum_totalprice").GetValueOrDefault();
        }
    }
}
```

The new feature you learn here is on the method *GetLineTotalPriceSum*. We prepare the fetchXml to get the totalprice, then we get the result using *Service.RetrieeMultiple* function. This function return *EntityCollection* data.

####Plugin: PreSalesOrderLine
After we defined all the business, then now we can create the plugin. This class will implement *IPlugin* so CRM will know that in our assembly, there is Plugin when you want to register the plugin.


```csharp
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.Framework.Plugin;
using Xrm.Example.LateBound.Business.SalesOrderLine;

namespace Xrm.Example.LateBound.Plugins
{
    // Registered on message: Create, Update, Delete
    public class PreSalesOrderLine : PluginBase, IPlugin
    {
        public PreSalesOrderLine(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        public static void ConfigurePlugin(IPluginConfiguration<Entity> config)
        {
            config.ColumnSet = new ColumnSet(
                "ins_salesorderid",
                "ins_productid",
                "ins_priceamount",
                "ins_qty"
            );
        }

        protected override void Configure(IPluginConfiguration<Entity> config)
        {
            base.Configure(config);
            ConfigurePlugin(config);
        }

        protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
        {
            var messageName = context.PluginExecutionContext.MessageName;
            if (messageName == "Create" || messageName == "Update")
            {
                var target = context.Target;
                if (target.ContainsAny("ins_productid"))
                {
                    new LoadProductData(context).Execute();
                }

                if (target.ContainsAny("ins_priceamount", "ins_qty"))
                {
                    new CalculateTotalPrice(context).Execute();
                }
            }

            if (messageName == "Delete")
            {
                new UpdateSalesOrder(context).Execute();
            }
        }
    }
}
```
For Plugin class, we need to implement PluginBase. In this class we need to overide *Configure* method. Configure method will set the ColumnSet when you call *Get* method in a business. Our framework in behind will automatically *Retrieve* the data needed if you are not set *PreImage* or *PostImage*. So for avoid select all the *ColumnSet*, we need you to configure this.

```csharp
public static void ConfigurePlugin(IPluginConfiguration<Entity> config)
{
    config.ColumnSet = new ColumnSet(
        "ins_salesorderid",
        "ins_productid",
        "ins_priceamount",
        "ins_qty"
    );
}
```

In the code above, we set the *ColumnSet* to select *ins_salesorderid, ins_productid, ins_priceamount, and ins_qty*.

Then in method *ExecuteCrmPlugin*, we call our business.