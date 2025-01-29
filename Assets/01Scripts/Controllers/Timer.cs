using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private UIController uIController;

    private float _currentDuration;

    public Action CountDownComplete;

    public void CountDown(float duration)
    {
        _currentDuration = duration;
        enabled = true;
    }

    public void Stop()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        _currentDuration -= Time.deltaTime;
        uIController.UpdateTimer(Mathf.FloorToInt(_currentDuration));

        if (_currentDuration < 0)
        {
            CountDownComplete?.Invoke();
            enabled = false;
            return;
        }
    }
}
