using UnityEngine;

public class ResourceManager: Singleton<ResourceManager>, IResourceManager
{
    public T LoadAsset<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
}