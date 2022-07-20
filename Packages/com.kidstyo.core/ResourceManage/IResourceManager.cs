public interface IResourceManager
{
    // 传入完整路径
    // 如果是AB路径会自动解析 bundleName和assetName
    // TODO bundle是否有后缀后续制定
    T LoadAsset<T>(string path) where T : UnityEngine.Object;
}
