using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct MovementPattern
{
    public string name;
    public List<Vector3> directions;
    public UnityEvent onRecognized;
    public float angleThreshold;
    public bool checkBothways;
}
public class MovePatternDetectorScript : MonoBehaviour
{
    public bool active;
    public OVRHand[] hands;
    public OVRHand.HandFinger fingerToPinch;
    public float recordFrequency;
    public float defaultAngleThreshhold;
    public float matchingThreshold;
    public KeyCode saveKey;
    public List<MovementPattern> movementPatterns;
    public UnityEvent noneRecognized;
    private List<Vector3> recordedPoints = new List<Vector3>();
    private List<Vector3> recordedPattern = new List<Vector3>();
    private List<Vector3> lastRecordedPattern = new List<Vector3>();
    private List<GameObject> marker = new List<GameObject>();
    private int handToRecordId = -1;
    private bool handIdSet;
    private bool recordingDone;
    private bool saveDone;
    private MovementPattern recognizedPattern;
    private List<GameObject> visualGuide = new List<GameObject>();

    private void Start()
    {
        recordFrequency *= Camera.main.pixelWidth;
    }

    void Update()
    {
        if (active)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                foreach (var o in visualGuide)
                {
                    Destroy(o);
                }
            }
            if (hands[1].GetFingerIsPinching(fingerToPinch))
            {
                if (!handIdSet)
                {
                    handToRecordId = hands[1].GetFingerIsPinching(fingerToPinch) ? 1 : 0;
                    handIdSet = true;
                    //set start point
                    recordedPoints.Add(Camera.main.WorldToScreenPoint(hands[handToRecordId].transform.position));
                }
                DebugCanvas.Instance.textfields[5].text = "handtorecordid:  " + handToRecordId;
                DebugCanvas.Instance.textfields[13].text = hands[1].HandConfidence.ToString();
                if (handToRecordId > -1 && hands[1].HandConfidence != OVRHand.TrackingConfidence.Low) 
                {
                    //check if the hand has moved enough to reach the threshold
                    var newPos = Camera.main.WorldToScreenPoint(hands[handToRecordId].transform.position);
                    DebugCanvas.Instance.textfields[2].text = "Last Point: " + recordedPoints.Last();
                    DebugCanvas.Instance.textfields[3].text = "Distance last point: " + Vector3.Distance(newPos, recordedPoints.Last());
                    DebugCanvas.Instance.textfields[4].text = "recorded points amount " + recordedPoints.Count;
                    if (Vector3.Distance(newPos, recordedPoints.Last()) >=
                        recordFrequency)
                    {
                        var vgPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        vgPoint.transform.localScale = new Vector3(0.05f, .05f, .05f);
                        vgPoint.transform.position = hands[handToRecordId].transform.position;
                        visualGuide.Add(vgPoint);
                        recordedPoints.Add(newPos);
                    }
                }
            }
            else
            {
                if (handToRecordId > -1)
                {
                    lastRecordedPattern.Clear();
                    //save directions from point to point
                    for (int i = 0; i < recordedPoints.Count - 1; i++)
                    {
                        recordedPattern.Add(recordedPoints[i + 1] - recordedPoints[i]);
                        lastRecordedPattern.Add(recordedPoints[i + 1] - recordedPoints[i]);
                    }
                    recordedPoints.Clear();
                    DebugCanvas.Instance.textfields[6].text = "recorded directions: " + recordedPattern.Count;
                    recordingDone = true;
                    saveDone = false;
                    DebugCanvas.Instance.textfields[7].text = "last rec directions: " + lastRecordedPattern.Count;
                }
                handIdSet = false;
                handToRecordId = -1;
            }

            if (Input.GetKeyDown(saveKey) && saveDone)
            {
                saveDone = false;
            }

            if (!saveDone && Input.GetKeyUp(saveKey))
            {
                SavePattern();
                saveDone = true;
                recordingDone = false;
                recordedPattern.Clear();
                lastRecordedPattern.Clear();
            }

            if (recordingDone)
            {
                try
                {
                    var recognizedPattern = RecognizePattern();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    recordingDone  = false;
                    DebugCanvas.Instance.textfields[12].text = "recognized: none";
                }/*
                if (!recognizedPattern.Equals(new MovementPattern()))
                {
                    DebugCanvas.Instance.textfields[12].text = "recognized: " + recognizedPattern.name;
                    recognizedPattern.onRecognized?.Invoke();
                }
                else
                {
                    DebugCanvas.Instance.textfields[12].text = "recognized: none";
                    noneRecognized?.Invoke();
                }*/
                recordingDone = false;
                recordedPattern.Clear();
            }
        }
    }

    private MovementPattern RecognizePattern()
    {
        DebugCanvas.Instance.textfields[14].text = "";
        List<float> dots = new List<float>();
        float avgDot = 0;
        
        var foundPattern = new MovementPattern();
        var lastmatchingpoints = 0;
        foreach (var pattern in movementPatterns)
        {
            var j = 0;
            var matchingPoints = 0;
            var matchPointNeeded = Math.Floor(pattern.directions.Count * matchingThreshold);
            var lastMatchingIndex = 0;
            var secondRound = false;
            var biggerCount = recordedPattern.Count >= pattern.directions.Count
                ? recordedPattern.Count
                : pattern.directions.Count;
            DebugCanvas.Instance.textfields[9].text = "recpat count before check: " + recordedPattern.Count;
            for (int i = 0; i < biggerCount; i++)
            {
                //set the smaller list to use j as index and the bigger one i
                var patternIndex = pattern.directions.Count <= recordedPattern.Count ? i : j;
                var recordedIndex = pattern.directions.Count <= recordedPattern.Count ? j : i;
                if (j >= pattern.directions.Count || j >= recordedPattern.Count)
                {
                    break;
                }

                DebugCanvas.Instance.textfields[8].text = "dot product: " + recordedPattern[recordedIndex] + " x ";
                DebugCanvas.Instance.textfields[8].text += pattern.directions[patternIndex];
                dots.Add(Vector3.Dot(recordedPattern[recordedIndex].normalized, pattern.directions[patternIndex].normalized));
                avgDot = dots.Average();
                if (Vector3.Dot(recordedPattern[recordedIndex].normalized, pattern.directions[patternIndex].normalized) >= pattern.angleThreshold)
                {
                    lastMatchingIndex = j;
                    matchingPoints++;
                    lastmatchingpoints = matchingPoints;
                }
                else
                {
                    j++;
                    i--;
                }
                DebugCanvas.Instance.textfields[10].text = "matching point: " + matchingPoints;
                DebugCanvas.Instance.textfields[11].text = "last matching point: " + lastmatchingpoints;
                if (matchingPoints >= matchPointNeeded)
                {
                    foundPattern = pattern;
                }
                
                // reset i and start a second round incase the start point was not the same
                if (i == biggerCount - 1 && !secondRound)
                {
                    i = 0;
                    secondRound = true;
                } 
            }
            DebugCanvas.Instance.textfields[14].text += pattern.name + " dot avg: " + avgDot + "\n";
        }
        return foundPattern;
    }
    
    private void SavePattern()
    {
        var newPattern = new MovementPattern();
        newPattern.name = "New Pattern " + movementPatterns.Count;
        DebugCanvas.Instance.textfields[9].text = "saving lastreccount: " + lastRecordedPattern.Count;
        newPattern.directions = new List<Vector3>();
        foreach (var dir in lastRecordedPattern)
        {
            newPattern.directions.Add(dir);
        }
        newPattern.angleThreshold = defaultAngleThreshhold;
        DebugCanvas.Instance.GetTextField(1).text = "Pattern saved: " + newPattern.name;
        movementPatterns.Add(newPattern);
    }
}
