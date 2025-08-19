// SetPlayModeStartScene.cs
using UnityEditor;
using UnityEditor.SceneManagement;

public class SetPlayModeStartScene
{
    [MenuItem("Tools/Set This Scene as Playmode Start Scene")]
    private static void SetScene()
    {
        SceneAsset currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(
            EditorSceneManager.GetActiveScene().path
        );

        if (currentScene == null)
        {
            EditorUtility.DisplayDialog("Error", "Please save the current scene before setting it as the start scene.", "OK");
            return;
        }

        EditorSceneManager.playModeStartScene = currentScene;
        EditorUtility.DisplayDialog("Success", $"'{currentScene.name}' is now set as the Playmode Start Scene.", "OK");
    }

    [MenuItem("Tools/Clear Playmode Start Scene")]
    private static void ClearScene()
    {
        EditorSceneManager.playModeStartScene = null;
        EditorUtility.DisplayDialog("Success", "The Playmode Start Scene has been cleared.", "OK");
    }
}