#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using UnityEngine;

public class AssetBundleManager : Singleton<AssetBundleManager>, IResourceManager
{
#if UNITY_EDITOR
    private static bool _simulateInEditor = true;
    const string kSimulateAssetBundles = "SimulateAssetBundles";

    /// <summary>
    /// 模拟加载
    /// </summary>
    public static bool SimulateInEditor
    {
        get
        {
            _simulateInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true);
            return _simulateInEditor;
        }
        set
        {
            _simulateInEditor = value;
            EditorPrefs.SetBool(kSimulateAssetBundles, value);
        }
    }
#endif
    
    public T LoadAsset<T>(string path) where T : Object
    {
        path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var bundleName = Directory.GetParent(path)?.Name;
        var assetName = Path.GetFileNameWithoutExtension(path);
        return LoadAsset<T>(bundleName, assetName);
    }
    
    private T LoadAsset<T>(string bundlePath, string assetName) where T : Object
    {
#if UNITY_EDITOR
        if (SimulateInEditor)
        {
            var assetPath = GetAssetPath(bundlePath, assetName);
            if (assetPath != null) return (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
        
            Debug.LogError("There is no asset with name \"" + assetName + "\" in " + bundlePath);
            return null;
        }
#endif
        var path = Path.Combine(Application.streamingAssetsPath, "AssetBundles", bundlePath);
        
        // Android 无效
        // if (!File.Exists(path))
        // {
        //     Debug.LogError("not find path:" + path);
        //     return null;
        // }
        
        var assetBundle = AssetBundle.LoadFromFile(path);
        if (assetBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return null;
        }

        var asset = assetBundle.LoadAsset<T>(assetName);
        if (null == asset)
        {
            Debug.LogError("Failed to LoadAsset:" + assetName);
        }

        assetBundle.Unload(false);
        return asset;
    }

#if UNITY_EDITOR
    private string GetAssetPath(string assetBundleName, string assetName)
    {
        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
        if (assetPaths.Length == 0)
        {
            assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(
                assetBundleName, assetName);

            if (assetPaths.Length == 0)
            {
                return null;
            }
        }
        return assetPaths[0];
    }
#endif
}
