using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(Tile))]
public class TileInspector : Editor
{
    private Tile _tile;
    public override VisualElement CreateInspectorGUI()
    {
        _tile = (Tile)target;
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isWalkable"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            _tile.StatusChange?.Invoke();
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("visualConfigurations"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("walkable"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unwalkable"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tileOccupierParent"));
        serializedObject.ApplyModifiedProperties();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("occupantType"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            _tile.DestroyOccupier(true);

            GameObject prefab = LevelEditor.TileOccupantDatabase.GetPrefab(_tile.occupantType);

            if (prefab == null)
            {
                _tile.SetOccupier(null);
            }
            else
            {
                _tile.SetOccupier(
                    ((GameObject)PrefabUtility.InstantiatePrefab(prefab))
                    .GetComponent<BaseTileOccupier>()
                );
            }
        }
    }
}
