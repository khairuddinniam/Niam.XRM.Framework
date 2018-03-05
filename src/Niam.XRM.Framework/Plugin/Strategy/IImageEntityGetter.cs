namespace Niam.XRM.Framework.Plugin.Strategy
{
    internal interface IImageEntityGetter
    {
        ImageEntityGetterBase GetHandler(int stage);
    }
}