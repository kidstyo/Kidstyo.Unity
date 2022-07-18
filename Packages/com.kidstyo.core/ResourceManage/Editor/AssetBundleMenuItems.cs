using UnityEditor;

public class AssetBundleMenuItems
{
    const string kSimulationMode = "AssetBundles/本地模拟加载";

    [MenuItem(kSimulationMode)]
    public static void ToggleSimulationMode()
    {
        ResourceManager.SimulateInEditor = !ResourceManager.SimulateInEditor;
    }

    [MenuItem(kSimulationMode, true)]
    public static bool ToggleSimulationModeValidate()
    {
        Menu.SetChecked(kSimulationMode, ResourceManager.SimulateInEditor);
        return true;
    }
}