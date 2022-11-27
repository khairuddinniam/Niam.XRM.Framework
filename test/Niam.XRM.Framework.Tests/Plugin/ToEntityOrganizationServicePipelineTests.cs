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
        var id = Guid.NewGuid();
        Entity entity = null;
        var next = (XrmCreateRequest req) =>
        {
            entity = req.Entity;
            return id;
        };
        
        var result = new ToEntityOrganizationServicePipeline().Handle(new XrmCreateRequest(new CustomEntity()), next);
        result.ShouldBe(id);
        entity.ShouldBeOfType<Entity>();
    }
    
    [Fact]
    public void To_entity_on_update()
    {
        Entity entity = null;
        var next = (XrmUpdateRequest req) =>
        {
            entity = req.Entity;
            return Unit.Value;
        };
        
        var result = new ToEntityOrganizationServicePipeline().Handle(new XrmUpdateRequest(new CustomEntity()), next);
        result.ShouldBe(Unit.Value);
        entity.ShouldBeOfType<Entity>();
    }
    
    private class CustomEntity : Entity
    {
    }
}