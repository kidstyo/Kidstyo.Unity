using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceManager
{
    T LoadAsset<T>(string bundlePath, string assetName) where T : UnityEngine.Object;
}
