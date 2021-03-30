using System;
using UnityEngine;

public class ObjDetectorScript : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRHand rightHand;
    public LayerMask objectLayerMask;
    public Boolean debugMode;
    private GameObject lastCollideObject = null;
    private Renderer rRenderer;
    private Renderer lRenderer;
    private GameObject emptyCompare;
    private Boolean rhandInPosition = false;
    private Boolean lhandInPosition = false;
    private bool[] handGrabbingState = new [] {false, false};
    private OVRHand[] hands;
    // Start is called before the first frame update
    void Start()
    {
        emptyCompare = new GameObject();
        rRenderer = rightHand.GetComponent<Renderer>();
        lRenderer = leftHand.GetComponent<Renderer>();
        hands = new OVRHand[] {leftHand, rightHand};
    }

    // Update is called once per frame
    void Update()
    {
        if (debugMode)
        {
            DebugLineScript.Instance.setStartAndEnd(rRenderer.bounds.center, lRenderer.bounds.center);
            DebugLineScript.Instance.setEnabled(true);
        }
        
        if (rhandInPosition && lhandInPosition)
        {
            var rTransPos = rRenderer.bounds.center;
            var lTransPos = lRenderer.bounds.center;
            var direction = lTransPos - rTransPos;
            var maxDistance = Vector3.Distance(rTransPos, lTransPos);
            var rayCastHit = new RaycastHit();
            var collideObject = emptyCompare;
            if (Physics.Raycast(rTransPos, direction, out rayCastHit, maxDistance, objectLayerMask) && lastCollideObject == null)
            {
                collideObject = rayCastHit.collider.gameObject;
                if (lastCollideObject != collideObject)
                {
                    collideObject.GetComponent<ObjectResize>().CubeIsBetweenHands(true);
                    lastCollideObject = collideObject;
                }
            }
        }
        else if (lastCollideObject)
        {
            lastCollideObject.GetComponent<ObjectResize>().CubeIsBetweenHands(false);
            lastCollideObject = null;
        }
    }

    public void SetrHandInPosition(Boolean b)
    {
        rhandInPosition = b;
    }
    
    public void SetlHandInPosition(Boolean b)
    {
        lhandInPosition = b;
    }

    public void SetHandIsGrabbing(int handId)
    {
        handGrabbingState[handId] = true;
    }
    
    public void SetHandStopGrabbing(int handId)
    {
        handGrabbingState[handId] = false;
        LetGoOfAllObjects(handId);
    }

    public bool GetHandState(int handId)
    {
        return handGrabbingState[handId];
    }

    private void LetGoOfAllObjects(int handID)
    {
        foreach (Transform child in hands[handID].transform)
        {
            if (child.tag.Equals("TouchObject"))
            {
                child.SetParent(null);
            }
        }
    }
}
