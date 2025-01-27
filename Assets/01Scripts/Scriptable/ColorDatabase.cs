using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorDatabase", menuName = "ScriptableObjects/ColorDatabase", order = 2)]
public class ColorDatabase : ScriptableObject
{
    [Serializable]
    public class ColorConfig
    {
        public Color color;
        public GameLogicColor gameLogicColor;
        public Material passiveCharMat;
        public Material activeCharMat;
        public Material busMat;
    }

    [SerializeField] private ColorConfig[] configs;

    public ColorConfig GetColorConfig(GameLogicColor color) => configs.First(item => item.gameLogicColor == color);
}
