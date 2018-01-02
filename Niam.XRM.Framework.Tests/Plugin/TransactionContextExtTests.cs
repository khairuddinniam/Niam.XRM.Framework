using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin
{
    public class TransactionContextExtTests
    {
        [Fact]
        public void Can_create_exception_with_custom_message()
        {
            var context = Substitute.For<ITransactionContextBase>();
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => ThrowErrorMessage(context));
            Assert.Equal("CUSTOM-ERROR-MESSAGE", ex.Message);
        }

        private static void ThrowErrorMessage(ITransactionContextBase context)
        {
            throw context.ErrorMessage("CUSTOM-ERROR-MESSAGE");
        }

        [Fact]
        public void Can_create_exception_with_custom_message_and_arguments()
        {
            var context = Substitute.For<ITransactionContextBase>();
            var ex = Assert.Throws<InvalidPluginExecutionException>(() => ThrowErrorMessageWithArguments(context));
            Assert.Equal("CUSTOM-ERROR-MESSAGE-One-Two", ex.Message);
        }

        private static void ThrowErrorMessageWithArguments(ITransactionContextBase context)
        {
            throw context.ErrorMessage("CUSTOM-ERROR-MESSAGE-{0}-{1}", "One", "Two");
        }
    }
}
