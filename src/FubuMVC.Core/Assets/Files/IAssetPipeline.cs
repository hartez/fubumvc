namespace FubuMVC.Core.Assets.Files
{
    public interface IAssetPipeline
    {
        AssetFile Find(string path);
        AssetPath AssetPathOf(AssetFile file);
    }
}