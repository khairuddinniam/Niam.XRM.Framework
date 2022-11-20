using System;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Plugin;
using Shouldly;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin;

public class ToEntityOrganizationServicePipelineTests
{
    [Fact]
    public void To_entity_on_create()
    {
        var request = new XrmCreateRequest(new CustomEntity());
        var id = Guid.NewGuid();
        var next = () => id;
        
        var result = new ToEntityOrganizationServicePipeline().Handle(request, next);
        request.Entity.ShouldBeOfType<Entity>();
        result.ShouldBe(id);
    }
    
    [Fact]
    public void To_entity_on_update()
    {
        var request = new XrmUpdateRequest(new CustomEntity());
        var next = () => Unit.Value;
        
        var result = new ToEntityOrganizationServicePipeline().Handle(request, next);
        request.Entity.ShouldBeOfType<Entity>();
        result.ShouldBe(Unit.Value);
    }
    
    private class CustomEntity : Entity
    {
    }
}