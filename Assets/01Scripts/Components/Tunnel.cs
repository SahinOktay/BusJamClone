using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : BaseTileOccupier
{
    [SerializeField] private GameLogicColor[] characterColors;
    [SerializeField] private GameLogicDirection direction;

    private Stack<Character> _characterStack;

    public void Setup(GameLogicDirection direction, Stack<Character> characterStack)
    {
        transform.eulerAngles = Vector3.up * ((int)direction * 90);
        this.direction = direction;
        _characterStack = characterStack;

        characterColors = new GameLogicColor[characterStack.Count];
        int index = 0;
        foreach (Character character in characterStack)
        {
            characterColors[index++] = character.LogicColor;
            character.gameObject.SetActive(false);
            character.transform.SetParent(transform);
            character.transform.localPosition = Vector3.down * 2;
        }
    }

    public TunnelData GetTunnelData()
    {
        TunnelData tunnelData = new TunnelData();
        tunnelData.characterColors = characterColors;
        tunnelData.direction = direction;
        return tunnelData;
    }

    private void OnValidate()
    {
        transform.eulerAngles = Vector3.up * ((int)direction * 90);
    }
}
