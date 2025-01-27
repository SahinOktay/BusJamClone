using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class LevelEditorSceneHandler
{
    private static LevelEditorWindow window;

    static LevelEditorSceneHandler()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorApplication.delayCall += CheckCurrentScene;
    }

    private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        window = EditorWindow.GetWindow<LevelEditorWindow>();
        if (window != null)
        {
            window.Close();
        }

        LevelEditor levelEditor = UnityEngine.Object.FindFirstObjectByType<LevelEditor>();
        if (levelEditor != null)
        {
            window = LevelEditorWindow.OpenWindow(levelEditor);
        }
    }

    private static void CheckCurrentScene()
    {
        OnSceneOpened(SceneManager.GetActiveScene(), OpenSceneMode.Single);
    }
}