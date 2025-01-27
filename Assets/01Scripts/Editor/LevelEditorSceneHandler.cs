using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class LevelEditorSceneHandler
{
    private static LevelEditorWindow window;

    static LevelEditorSceneHandler()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
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
}