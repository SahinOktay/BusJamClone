using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using Rollic;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;

public class LevelEditorWindow : EditorWindow
{
    private LevelEditor _levelEditor;
    private LevelData _levelData;
    private const int _defaultTileWidth = 32;
    private int _levelCount;

    public static LevelEditorWindow OpenWindow(LevelEditor editor)
    {
        LevelEditorWindow levelEditorWindow = GetWindow<LevelEditorWindow>();
        levelEditorWindow.titleContent = new GUIContent("Level Editor");
        levelEditorWindow._levelEditor = editor;
        levelEditorWindow.Show();
        return levelEditorWindow;
    }

    private void CreateGUI()
    {
        Load();
        _levelEditor.Initialize();
    }

    private void Load()
    {
        _levelCount = AssetDatabase.FindAssets("", new[] { "Assets/Resources/Levels" }).Length;
    }

    private void OnGUI()
    {
        string levelName;

        int currentLevelIndex = _levelData == null ? -1 : _levelData.level_number - 1;
        bool isCurrentLevel;

        for (int i = 0; i < _levelCount; i++)
        {
            levelName = "Level " + (i + 1);
            isCurrentLevel = currentLevelIndex == i;

            if (isCurrentLevel)
                GUI.backgroundColor = new Color(0.2f, 0.5f, 1) * 3;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(levelName, GUILayout.Width(EditorGUIUtility.currentViewWidth - 80)))
            {
                _levelData = new LevelData();

                EditorJsonUtility.FromJsonOverwrite(
                    (Resources.Load(
                        "Levels/level_" + (i + 1),
                        typeof(TextAsset)
                    ) as TextAsset).text,
                    _levelData
                );

                _levelEditor.SetupLevel(_levelData);
            }

            GUI.backgroundColor = Color.white;

            GUILayout.Space(10);

            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(40)))
                DeleteLevel(i);
            else 
                EditorGUILayout.EndHorizontal();
        }

        if (_levelData == null)
            _levelEditor.UnloadLevel();

        GUI.backgroundColor = new Color(2, .9f, 0, 1);

        if (_levelData != null && GUILayout.Button("Save Changes"))
            SaveLevel();

        if (GUILayout.Button("Add New Level"))
            AddNewLevel();

        GUI.backgroundColor = Color.white;
    }

    private void AddNewLevel()
    {
        LevelData generatedData = LevelData.GenerateNewData();
        generatedData.level_number = ++_levelCount;

        int tileCount = Constants.GridDimentions.x * Constants.GridDimentions.y;
        generatedData.tileStatus = new bool[tileCount];
        for (int i = 0; i < tileCount; i++)
        {
            generatedData.tileStatus[i] = false;
        }

        _levelEditor.SetupLevel(generatedData);

        LevelCount levelCount = AssetDatabase.LoadAssetAtPath<LevelCount>("Assets/Resources/LevelCount.asset");
        levelCount.levelCount = _levelCount;
        EditorUtility.SetDirty(levelCount);
        AssetDatabase.SaveAssetIfDirty(levelCount);

        _levelData = generatedData;

        SaveLevel();
    }

    private void DeleteLevel(int levelIndex)
    {
        AssetDatabase.DeleteAsset(GetLevelPath(levelIndex + 1));

        if (_levelData != null)
        {
            if (_levelData.level_number == (levelIndex + 1))
            {
                _levelEditor.UnloadLevel();
            }
            else if (_levelData.level_number > levelIndex + 1)
            {
                _levelData.level_number--;
            }
        }

        LevelData changeOrderData;

        LevelCount levelCount = AssetDatabase.LoadAssetAtPath<LevelCount>("Assets/Resources/LevelCount.asset");

        for (int j = levelIndex + 1; j < levelCount.levelCount; j++)
        {
            changeOrderData = new LevelData();

            EditorJsonUtility.FromJsonOverwrite(
                (Resources.Load(
                    "Levels/level_" + (j + 1),
                    typeof(TextAsset)
                ) as TextAsset).text,
                changeOrderData
            );

            changeOrderData.level_number = j;

            File.WriteAllText(GetLevelPath(changeOrderData.level_number + 1), EditorJsonUtility.ToJson(changeOrderData));
            AssetDatabase.RenameAsset(GetLevelPath(changeOrderData.level_number + 1), "level_" + changeOrderData.level_number);
        }

        levelCount.levelCount = _levelCount - 1;
        EditorUtility.SetDirty(levelCount);
        AssetDatabase.SaveAssetIfDirty(levelCount);
        EditorGUILayout.EndHorizontal();
        GUILayout.EndScrollView();

        Load();
        Repaint();
        return;
    }

    private void SaveLevel()
    {
        int levelNumber = _levelData.level_number;
        _levelData = _levelEditor.GetLevelData();
        _levelData.level_number = levelNumber;
        File.WriteAllText(GetLevelPath(_levelData.level_number), EditorJsonUtility.ToJson(_levelData));

        AssetDatabase.Refresh();
    }

    private static string GetLevelPath(int levelNumber) => "Assets/Resources/Levels/level_" + (levelNumber) + ".json";
}
