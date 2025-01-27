using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : BaseTileOccupier
{
    [SerializeField] private GameLogicColor color;
    [SerializeField] private Renderer mainRenderer;

    private ColorDatabase.ColorConfig _currentColorConfig;

    public CharacterData GetCharacterData() => new CharacterData() { color = this.color };
    public GameLogicColor LogicColor => color;

    public void Setup(ColorDatabase.ColorConfig colorConfig)
    {
        color = colorConfig.gameLogicColor;
        _currentColorConfig = colorConfig;

        mainRenderer.material = _currentColorConfig.passiveCharMat;
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (LevelEditor.Initialized)
            Setup(LevelEditor.ColorDatabase.GetColorConfig(this.color));
    }
#endif
}
