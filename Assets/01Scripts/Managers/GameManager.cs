using Rollic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private InputController inputController;
    [SerializeField] private LevelController levelController;
    [SerializeField] private PoolManager poolManager;

    private int _currentLevel;
    private int _levelCount;

    private void Start()
    {
        poolManager.Initialize();
        inputController.Initialize(mainCam);

        _currentLevel = PlayerPrefs.GetInt(Constants.Prefs.NextLevel, 1);
        _levelCount = Resources.Load<LevelCount>("LevelCount").levelCount;

        levelController.InitializeLevel(_currentLevel);
        levelController.LevelComplete += OnLevelComplete;
        levelController.NextLevel += OnNextLevel;
        levelController.TryAgain += OnTryAgain;
    }

    private void OnLevelComplete()
    {
        if (PlayerPrefs.GetInt(Constants.Prefs.AllLevelsComplete, 0) == 1)
            _currentLevel = Random.Range(1, _levelCount + 1);
        else if ((_currentLevel + 1) > _levelCount)
        {
            PlayerPrefs.SetInt(Constants.Prefs.AllLevelsComplete, 1);
            _currentLevel = Random.Range(1, _levelCount + 1);
        }
        else
            ++_currentLevel;

        PlayerPrefs.SetInt(Constants.Prefs.NextLevel, _currentLevel);
        PlayerPrefs.Save();
    }

    private void OnNextLevel()
    {
        levelController.UnloadLevel();
        levelController.InitializeLevel(_currentLevel);
    }

    private void OnTryAgain()
    {
        levelController.UnloadLevel();
        levelController.InitializeLevel(_currentLevel);
    }
}
