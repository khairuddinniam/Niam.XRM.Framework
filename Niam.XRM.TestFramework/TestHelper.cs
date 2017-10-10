using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Interfaces.Plugin.Actions;
using Niam.XRM.Framework.Plugin.Actions;
using Niam.XRM.TestFramework.Query;

namespace Niam.XRM.TestFramework
{
    public class TestHelper
    {
        private readonly InMemoryQueryByAttribute _queryByAttribute;
        private readonly InMemoryQueryExpression _queryByExpression;

        public IQueryParameter QueryParameter { get; } = Substitute.For<IQueryParameter>();
        public IDictionary<string, Entity> Db { get; } = new Dictionary<string, Entity>();
        public IList<Entity> CreatedEntities { get; } = new List<Entity>();
        public IList<Entity> UpdatedEntities { get; } = new List<Entity>();
        public IList<EntityReference> DeletedEntities { get; } = new List<EntityReference>();
        public IOrganizationService Service { get; }
        public IPluginExecutionContext PluginExecutionContext { get; }
        public IServiceProvider ServiceProvider { get; }

        public TestHelper()
        {
            _queryByAttribute = new InMemoryQueryByAttribute(Db, QueryParameter);
            _queryByExpression = new InMemoryQueryExpression(Db, QueryParameter);

            Service = CreateOrganizationService();
            PluginExecutionContext = CreatePluginExecutionContext();
            ServiceProvider = CreateServiceProvider();
        }

        private IOrganizationService CreateOrganizationService()
        {
            var service = Substitute.For<IOrganizationService>();
            MockCreateMethod(service);
            MockRetrieveMethod(service);
            MockUpdateMethod(service);
            MockDeleteMethod(service);
            MockRetrieveMultipleMethod(service);
            return service;
        }

        private void MockRetrieveMultipleMethod(IOrganizationService service)
        {
            service.RetrieveMultiple(Arg.Any<QueryBase>())
                .Returns(ci =>
                {
                    var query = ci.ArgAt<QueryBase>(0);
                    if (query is QueryByAttribute qba)
                        return _queryByAttribute.RetrieveMultiple(qba);

                    if (query is QueryExpression qexp)
                        return _queryByExpression.RetrieveMultiple(qexp);

                    return new EntityCollection();
                });
        }

        private void MockCreateMethod(IOrganizationService service)
        {
            service.Create(Arg.Any<Entity>())
                .Returns(ci =>
                {
                    var paramEntity = ci.ArgAt<Entity>(0);
                    if (paramEntity == null)
                        throw new InvalidOperationException("Parameter 'entity' is null.");

                    if (String.IsNullOrWhiteSpace(paramEntity.LogicalName))
                        throw new InvalidOperationException("Parameter 'entity.LogicalName' is null or empty.");

                    var createdEntity = paramEntity.Copy();
                    var id = createdEntity.Id != Guid.Empty ? createdEntity.Id : Guid.NewGuid();
                    createdEntity.Id = id;
                    CreatedEntities.Add(createdEntity);
                    Db[id.ToString()] = createdEntity.Copy();
                    return id;
                });
        }

        private void MockRetrieveMethod(IOrganizationService service)
        {
            service.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(ci =>
                {
                    var entityName = ci.ArgAt<string>(0);
                    if (entityName == null)
                        throw new ArgumentException("entityName must not null.");
                    var id = ci.ArgAt<Guid>(1);
                    var columnSet = ci.ArgAt<ColumnSet>(2);
                    var copy = Db.Values.First(e => e.LogicalName == entityName && e.Id == id).Copy();
                    if (columnSet == null)
                    {
                        copy.Attributes.Clear();
                        return copy;
                    }

                    if (!columnSet.AllColumns)
                    {
                        var toBeAddedList =
                            (from column in columnSet.Columns
                                join attr in copy.Attributes
                                    on column equals attr.Key
                                select attr).ToArray();

                        copy.Attributes.Clear();
                        copy.Attributes.AddRange(toBeAddedList);
                    }

                    return copy;
                });
        }

        private void MockUpdateMethod(IOrganizationService service)
        {
            service.When(svc => svc.Update(Arg.Any<Entity>()))
                .Do(ci =>
                {
                    var paramEntity = ci.ArgAt<Entity>(0);
                    var updatedEntity = paramEntity.Copy();
                    UpdatedEntities.Add(updatedEntity);

                    var dbEntity = Db.Select(pair => pair.Value).FirstOrDefault(e => e.Id == paramEntity.Id);
                    if (dbEntity == null) return;

                    foreach (var attribute in updatedEntity.Attributes)
                        dbEntity[attribute.Key] = attribute.Value;
                });
        }

        private void MockDeleteMethod(IOrganizationService service)
        {
            service.When(svc => svc.Delete(Arg.Any<string>(), Arg.Any<Guid>()))
                .Do(ci =>
                {
                    var logicalName = ci.ArgAt<string>(0);
                    var id = ci.ArgAt<Guid>(1);
                    var reference = new EntityReference(logicalName, id);
                    DeletedEntities.Add(reference);

                    var pair = Db.FirstOrDefault(p => p.Value.ToEntityReference().Equals(reference));
                    if (!pair.Equals(default(KeyValuePair<string, Entity>)))
                        Db.Remove(pair);
                });
        }

        private static IPluginExecutionContext CreatePluginExecutionContext()
        {
            var context = Substitute.For<IPluginExecutionContext>();
            context.MessageName.Returns(PluginMessage.Create);
            context.Stage.Returns((int) SdkMessageProcessingStepStage.Preoperation);
            context.SharedVariables.Returns(new ParameterCollection());
            context.InputParameters.Returns(new ParameterCollection());
            context.PreEntityImages.Returns(new EntityImageCollection());
            context.PostEntityImages.Returns(new EntityImageCollection());
            return context;
        }

        private IServiceProvider CreateServiceProvider()
        {
            var serviceFactory = Substitute.For<IOrganizationServiceFactory>();
            serviceFactory.CreateOrganizationService(Arg.Any<Guid?>()).Returns(Service);

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Is(typeof(IPluginExecutionContext))).Returns(PluginExecutionContext);
            serviceProvider.GetService(Arg.Is(typeof(IOrganizationServiceFactory))).Returns(serviceFactory);

            return serviceProvider;
        }

        public ITransactionContext<TEntity> CreateTransactionContext<TEntity>()
            where TEntity : Entity => 
            CreateTransactionContext<TEntity, ITransactionContext<TEntity>>();

        public TContext CreateTransactionContext<TEntity, TContext>()
            where TEntity : Entity
            where TContext : class, ITransactionContext<TEntity>
        {
            var context = Substitute.For<TContext>();
            context.PluginExecutionContext.Returns(PluginExecutionContext);
            context.Service.Returns(Service);
            context.SystemService.Returns(Service);
            context.SessionStorage.Returns(new Dictionary<string, object>());

            var reference = Activator.CreateInstance<TEntity>();
            reference.Id = Guid.NewGuid();
            var txReference = new TransactionContextEntity<TEntity>(reference);
            context.Reference.Returns(txReference);

            var input = Activator.CreateInstance<TEntity>();
            input.Id = reference.Id;
            var txInput = new TransactionContextEntity<TEntity>(input);
            context.Input.Returns(txInput);

            var inputActions = new IInputAction[]
            {
            };

            var referenceActions = new IReferenceAction[]
            {
                new CopyToReferenceAction(),
                new CopyValueEventReferenceAction()
            };

            var inputActionContext = new InputActionContext
            {
                TransactionContext = context,
                Input = txInput
            };

            foreach (var action in inputActions)
            {
                if (action.CanExecute(inputActionContext))
                    action.Execute(inputActionContext);
            }

            var referenceActionContext = new ReferenceActionContext
            {
                TransactionContext = context,
                Input = txInput,
                Reference = txReference
            };

            foreach (var action in referenceActions)
            {
                if (action.CanExecute(referenceActionContext))
                    action.Execute(referenceActionContext);
            }

            return context;
        }
    }
}
