using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Plugin;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin;

public static class PipelineOrganizationServiceTests
{
    public class CreateTests
    {
        [Fact]
        public void Without_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var expectedId = Guid.NewGuid();
            crmService.Create(Arg.Any<Entity>()).Returns(expectedId);
            var service = new PipelineOrganizationService(crmService);
            var entity = new Entity();
            var id = service.Create(entity);
                
            crmService.Received(1).Create(Arg.Is(entity));
            id.ShouldBe(expectedId);
        }

        [Fact]
        public void With_a_pipeline()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var expectedId = Guid.NewGuid();
            crmService.Create(Arg.Any<Entity>()).Returns(expectedId);
            var service = new PipelineOrganizationService(crmService);
            var before = string.Empty;
            var after = string.Empty;
            var pipeline = new XrmCreatePipeline((request, next) =>
            {
                before = "BEFORE";
                var result = next();
                after = "AFTER";

                return result;
            });
            service.AddPipeline(pipeline);
            var entity = new Entity();
            var id = service.Create(entity);
                
            crmService.Received(1).Create(Arg.Is(entity));
            before.ShouldBe("BEFORE");
            after.ShouldBe("AFTER");
            id.ShouldBe(expectedId);
        }
            
        [Fact]
        public void With_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var expectedId = Guid.NewGuid();
            crmService.Create(Arg.Any<Entity>()).Returns(expectedId);
            var service = new PipelineOrganizationService(crmService);
            var before = string.Empty;
            var after = string.Empty;
            var pipeline1 = new XrmCreatePipeline((request, next) =>
            {
                before += "HELLO";
                var result = next();
                after += " BAR";

                return result;
            });
            var pipeline2 = new XrmCreatePipeline((request, next) =>
            {
                before += " WORLD";
                var result = next();
                after += "FOO";
                    
                return result;
            });
            service.AddPipeline(pipeline1);
            service.AddPipeline(pipeline2);
            var entity = new Entity();
            var id = service.Create(entity);
            
            crmService.Received(1).Create(Arg.Is(entity));
            before.ShouldBe("HELLO WORLD");
            after.ShouldBe("FOO BAR");
            id.ShouldBe(expectedId);
        }
    }
        
    public class RetrieveTests
    {
        [Fact]
        public void Without_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var expectedEntity = new Entity();
            crmService.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(expectedEntity);
            var service = new PipelineOrganizationService(crmService);
            var id = Guid.NewGuid();
            var columnSet = new ColumnSet();
            var entity = service.Retrieve("crm_entity", id, columnSet);
                
            crmService.Received(1).Retrieve(
                Arg.Is("crm_entity"),
                Arg.Is(id),
                Arg.Is(columnSet));
            entity.ShouldBe(expectedEntity);
        }

        [Fact]
        public void With_a_pipeline()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var expectedEntity = new Entity();
            crmService.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(expectedEntity);
            var service = new PipelineOrganizationService(crmService);
            var before = string.Empty;
            var after = string.Empty;
            var pipeline = new XrmRetrievePipeline((request, next) =>
            {
                before = "BEFORE";
                var result = next();
                after = "AFTER";

                return result;
            });
            service.AddPipeline(pipeline);
            var id = Guid.NewGuid();
            var columnSet = new ColumnSet();
            var entity = service.Retrieve("crm_entity", id, columnSet);
                
            crmService.Received(1).Retrieve(
                Arg.Is("crm_entity"),
                Arg.Is(id),
                Arg.Is(columnSet));
            before.ShouldBe("BEFORE");
            after.ShouldBe("AFTER");
            entity.ShouldBe(expectedEntity);
        }
            
        [Fact]
        public void With_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var expectedEntity = new Entity();
            crmService.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(expectedEntity);
            var service = new PipelineOrganizationService(crmService);
            var before = string.Empty;
            var after = string.Empty;
            var pipeline1 = new XrmRetrievePipeline((request, next) =>
            {
                before += "HELLO";
                var result = next();
                after += " BAR";
                    
                return result;
            });
            var pipeline2 = new XrmRetrievePipeline((request, next) =>
            {
                before += " WORLD";
                var result = next();
                after += "FOO";
                    
                return result;
            });
            service.AddPipeline(pipeline1);
            service.AddPipeline(pipeline2);
            var id = Guid.NewGuid();
            var columnSet = new ColumnSet();
            var entity = service.Retrieve("crm_entity", id, columnSet);
           
            crmService.Received(1).Retrieve(
                Arg.Is("crm_entity"),
                Arg.Is(id),
                Arg.Is(columnSet));
            before.ShouldBe("HELLO WORLD");
            after.ShouldBe("FOO BAR");
            entity.ShouldBe(expectedEntity);
        }
    }
    
    public class UpdateTests
    {
        [Fact]
        public void Without_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var entity = new Entity();
            service.Update(entity);
                
            crmService.Received(1).Update(Arg.Is(entity));
        }

        [Fact]
        public void With_a_pipeline()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var before = string.Empty;
            var after = string.Empty;
            var pipeline = new XrmUpdatePipeline((request, next) =>
            {
                before = "BEFORE";
                var result = next();
                after = "AFTER";

                return result;
            });
            service.AddPipeline(pipeline);
            var entity = new Entity();
            service.Update(entity);
                
            crmService.Received(1).Update(Arg.Is(entity));
            before.ShouldBe("BEFORE");
            after.ShouldBe("AFTER");
        }
            
        [Fact]
        public void With_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var before = string.Empty;
            var after = string.Empty;
            var pipeline1 = new XrmUpdatePipeline((request, next) =>
            {
                before += "HELLO";
                var result = next();
                after += " BAR";

                return result;
            });
            var pipeline2 = new XrmUpdatePipeline((request, next) =>
            {
                before += " WORLD";
                var result = next();
                after += "FOO";
                    
                return result;
            });
            service.AddPipeline(pipeline1);
            service.AddPipeline(pipeline2);
            var entity = new Entity();
            service.Update(entity);
            
            crmService.Received(1).Update(Arg.Is(entity));
            before.ShouldBe("HELLO WORLD");
            after.ShouldBe("FOO BAR");
        }
    }
    
    public class DeleteTests
    {
        [Fact]
        public void Without_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var id = Guid.NewGuid();
            service.Delete("crm_entity", id);
                
            crmService.Received(1).Delete(Arg.Is("crm_entity"), Arg.Is(id));
        }

        [Fact]
        public void With_a_pipeline()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var before = string.Empty;
            var after = string.Empty;
            var pipeline = new XrmDeletePipeline((request, next) =>
            {
                before = "BEFORE";
                var result = next();
                after = "AFTER";

                return result;
            });
            service.AddPipeline(pipeline);
            var id = Guid.NewGuid();
            service.Delete("crm_entity", id);
                
            crmService.Received(1).Delete(Arg.Is("crm_entity"), Arg.Is(id));
            before.ShouldBe("BEFORE");
            after.ShouldBe("AFTER");
        }
            
        [Fact]
        public void With_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var before = string.Empty;
            var after = string.Empty;
            var pipeline1 = new XrmDeletePipeline((request, next) =>
            {
                before += "HELLO";
                var result = next();
                after += " BAR";

                return result;
            });
            var pipeline2 = new XrmDeletePipeline((request, next) =>
            {
                before += " WORLD";
                var result = next();
                after += "FOO";
                    
                return result;
            });
            service.AddPipeline(pipeline1);
            service.AddPipeline(pipeline2);
            var id = Guid.NewGuid();
            service.Delete("crm_entity", id);
                
            crmService.Received(1).Delete(Arg.Is("crm_entity"), Arg.Is(id));
            before.ShouldBe("HELLO WORLD");
            after.ShouldBe("FOO BAR");
        }
    }
}