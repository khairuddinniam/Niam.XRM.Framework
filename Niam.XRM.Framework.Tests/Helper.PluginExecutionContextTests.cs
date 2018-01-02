using Microsoft.Xrm.Sdk;
using NSubstitute;
using Xunit;

namespace Niam.XRM.Framework.Tests
{
    public class HelperPluginExecutionContextTests
    {
        [Fact]
        public void Can_get_ancestor_self()
        {
            var context = Substitute.For<IPluginExecutionContext>();
            context.ParentContext.Returns((IPluginExecutionContext) null);
            Assert.Same(context, context.GetAncestor());
        }

        [Fact]
        public void Can_get_ancestor_top()
        {
            var grandParentContext = Substitute.For<IPluginExecutionContext>();
            grandParentContext.ParentContext.Returns((IPluginExecutionContext) null);
            var parentContext = Substitute.For<IPluginExecutionContext>();
            parentContext.ParentContext.Returns(grandParentContext);
            var context = Substitute.For<IPluginExecutionContext>();
            context.ParentContext.Returns(parentContext);
            Assert.Same(grandParentContext, context.GetAncestor());
        }

        [Fact]
        public void Can_get_shared()
        {
            var context = Substitute.For<IPluginExecutionContext>();
            context.SharedVariables.Returns(new ParameterCollection());

            Assert.Null(context.GetShared<string>("hello"));
            context.SharedVariables["hello"] = "world";
            Assert.Equal("world", context.GetShared<string>("hello"));
        }
    }
}
