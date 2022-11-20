using System.Collections.Generic;
using Niam.XRM.Framework.Interfaces;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Niam.XRM.Framework.Tests;

public class SubscriptionCollectionTests
{
    [Fact]
    public void Can_add()
    {
        var collection = new SubscriptionCollection();
        var pipeline1 = Substitute.For<IPipeline<string, Unit>>();
        var pipeline2 = Substitute.For<IPipeline<string, string>>();
        collection.Add(pipeline1);
        collection.Add(pipeline2);
        collection.GetAll<IPipeline<string, Unit>>().ShouldBe(new [] { pipeline1 });
        collection.GetAll<IPipeline<string, string>>().ShouldBe(new [] { pipeline2 });
    }

    [Fact]
    public void Can_remove()
    {
        var collection = new SubscriptionCollection();
        var pipeline1 = Substitute.For<IPipeline<string, Unit>>();
        collection.Add(pipeline1);
        collection.Remove(pipeline1);
        collection.GetAll<IPipeline<string, Unit>>().ShouldBeEmpty();
    }
    
    [Fact]
    public void Can_remove_using_disposable()
    {
        var collection = new SubscriptionCollection();
        var pipeline1 = Substitute.For<IPipeline<string, Unit>>();
        var disposable = collection.Add(pipeline1);
        disposable.Dispose();
        collection.GetAll<IPipeline<string, Unit>>().ShouldBeEmpty();
    }
}