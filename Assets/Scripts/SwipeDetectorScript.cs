using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Events;
using PDollarGestureRecognizer;
using Debug = UnityEngine.Debug;

public class SwipeDetectorScript : MonoBehaviour
{
    public OVRHand[] hands;
    public List<UnityEvent> onRecognized;
    private List<Vector2> directions = new List<Vector2>();
    private bool handIdSet;
    private bool recordingDone;
    private bool startDetection;
    private int handToRecordId;
    private Vector2 startPos;
    private Vector2 newPos;
    private Vector2 currentMovingVector;
    private Vector2 lastMovingVector;
    private float timeLeft = 0.5f;
    private void Start()
    {
        directions.Add(Vector2.left);
        directions.Add(Vector2.up);
        directions.Add(Vector2.right);
        directions.Add(Vector2.down);
        
        lastMovingVector = Vector2.zero;
        currentMovingVector = Vector2.zero;
    }

    void Update()
    {
        if (startDetection && hands[handToRecordId].HandConfidence == OVRHand.TrackingConfidence.High )
        {
            if (timeLeft == 0.5)
                startPos = Camera.main.WorldToScreenPoint(hands[handToRecordId].transform.position);
            if (timeLeft > 0)
            {
                newPos = Camera.main.WorldToScreenPoint(hands[handToRecordId].transform.position);

                foreach (var dir in directions)
                {
                    if (Vector2.Dot(newPos - startPos, dir) >= 0.8)
                    {
                        Debug.Log("found matching vector");
                        currentMovingVector = dir;
                    }
                }

                if (!lastMovingVector.Equals(currentMovingVector))
                {
                    Debug.Log("time reset");
                    timeLeft = 0.5f;
                }
                if(timeLeft == 0.5)
                    lastMovingVector = currentMovingVector;
                
                timeLeft -= Time.deltaTime;
            }
            else
            {
                Debug.Log("time over");
                for (int i = 0; i < directions.Count; i++)
                {
                    if (directions[i].Equals(currentMovingVector))
                    {
                        Debug.Log("invoking: " + i);
                        onRecognized[i]?.Invoke();
                    }
                }
                timeLeft = 0.5f;
                startDetection = false;
            }
        }
        else
        {
            timeLeft = 0.5f;
        }
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
