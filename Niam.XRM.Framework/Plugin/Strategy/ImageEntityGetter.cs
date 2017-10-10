using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin.Strategy
{
    internal class ImageEntityGetter : IImageEntityGetter
    {
        public static string ImageKey => "EntityImage";

        public static IImageEntityGetter Default { get; } = new ImageEntityGetter();

        private readonly IDictionary<int, ImageEntityGetterBase> _handlers;

        private ImageEntityGetter()
        {
            var preImageHandler = new PreImage();
            var postImageHandler = new PostImage();
            _handlers = new Dictionary<int, ImageEntityGetterBase>
            {
                [(int) SdkMessageProcessingStepStage.Prevalidation] = preImageHandler,
                [(int) SdkMessageProcessingStepStage.Preoperation] = preImageHandler,
                [(int) SdkMessageProcessingStepStage.Postoperation] = postImageHandler
            };
        }

        ImageEntityGetterBase IImageEntityGetter.GetHandler(int stage)
        {
            ImageEntityGetterBase handler;
            if (_handlers.TryGetValue(stage, out handler))
                return handler;

            throw new InvalidPluginExecutionException($"Stage '{stage}' doesn't have {nameof(ImageEntityGetterBase)} handler.");
        }
        
        private class PreImage : ImageEntityGetterBase
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.PreEntityImages.GetImage(ImageKey);
        }

        private class PostImage : ImageEntityGetterBase
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.PostEntityImages.GetImage(ImageKey);
        }
    }
}