## Installation

The version for this nuget package follow the version number of [Microsoft.Crm.Sdk.CoreAssemblies](https://www.nuget.org/packages/Microsoft.CrmSdk.CoreAssemblies).

```
PM> Install-Package Niam.XRM.TestFramework
```

## Getting Started

Simple tutorial how to do Test Driven Development in Microsoft Dynamic CRM Plugin.

Reference will hold the object for InputParameters["Target"] both on late bound or early bound development.

### Late bound example

``` csharp
using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.TestFramework;
using Xunit;

namespace XrmTutorial.Test
{
    public class SetDefaultValueTest : TestBase
    {
        protected override void Configure(IPluginConfiguration<Entity> config)
        {
            ColumnSetConfig.Configure(config.TransactionContext);
        }

        [Fact]
        public void OnCreate_ShouldSetDefaultValue()
        {
            var entity = new Entity("new_entity1") {Id = Guid.NewGuid()};
            Reference = entity;

            new SetDefaultValue(Context).Execute();

            Assert.Equal("hello world", Context.Input.Get<string>("new_name"));
            Assert.Equal(DateTime.UtcNow.Date, Context.Input.Get<DateTime?>("new_transactiondate").Value.Date);
            Assert.Equal(1000m, Context.Input.Get<Money>("new_amount").Value);
        }
    }
}
```

### Early bound example

``` csharp
using System;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin.Configurations;
using Niam.XRM.TestFramework;
using Xunit;

namespace XrmTutorial.Test
{
    public class SetDefaultValueEarlyBoundTest : TestBase<Entities.new_entity1>
    {
        protected override void Configure(IPluginConfiguration<Entities.new_entity1> config)
        {
            var columnSet = new ColumnSet<Entities.new_entity1>(
                e => e.new_amount,
                e => e.new_name,
                e => e.new_transactiondate);

            config.TransactionContext.ColumnSet = columnSet;
        }

        [Fact]
        public void OnCreate_ShouldSetDefaultValueEarlyBound()
        {
            var entity = new Entities.new_entity1 {Id = Guid.NewGuid()};
            Reference = entity;

            new SetDefaultValueEarlyBound(Context).Execute();

            Assert.Equal("from early bound", Context.Input.Get(e=>e.new_name));
            Assert.Equal(DateTime.UtcNow.Date, Context.Input.Get(e => e.new_transactiondate).Value.Date);
            Assert.Equal(1000m, Context.Input.Get(e => e.new_amount).Value);
        }
    }
}
```

## In Memory Database

Powerful implementation of In Memory Database. You can test your retrieved query using this framework.

We support this two class for retrieve data by default:
1. [QueryByAttribute](https://msdn.microsoft.com/en-us/library/microsoft.xrm.sdk.query.querybyattribute.aspx)
1. [QueryExpression](https://msdn.microsoft.com/en-us/library/microsoft.xrm.sdk.query.queryexpression.aspx)

Example
``` csharp
public class UpdateRelatedValueTest : TestBase
{
	protected override void Configure(IPluginConfiguration<Entity> config)
	{
		ColumnSetConfig.Configure(config.TransactionContext);
	}

	[Fact]
	public void OnCreate_UpdateEntityWithSameName()
	{
		var entity = new Entity("new_entity1") { Id = Guid.NewGuid() };
		entity.Set("new_name", "0001");
		entity.Set("new_amount", new Money(500m));

		Reference = entity;

		var childEntity = new Entity("new_entity1") { Id = Guid.NewGuid()};
		childEntity.Set("new_name", "0001");
		childEntity.Set("new_amount", new Money(200m));
		Db["Child1"] = childEntity;

		var childEntity2 = new Entity("new_entity1") { Id = Guid.NewGuid() };
		childEntity2.Set("new_name", "0001");
		childEntity2.Set("new_amount", new Money(100m));
		Db["Child2"] = childEntity2;

		new UpdateRelatedValue(Context).Execute();

		var updatedChild1 = Test.UpdatedEntities[0];
		Assert.Equal(childEntity.Id, updatedChild1.Id);
		Assert.Equal(500m, updatedChild1.Get<Money>("new_amount").Value);

		var updatedChild2 = Test.UpdatedEntities[1];
		Assert.Equal(childEntity2.Id, updatedChild2.Id);
		Assert.Equal(500m, updatedChild2.Get<Money>("new_amount").Value);
	}
}

public class UpdateRelatedValue : OperationBase
{
	public UpdateRelatedValue(ITransactionContext<Entity> context) : base(context)
	{
	}

	protected override void HandleExecute()
	{
		var data = GetRelated();
		var amount = Get<Money>("new_amount");
		foreach (var entity in data)
		{
			var updated = new Entity("new_entity1") {Id = entity.Id};
			updated.Set("new_amount", amount);

			Service.Update(updated);
		}
	}

	private Entity[] GetRelated()
	{
		var name = Get<string>("new_name");
		var query = new QueryByAttribute("new_entity1")
		{
			ColumnSet = new ColumnSet(true)
		};
		query.AddAttributeValue("new_name", name);

		var result = Service.RetrieveMultiple(query);
		return result.Entities.Any() ? result.Entities.ToArray() : new Entity[] { };
	}
}
```

### Mocking Request
You also can substitute others request when you want it using [NSubstitute](https://github.com/nsubstitute/NSubstitute) function. For example:

``` csharp
var response = Substitute.For<WhoAmIResponse>();
response.BusinessUnitId.Returns(Guid.NewGuid());
response.OrganizationId.Returns(Guid.NewGuid());
response.UserId.Returns(Guid.NewGuid());

Context.Service.Execute(Arg.Any<WhoAmIRequest>()).Returns(response);
```