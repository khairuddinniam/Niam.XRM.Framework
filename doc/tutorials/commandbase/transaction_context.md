## **TransactionContext**
From OperationBase you can access the Context object. This object have access for this:
1. Initial
1. Input is same with InputParameters["Target"] that holds only changes in this target entity.
1. Reference is merge between Initial data and Input.
1. PluginExecutionContext is an implementation of IPluginExecutionContext in this Plugin Step.
1. Plugin holds SecureConfig and UnsecureConfig for the PluginStep.
1. Service is an implementation of IOrganizationService in this target entity.
1. SystemService is an implementation of IOrganizationService on behalf of Admin.
1. TracingService is an implementation of ITracingService.
1. ServiceFactory is an implementation of IOrganizationServiceFactory.
1. ServiceProvider is an implementation of IServiceProvider.