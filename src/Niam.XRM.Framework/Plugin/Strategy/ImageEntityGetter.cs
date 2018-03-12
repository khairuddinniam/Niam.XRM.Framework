using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace Niam.XRM.Framework.Plugin.Strategy
{
    internal abstract class ImageEntityGetter
    {
        public static string ImageKey => "EntityImage";

        private static readonly IDictionary<int, ImageEntityGetter> Handlers;

        static ImageEntityGetter()
        {
            var preImageHandler = new PreImage();
            var postImageHandler = new PostImage();
            Handlers = new Dictionary<int, ImageEntityGetter>
            {
                [(int) SdkMessageProcessingStepStage.Prevalidation] = preImageHandler,
                [(int) SdkMessageProcessingStepStage.Preoperation] = preImageHandler,
                [(int) SdkMessageProcessingStepStage.Postoperation] = postImageHandler
            };
        }

        public static ImageEntityGetter GetHandler(int stage)
        {
            if (Handlers.TryGetValue(stage, out var handler))
                return handler;

            throw new InvalidPluginExecutionException($"Stage '{stage}' doesn't have {nameof(ImageEntityGetter)} handler.");
        }

        public abstract Entity Get(IPluginExecutionContext context);
        
        private class PreImage : ImageEntityGetter
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.PreEntityImages.GetImage(ImageKey);
        }

        private class PostImage : ImageEntityGetter
        {
            public override Entity Get(IPluginExecutionContext context)
                => context.PostEntityImages.GetImage(ImageKey);
        }
    }
}