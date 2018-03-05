## **Initial** property

From OperationBase class, Initial is pure object from database that has not been edit by user. So when you want to do comparison between data from database and data from screen. You can use this object.

## Late bound Get Method

This is an example of usage for Initial object:

``` csharp
var oldAmount = Initial.Get<Money>("new_amount");
var newAmount = Get<Money>("new_amount");
if (oldAmount.Value != newAmount.Value)
{
	throw new InvalidPluginExecutionException("Amount is not same");
}
```

## Early bound Get Method

For Get method in early bound, you must define the attribute that you want to use using lambda expression:

``` csharp
var oldAmount = Initial.Get(e => e.new_amount);
var newAmount = Get(e => e.new_amount);
if (oldAmount.Value != newAmount.Value)
{
	throw new InvalidPluginExecutionException("Amount is not same");
}
```