using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.Framework.Plugin;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public static class PipelineOrganizationServiceTests
    {
        public class CreateTests
        {
            [Fact]
            public void Without_pipeline()
            {
                var crmService = Substitute.For<IOrganizationService>();
                var service = new PipelineOrganizationService(crmService);
                service.Create(new Entity());
                
                crmService.Received(1).Create(Arg.Any<Entity>());
            }

            [Fact]
            public void With_pipeline()
            {
                var crmService = Substitute.For<IOrganizationService>();
                var service = new PipelineOrganizationService(crmService);
                service.CreatePipelines.Add(new SinglePipeline());
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
                service.CreatePipelines.Add(new Pipeline1());
                service.CreatePipelines.Add(new Pipeline2());
                var entity = new Entity();
                service.Create(entity);
                
                /*
                 * Orders:
                 * - Pipeline1.Pre
                 * - Pipeline2.Pre
                 * - next
                 * - Pipeline2.Post
                 * - Pipeline2.Post
                 *
                 * Pre = code before call next func.
                 * Post = code after call next func.
                 */
                crmService.Received(1).Create(Arg.Any<Entity>());
                entity["crm_attr"].ShouldBe("hello world");
                entity.FormattedValues["crm_attr"].ShouldBe("foo bar");
            }
            
            private class SinglePipeline : IPipeline<XrmCreateRequest, Guid>
            {
                public Guid Handle(XrmCreateRequest request, Func<Guid> next)
                {
                    // Pre
                    request.Entity.Attributes["crm_hello"] = "world";
                    
                    var result = next();
                    
                    // Post
                    request.Entity.FormattedValues["crm_foo"] = "bar";
                    
                    return result;
                }
            }
            
            private class Pipeline1 : IPipeline<XrmCreateRequest, Guid>
            {
                public Guid Handle(XrmCreateRequest request, Func<Guid> next)
                {
                    // Pre
                    request.Entity.Attributes["crm_attr"] = "hello";
                    
                    var result = next();
                    
                    // Post
                    request.Entity.FormattedValues["crm_attr"] += " bar";
                    
                    return result;
                }
            }
            
            private class Pipeline2 : IPipeline<XrmCreateRequest, Guid>
            {
                public Guid Handle(XrmCreateRequest request, Func<Guid> next)
                {
                    // Pre
                    request.Entity.Attributes["crm_attr"] += " world";
                    
                    var result = next();
                    
                    // Post
                    request.Entity.FormattedValues["crm_attr"] = "foo";
                    
                    return result;
                }
            }
        }
    }
}