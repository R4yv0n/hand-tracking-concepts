using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public bool startTimerOnStart = false;
    public Text countDownTimerText;
    private bool timerIsRunning = false;
    private float timePassed = 0f;

    private void Start()
    {
        timerIsRunning = startTimerOnStart;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            countDownTimerText.text = Mathf.FloorToInt(timePassed).ToString();
            timePassed += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
        ResetTimer();
    }

    public void PauseTimer()
    {
        timerIsRunning = false;
    }

    public void ResetTimer()
    {
        timePassed = 0;
        countDownTimerText.text = timePassed.ToString();
    }
}
