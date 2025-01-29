using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

[CustomEditor(typeof(Tile)), CanEditMultipleObjects]
public class TileInspector : Editor
{
    private Tile[] tiles;

    public override void OnInspectorGUI()
    {
        Object[] objects = targets;
        tiles = objects.Select(item => item as Tile).ToArray();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isWalkable"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            tiles[0].StatusChange?.Invoke();
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
            for (int i = 0; i < tiles.Length; i++)
                tiles[i].NeedNewOccupier?.Invoke(tiles[i], tiles[i].occupantType);
        }
    }
}
