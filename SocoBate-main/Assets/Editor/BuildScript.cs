using UnityEditor;
using UnityEngine;

public class BuildScript
{
    public static void PerformBuild()
    {
        string[] scenes = { "Assets/Scenes/MainScene.unity" };  // List your scenes here
        BuildPipeline.BuildPlayer(scenes, "Build/StandaloneLinux64", BuildTarget.StandaloneLinux64, BuildOptions.None);
        Debug.Log("Build completed!");
    }
}
