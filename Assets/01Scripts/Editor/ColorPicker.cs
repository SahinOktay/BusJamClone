using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColorPicker : EditorWindow
{
    private int _colorIndex;
    private Vector2 _scrollPos;
    public Action<GameLogicColor, int> ColorPickComplete;

    public void PickColor(int colorIndex)
    {
        _colorIndex = colorIndex;
        Show();
    }

    private void CreateGUI()
    {
        _scrollPos = Vector2.zero;
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, true);
        foreach (GameLogicColor color in Enum.GetValues(typeof(GameLogicColor)))
        {
            GUI.color = LevelEditor.ColorDatabase.GetColorConfig(color).color * 3;
            if (GUILayout.Button("", GUILayout.Width(80), GUILayout.Height(80)))
            {
                ColorPickComplete?.Invoke(color, _colorIndex);
                ColorPickComplete = null;
                GUI.color = Color.white;
                EditorGUILayout.EndScrollView();
                Close();
                return;
            }
        }

        EditorGUILayout.EndScrollView();
        GUI.color = Color.white;
    }

    private void OnLostFocus()
    {
        ColorPickComplete = null;
        GUI.color = Color.white;
        Close();
    }
}
