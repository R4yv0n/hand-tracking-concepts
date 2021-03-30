
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using PDollarGestureRecognizer;

public class MovePatternV2 : MonoBehaviour
{
    public bool active;
    public OVRHand[] hands;
    public bool controlledByPinching = true;
    public OVRHand.HandFinger fingerToPinch;
    public float recordFrequency;
    public float defaultThreshold;
    public KeyCode saveKey;
    public UnityEvent noneRecognized;
    public List<PDollarGestureRecognizer.Gesture> pGesture = new List<PDollarGestureRecognizer.Gesture>();
    private List<Vector3> recordedPointsV3 = new List<Vector3>();
    private List<Point> convertedPoints = new List<Point>();
    private int handToRecordId = -1;
    private bool handIdSet;
    private bool recordingDone;
    private bool saveDone = true;
    private bool startDetection;
    private GameObject[] handPointer;

    private void Start()
    {
        recordFrequency *= Camera.main.pixelWidth;
        handPointer = new[] {new GameObject(), new GameObject()};
        handPointer[0].transform.SetParent(hands[0].transform);
        handPointer[0].transform.localPosition = new Vector3(0.2f, 0.05f, 0f);
        handPointer[1].transform.SetParent(hands[1].transform);
        handPointer[1].transform.localPosition = new Vector3(-0.2f, -0.05f, 0f);
    }

    void Update()
    {
        if (active)
        {
            if ((controlledByPinching && hands[1].GetFingerIsPinching(fingerToPinch)) || (!controlledByPinching && startDetection))
            {
                if (!handIdSet)
                {
                    recordedPointsV3.Clear();
                    if (controlledByPinching)
                    {
                        handToRecordId = hands[1].GetFingerIsPinching(fingerToPinch) ? 1 : 0;
                    }
                    handIdSet = true;
                    //set start point
                    recordedPointsV3.Add(Camera.main.WorldToScreenPoint(handPointer[handToRecordId].transform.position));
                }
                // DebugCanvas.Instance.textfields[5].text = "handtorecordid:  " + handToRecordId;
                // DebugCanvas.Instance.textfields[13].text = hands[1].HandConfidence.ToString();
                if (handToRecordId > -1  && hands[1].HandConfidence != OVRHand.TrackingConfidence.Low) 
                {
                    //check if the hand has moved enough to reach the threshold
                    var newPos = Camera.main.WorldToScreenPoint(handPointer[handToRecordId].transform.position);
                    // DebugCanvas.Instance.textfields[2].text = "Last Point: " + recordedPointsV3.Last();
                    // DebugCanvas.Instance.textfields[3].text = "Distance last point: " + Vector3.Distance(newPos, recordedPointsV3.Last());
                    // DebugCanvas.Instance.textfields[4].text = "recorded points amount " + recordedPointsV3.Count;
                    if (Vector3.Distance(newPos, recordedPointsV3.Last()) >=
                        recordFrequency)
                    {
                        var vgPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        vgPoint.transform.localScale = new Vector3(0.025f, .025f, .025f);
                        vgPoint.GetComponent<Renderer>().material.color = Color.blue;
                        vgPoint.transform.position = handPointer[handToRecordId].transform.position;
                        Destroy(vgPoint, 5);
                        recordedPointsV3.Add(newPos);
                    }
                }
            }
            else
            {
                if (handToRecordId > -1)
                {
                    recordingDone = true;
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
                startDetection = false;
            }
            // DebugCanvas.Instance.textfields[12].text = "gesture amount: " + pGesture.Count + recordingDone;
            if (recordingDone && recordedPointsV3.Count > 1 && pGesture.Count > 0)
            {
                var recognizedPattern = PointCloudRecognizer.Classify(new PDollarGestureRecognizer.Gesture(convertVectorToPoints(recordedPointsV3).ToArray()), pGesture.ToArray());
                var recognizedMoveGesture = pGesture.First(g => g.Name.Equals(recognizedPattern.GestureClass));
                if (recognizedPattern.Score >= defaultThreshold)
                {
                    // DebugCanvas.Instance.textfields[11].text = "gesture found: " + recognizedMoveGesture.Name + recognizedPattern.Score;
                    recognizedMoveGesture.onRecognized?.Invoke();
                }
                else
                {
                    // DebugCanvas.Instance.textfields[12].text = "recognized: none";
                    noneRecognized?.Invoke();
                }
                recordingDone = false;
                startDetection = false;
            }
        }
    }

    private void SavePattern()
    {
        // DebugCanvas.Instance.textfields[9].text = "recorded point: " + recordedPointsV3.Count;
        if (recordedPointsV3.Count > 0)
        {
            var newPattern = new PDollarGestureRecognizer.Gesture(convertVectorToPoints(recordedPointsV3).ToArray());
            newPattern.Name = "New Pattern " + pGesture.Count;
            // DebugCanvas.Instance.textfields[10].text = "saved gesture: " + newPattern.Name;
            pGesture.Add(newPattern);
        }
    }

    private List<Point> convertVectorToPoints(List<Vector3> vectorList)
    {
        var pointList = new List<Point>();
        foreach (var vector3 in vectorList)
        {
            pointList.Add(new Point(vector3.x, vector3.y, 0));
        }
        return pointList;
    }

    public void StartDetection(int handId)
    {
        if (!startDetection)
        {
            startDetection = true;
            handToRecordId = handId;
        }
    }

    public void StopDetection(int handId)
    {
        if (handToRecordId == handId)
        {
            startDetection = false;
        }
    }
    
}
