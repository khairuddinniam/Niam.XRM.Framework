## **Common Functionality**

There are lot of common functionality that you can use inside OperationBase:

## GetValue Method

Get value with default value in Early bound operation. You must declare the attribute that you will Get, and the second parameter is the default value if null (optional).

``` csharp
var amount = GetValue(e => e.new_amount); // by default is 0
var amountWithOptional = GetValue(e => e.new_amount, 1000m); // if null, default is 1000m
var transactionDate = GetValue(e => e.new_transactiondate); //by default is min date
var transactionDateWithOptional = GetValue(e => e.new_transactiondate, DateTime.UtcNow); //by default is now
```

## String.ToJson Method
If you need to convert Json format string, you can use this function.

``` csharp
var dataRequest = new DataRequest
{
	Url = "localhost",
	Username = "admin",
	Password = "password-123"
};

var jsonString = dataRequest.ToJson();
```

This is how you defined the class

``` csharp
public class DataRequest
{
	public string Url { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }
}
```

## FromJson<> Method
If you need to convert from string back to Json data, you only need to pass the value type like this:

``` csharp
var requestObj = stringJson.FromJson<DataRequest>();
```