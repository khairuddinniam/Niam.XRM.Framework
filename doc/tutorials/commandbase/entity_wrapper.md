## **EntityWrapper**

EntityWrapper is an object that holds main target entity for the plugin step. The EntityWrapper properties is combination between _**Context.PluginExecutionContext.InputParameters["Target"]**_ and properties that retrieved from database/Entity Pre-Image. All of the changes from screen and plugin will update this object and of course you can rely on this object.

## Get Method

Get is a method that helps you to get value from this object.

### Late Bound Get

You must give a return value for when using late bound Get method. For example:

``` csharp
var parentRef = Get<EntityReference>("new_parentid");
var amount = Get<Money>("new_amount");
var transactionDate = Get<DateTime>("new_transactiondate");
```

### Early Bound Get

In early bound Get method, you must define the attribute that you need to get using Lambda Expression.

``` csharp
EntityReference parentRef = Get(e => e.new_parentid);
Money amount = Get(e => e.new_amount);
DateTime? transactionDate = Get(e => e.new_transactiondate);
```
## Set Method

Set is a method that helps you to change value from an attribute. (**Only works in Pre-Operation stage. For Post-Operation, you must update manually using Service.Update method**).

### Late Bound Set

In late bound Set method, you must define the string attribute name and the value.

``` csharp
Set("new_name", "Hello world");
Set("new_transactiondate", DateTime.UtcNow);
Set("new_amount", new Money(1000m));
Set("new_parentid", new EntityReference("new_entity1", parentRef.Id));
```

### Early Bound Set

In early bound Set method, you must define the attribute using lambda expression and the value.

``` csharp
Set(e => e.new_name, "hello world");
Set(e => e.new_transactiondate, DateTime.UtcNow);
Set(e => e.new_amount, new Money(1000m));
Set(e => e.new_parentid, new EntityReference(Entities.new_entity1.EntityLogicalName, parentId));
```

## GetRelated Method
GetRelated is a method that helps you to get related entity data (attribute with type EntityReference).

### Late Bound GetRelated

You must define the attribute name (string) and the ColumnSet that will be retrieved:

``` csharp
var parentWrapper = GetRelated("new_parentid", new ColumnSet("new_name", "new_amount"));
var parentName = parentWrapper.Get<string>("new_name");
var parentAmount = parentWrapper.Get<Money>("new_amount");
```

### Early Bound GetRelated

You must define the attribute using lambda expression and the ColumnSet that will be retrieved:

``` csharp
var columnSet = new ColumnSet<Entities.new_entity1>(e => e.new_name, e=>e.new_amount);
var parentWrapper = Wrapper.GetRelated(e => e.new_parentid, columnSet);
string parentName = parentWrapper.Get(e => e.new_name);
Money parentAmount = parentWrapper.Get(e => e.new_amount);
```