using System;
using NSubstitute;
using Niam.XRM.Framework.Interfaces;
using Niam.XRM.Framework.Plugin.ValueProviders;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.ValueProviders
{
    public class ValueProviderBaseTests
    {
        [Fact]
        public void Can_get_correct_value()
        {
            var entity = new xts_entity();
            var provider = Substitute.For<ValueProviderBase<xts_entity>>();
            provider.GetValue().Returns(entity);
            IValueProvider<xts_entity> interfaceStrongTypeProvider = provider;
            IValueProvider interfaceProvider = provider;

            Assert.Same(entity, provider.GetValue());
            Assert.Same(entity, interfaceStrongTypeProvider.GetValue());
            Assert.Same(entity, interfaceProvider.GetValue());
        }

        [Fact]
        public void Can_set_to_attribute()
        {
            var date = DateTime.UtcNow;
            var provider = Substitute.For<ValueProviderBase<DateTime?>>();
            provider.GetValue().Returns(date);

            var entity = new xts_entity();
            entity.Set(e => e.xts_datetime, provider);
            Assert.Equal(date, entity.Get(e => e.xts_datetime));
        }
    }
}