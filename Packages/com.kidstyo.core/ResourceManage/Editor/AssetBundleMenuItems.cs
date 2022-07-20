using System.IO;
using UnityEditor;

public class AssetBundleMenuItems
{
    const string kSimulationMode = "AssetBundles/本地模拟加载";

    [MenuItem(kSimulationMode)]
    public static void ToggleSimulationMode()
    {
        AssetBundleManager.SimulateInEditor = !AssetBundleManager.SimulateInEditor;
    }

    [MenuItem(kSimulationMode, true)]
    public static bool ToggleSimulationModeValidate()
    {
        Menu.SetChecked(kSimulationMode, AssetBundleManager.SimulateInEditor);
        return true;
    }
    
    [MenuItem("AssetBundles/打包测试")]
    public static void TestBuild()
    {
        // var outPut = Path.Combine(Application.streamingAssetsPath, "UI");
        
        //参数一为打包到哪个路径，参数二压缩选项  参数三 平台的目标
        BuildPipeline.BuildAssetBundles("AssetBundles/AAA", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}