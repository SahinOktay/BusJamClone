using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

[CustomEditor(typeof(Tile)), CanEditMultipleObjects]
public class TileInspector : Editor
{
    private Tile[] _tiles;
    public override VisualElement CreateInspectorGUI()
    {
        Object[] objects = targets;
        _tiles = objects.Select(item => item as Tile).ToArray();
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isWalkable"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            for (int i = 0; i < _tiles.Length; i++)
            {
                _tiles[i].StatusChange?.Invoke();
            }
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
            for (int i = 0; i < _tiles.Length; i++)
            {
                _tiles[i].DestroyOccupier(true);
                GameObject prefab = LevelEditor.TileOccupantDatabase.GetPrefab(_tiles[i].occupantType);

                if (prefab == null)
                {
                    _tiles[i].SetOccupier(null);
                }
                else
                {
                    _tiles[i].SetOccupier(
                        ((GameObject)PrefabUtility.InstantiatePrefab(prefab))
                        .GetComponent<BaseTileOccupier>()
                    );
                }
            }

        }
    }
}
