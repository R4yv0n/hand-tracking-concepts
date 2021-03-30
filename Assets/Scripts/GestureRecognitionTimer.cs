using UnityEngine;
using UnityEngine.UI;

public class GestureRecognitionTimer : MonoBehaviour
{
    public GestureDetection gestureDetector;
    public Text timerText;
    public CircleFillScript loadingCircle;
    private Gesture gestureToRecognize;
    private Gesture lastDetectedGesture;
    private bool startTimer = false;
    private float timeLeft;
    private bool gestureRecognized = false;

    void Update()
    {
        if (startTimer)
        {
            if (!gestureRecognized && gestureToRecognize.Equals(gestureDetector.currentGesture))
            {
                if (timeLeft > 0)
                {
                    timeLeft -= Time.deltaTime;
                    if (timerText)
                    {
                        timerText.text = Mathf.FloorToInt(timeLeft % 60).ToString();
                    }
                    if (loadingCircle)
                    {
                        loadingCircle.SetProgress(timeLeft/gestureToRecognize.gestureRecognitionTime);
                    }
                }
                else
                {
                    if (timerText){
                        timerText.text = "Gesture: " + gestureToRecognize.name;
                    }
                    
                    lastDetectedGesture = gestureToRecognize;
                    lastDetectedGesture.onRecognized?.Invoke();
                    gestureDetector.anyRecognize?.Invoke();
                    StopDetector();
                    startTimer = false;
                    gestureRecognized = true;
                }
            }
            else
            {
                StopDetector();
            }
        }
        else if (gestureRecognized && !lastDetectedGesture.Equals(gestureDetector.currentGesture))
        {
            lastDetectedGesture.onReleased?.Invoke();
            gestureRecognized = false;
            StopDetector();
        }
    }

    public void StartTimer(Gesture gesture)
    {
        if (!startTimer && !gesture.Equals(new Gesture()))
        {
            gestureToRecognize = gesture;
            startTimer = true;
            timeLeft = gesture.gestureRecognitionTime;
            if (loadingCircle)
            {
                loadingCircle.SetParent(gestureDetector.hand.transform);
                loadingCircle.StartProgress();
            }
        }
    }
    

    //stop GestureDetections current detection attempt
    private void StopDetector()
    {
        startTimer = false;
        gestureDetector.StopTimer();
        //gestureDetector.SetActive(false);
    }
    
    //get stopped by GestureDetection 
    public void StopTimer()
    {
        startTimer = false;
        if (timerText)
        {
            timerText.text = gestureToRecognize.gestureRecognitionTime.ToString();
        }
        
        if (loadingCircle)
        {
            loadingCircle.StopProgress();
        }
    }
}
