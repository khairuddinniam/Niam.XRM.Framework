using System;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Niam.XRM.Framework.Plugin;
using Niam.XRM.Framework.Plugin.Strategy;
using Xunit;

namespace Niam.XRM.Framework.Tests.Plugin.Strategy
{
    public class ImageEntityGetterTests
    {
        [Fact]
        public void Can_get_image_stage_validation()
        {
            var entity = new Entity("entity") { Id = Guid.NewGuid() };
            var context = Substitute.For<IPluginExecutionContext>();
            context.Stage.Returns((int) SdkMessageProcessingStepStage.Prevalidation);
            context.PreEntityImages.Returns(new EntityImageCollection
            {
                [ImageEntityGetter.ImageKey] = entity
            });

            var handler = ImageEntityGetter.GetHandler(context.Stage);
            Assert.Equal(entity, handler.Get(context));
        }

        [Fact]
        public void Can_get_image_stage_preoperation()
        {
            var entity = new Entity("entity") { Id = Guid.NewGuid() };
            var context = Substitute.For<IPluginExecutionContext>();
            context.Stage.Returns((int) SdkMessageProcessingStepStage.Preoperation);
            context.PreEntityImages.Returns(new EntityImageCollection
            {
                [ImageEntityGetter.ImageKey] = entity
            });

            var handler = ImageEntityGetter.GetHandler(context.Stage);
            Assert.Equal(entity, handler.Get(context));
        }

        [Fact]
        public void Can_get_image_stage_postoperation()
        {
            var entity = new Entity("entity") { Id = Guid.NewGuid() };
            var context = Substitute.For<IPluginExecutionContext>();
            context.Stage.Returns((int) SdkMessageProcessingStepStage.Postoperation);
            context.PostEntityImages.Returns(new EntityImageCollection
            {
                [ImageEntityGetter.ImageKey] = entity
            });

            var handler = ImageEntityGetter.GetHandler(context.Stage);
            Assert.Equal(entity, handler.Get(context));
        }

        [Theory]
        [InlineData((int) SdkMessageProcessingStepStage.Prevalidation)]
        [InlineData((int) SdkMessageProcessingStepStage.Preoperation)]
        [InlineData((int) SdkMessageProcessingStepStage.Postoperation)]
        public void Can_get_image_no_image_registered(int stage)
        {
            var context = Substitute.For<IPluginExecutionContext>();
            context.PreEntityImages.Returns(new EntityImageCollection());
            context.PostEntityImages.Returns(new EntityImageCollection());
            context.Stage.Returns(stage);

            var handler = ImageEntityGetter.GetHandler(context.Stage);
            Assert.Null(handler.Get(context));
        }
    }
}
