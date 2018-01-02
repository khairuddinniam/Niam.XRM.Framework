using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Interfaces;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class EntityAccessorWrapperTests
    {
        [Fact]
        public void Can_access_accessor_members()
        {
            var accessor = Substitute.For<IEntityAccessor<xts_entity>>();
            var wrapper = new EntityAccessorWrapper<xts_entity>(accessor);

            accessor.DidNotReceive().Get<OptionSetValue>(Arg.Any<string>());
            wrapper.Get<OptionSetValue>("xts_attribute");
            accessor.Received(1).Get<OptionSetValue>(Arg.Any<string>());

            accessor.DidNotReceive().GetFormattedValue(Arg.Any<string>());
            wrapper.GetFormattedValue("xts_attribute");
            accessor.Received(1).GetFormattedValue(Arg.Any<string>());

            accessor.DidNotReceive().Set(Arg.Any<string>(), Arg.Any<object>());
            wrapper.Set("xts_attribute", 1234);
            accessor.Received(1).Set(Arg.Any<string>(), Arg.Any<object>());

            accessor.DidNotReceive().Set(Arg.Any<string>(), Arg.Any<IAttributeValueProvider>());
            wrapper.Set("xts_attribute", Substitute.For<IAttributeValueProvider>());
            accessor.Received(1).Set(Arg.Any<string>(), Arg.Any<IAttributeValueProvider>());

            accessor.DidNotReceive().Set(Arg.Any<string>(), Arg.Any<IValueProvider>());
            wrapper.Set("xts_attribute", Substitute.For<IValueProvider>());
            accessor.Received(1).Set(Arg.Any<string>(), Arg.Any<IValueProvider>());

            accessor.DidNotReceive().SetFormattedValue(Arg.Any<string>(), Arg.Any<string>());
            wrapper.SetFormattedValue("xts_attribute", "formatted-value");
            accessor.Received(1).SetFormattedValue(Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
