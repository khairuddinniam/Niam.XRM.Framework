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
            var service = new PipelineOrganizationService(crmService);
            service.Create(new Entity());
                
            crmService.Received(1).Create(Arg.Any<Entity>());
        }

        [Fact]
        public void With_a_pipeline()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var pipeline = new XrmCreatePipeline((request, next) =>
            {
                request.Entity["crm_hello"] = "world"; // Pre
                var result = next();
                request.Entity.FormattedValues["crm_foo"] = "bar"; // Post

                return result;
            });
            service.AddPipeline(pipeline);
            var entity = new Entity();
            service.Create(entity);
                
            crmService.Received(1).Create(Arg.Any<Entity>());
            entity["crm_hello"].ShouldBe("world");
            entity.FormattedValues["crm_foo"].ShouldBe("bar");
        }
            
        [Fact]
        public void With_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var pipeline1 = new XrmCreatePipeline((request, next) =>
            {
                request.Entity["crm_attr"] = "hello"; // Pre
                var result = next();
                request.Entity.FormattedValues["crm_attr"] += " bar"; // Post

                return result;
            });
            var pipeline2 = new XrmCreatePipeline((request, next) =>
            {
                request.Entity["crm_attr"] += " world"; // Pre
                var result = next();
                request.Entity.FormattedValues["crm_attr"] = "foo"; // Post
                    
                return result;
            });
            service.AddPipeline(pipeline1);
            service.AddPipeline(pipeline2);
            var entity = new Entity();
            service.Create(entity);
                
            /*
             * Orders:
             * - Pipeline1.Pre
             * - Pipeline2.Pre
             * - next
             * - Pipeline2.Post
             * - Pipeline1.Post
             *
             * Pre = code before call next func.
             * Post = code after call next func.
             */
            crmService.Received(1).Create(Arg.Any<Entity>());
            entity["crm_attr"].ShouldBe("hello world");
            entity.FormattedValues["crm_attr"].ShouldBe("foo bar");
        }
    }
        
    public class RetrieveTests
    {
        [Fact]
        public void Without_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var id = Guid.NewGuid();
            var columnSet = new ColumnSet();
            service.Retrieve("crm_entity", id, columnSet);
                
            crmService.Received(1).Retrieve(
                Arg.Is("crm_entity"),
                Arg.Is(id),
                Arg.Is(columnSet));
        }

        [Fact]
        public void With_a_pipeline()
        {
            var crmService = Substitute.For<IOrganizationService>();
            crmService.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(new Entity());
            var service = new PipelineOrganizationService(crmService);
            var pipeline = new XrmRetrievePipeline((request, next) =>
            {
                request.ColumnSet.AddColumn("crm_column"); // Pre
                var result = next();
                result["crm_attr"] = "hello world"; // Post

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
            columnSet.Columns[0].ShouldBe("crm_column");
            entity["crm_attr"].ShouldBe("hello world");
        }
            
        [Fact]
        public void With_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            crmService.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
                .Returns(new Entity());
            var service = new PipelineOrganizationService(crmService);
            var pipeline1 = new XrmRetrievePipeline((request, next) =>
            {
                request.ColumnSet.AddColumn("hello"); // Pre
                var result = next();
                result["crm_attr"] += " bar"; // Post
                    
                return result;
            });
            var pipeline2 = new XrmRetrievePipeline((request, next) =>
            {
                request.ColumnSet.AddColumn("world"); // Pre
                var result = next();
                result["crm_attr"] = "foo"; // Post
                    
                return result;
            });
            service.AddPipeline(pipeline1);
            service.AddPipeline(pipeline2);
            var id = Guid.NewGuid();
            var columnSet = new ColumnSet();
            var entity = service.Retrieve("crm_entity", id, columnSet);
                
            /*
             * Orders:
             * - Pipeline1.Pre
             * - Pipeline2.Pre
             * - next
             * - Pipeline2.Post
             * - Pipeline1.Post
             *
             * Pre = code before call next func.
             * Post = code after call next func.
             */
            crmService.Received(1).Retrieve(
                Arg.Is("crm_entity"),
                Arg.Is(id),
                Arg.Is(columnSet));
                
            columnSet.Columns[0].ShouldBe("hello");
            columnSet.Columns[1].ShouldBe("world");
            entity["crm_attr"].ShouldBe("foo bar");
        }
    }
    
    public class UpdateTests
    {
        [Fact]
        public void Without_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            service.Update(new Entity());
                
            crmService.Received(1).Update(Arg.Any<Entity>());
        }

        [Fact]
        public void With_a_pipeline()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var pipeline = new XrmUpdatePipeline((request, next) =>
            {
                request.Entity["crm_hello"] = "world"; // Pre
                var result = next();
                request.Entity.FormattedValues["crm_foo"] = "bar"; // Post

                return result;
            });
            service.AddPipeline(pipeline);
            var entity = new Entity();
            service.Update(entity);
                
            crmService.Received(1).Update(Arg.Any<Entity>());
            entity["crm_hello"].ShouldBe("world");
            entity.FormattedValues["crm_foo"].ShouldBe("bar");
        }
            
        [Fact]
        public void With_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            var pipeline1 = new XrmUpdatePipeline((request, next) =>
            {
                request.Entity["crm_attr"] = "hello"; // Pre
                var result = next();
                request.Entity.FormattedValues["crm_attr"] += " bar"; // Post

                return result;
            });
            var pipeline2 = new XrmUpdatePipeline((request, next) =>
            {
                request.Entity["crm_attr"] += " world"; // Pre
                var result = next();
                request.Entity.FormattedValues["crm_attr"] = "foo"; // Post
                    
                return result;
            });
            service.AddPipeline(pipeline1);
            service.AddPipeline(pipeline2);
            var entity = new Entity();
            service.Update(entity);
                
            /*
             * Orders:
             * - Pipeline1.Pre
             * - Pipeline2.Pre
             * - next
             * - Pipeline2.Post
             * - Pipeline1.Post
             *
             * Pre = code before call next func.
             * Post = code after call next func.
             */
            crmService.Received(1).Update(Arg.Any<Entity>());
            entity["crm_attr"].ShouldBe("hello world");
            entity.FormattedValues["crm_attr"].ShouldBe("foo bar");
        }
    }
    
    public class DeleteTests
    {
        [Fact]
        public void Without_pipelines()
        {
            var crmService = Substitute.For<IOrganizationService>();
            var service = new PipelineOrganizationService(crmService);
            service.Delete("crm_entity", Guid.NewGuid());
                
            crmService.Received(1).Delete(Arg.Any<string>(), Arg.Any<Guid>());
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
            service.Delete("crm_entity", Guid.NewGuid());
                
            crmService.Received(1).Delete(Arg.Any<string>(), Arg.Any<Guid>());
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
            var entity = new Entity();
            service.Delete("crm_entity", Guid.NewGuid());
                
            /*
             * Orders:
             * - Pipeline1.Pre
             * - Pipeline2.Pre
             * - next
             * - Pipeline2.Post
             * - Pipeline1.Post
             *
             * Pre = code before call next func.
             * Post = code after call next func.
             */
            crmService.Received(1).Delete(Arg.Any<string>(), Arg.Any<Guid>());
            before.ShouldBe("HELLO WORLD");
            after.ShouldBe("FOO BAR");
        }
    }
}