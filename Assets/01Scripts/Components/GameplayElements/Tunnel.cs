using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tunnel : BaseTileOccupier
{
    [SerializeField] private GameLogicColor[] characterColors;
    [SerializeField] private GameLogicDirection direction;
    [SerializeField] private TMP_Text countText;

    private Stack<Character> _characterStack;

    public void Setup(GameLogicDirection direction, Stack<Character> characterStack, Tile tileInFront, Camera mainCam)
    {
        tileInFront.GotEmpty += OnFrontTileIsEmpty;
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

        countText.gameObject.SetActive(true);
        countText.text = _characterStack.Count.ToString();
        countText.transform.forward = mainCam.transform.forward;
    }

    public TunnelData GetTunnelData()
    {
        TunnelData tunnelData = new TunnelData();
        tunnelData.characterColors = characterColors;
        tunnelData.direction = direction;
        return tunnelData;
    }

    private void OnFrontTileIsEmpty(Tile tile)
    {
        Character characterFromStack = _characterStack.Pop();
        characterFromStack.gameObject.SetActive(true);
        tile.SetOccupier(characterFromStack);
        characterFromStack.transform.position = transform.position;
        characterFromStack.Move(tile.transform.position);

        countText.text = _characterStack.Count.ToString();

        if (_characterStack.Count == 0)
        {
            tile.GotEmpty -= OnFrontTileIsEmpty;
            countText.gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        transform.eulerAngles = Vector3.up * ((int)direction * 90);
    }
}
