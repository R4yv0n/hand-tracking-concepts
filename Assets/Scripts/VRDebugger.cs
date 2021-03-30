using System;
using UnityEngine;
using UnityEngine.UI;

public class VRDebugger : MonoBehaviour
{
    public static VRDebugger Instance;
    public Boolean showDebugger;
    public Text logText;
    private OVRHand _rHand;
    private Boolean delayOn;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {   
        if (showDebugger) {
            var rt = DebugUIBuilder.instance.AddLabel("Debug");
            logText = rt.GetComponent<Text>();
                DebugUIBuilder.instance.Show();
        }
        //_rHand = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefab").GetComponent<OVRHand>();
    }

    private void FixedUpdate()
    {
        /*if (_rHand.GetFingerIsPinching(OVRHand.HandFinger.Pinky))
        {
            showDebugger = !showDebugger;
        }*/
    }

    public void ToggleDebugger()
    {
        delayOn = true;
        

    }

    public void Log(string msg)
    {
        logText.text = msg;
    }
    
    public void Log(Exception e)
    {
        logText.text = e.Message + "\n" + e.Source + "\n" + e.HelpLink + "\n" + e.StackTrace;
    }

    public void AddLogLine(string msg)
    {
        logText.text += "\n" + msg;
    }
}
