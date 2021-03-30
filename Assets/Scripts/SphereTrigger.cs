using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SphereTrigger : MonoBehaviour
{
    private OVRHand[] m_hands;
    private Renderer m_renderer;
    public int handInTriggerZone;

    void Start()
    {
        m_hands = new OVRHand[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRHandPrefab").GetComponent<OVRHand>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefab").GetComponent<OVRHand>()
        };
        m_renderer = GetComponent<Renderer>();
        //we don't want the cube to move over collision, so let's just use a trigger
        GetComponent<Collider>().isTrigger = true;
        handInTriggerZone = -1;
    }

    void Update()
    {
        if (handInTriggerZone == 1)
            m_renderer.material.color = Color.red;
        else if (handInTriggerZone == 0)
            m_renderer.material.color = Color.blue;
        else
            m_renderer.material.color = Color.white;

        /*
        //check for middle finger pinch on the left hand, and make che cube red in this case
        if (m_hands[0].GetFingerIsPinching(OVRHand.HandFinger.Index)) {
            cubeRender.material.color = Color.red;
            m_renderer.material.color = Color.red;
        }
        //if no pinch, and the cube was red, make it white again
        else if (m_hands[1].GetFingerIsPinching(OVRHand.HandFinger.Index)) {
            cubeRender.material.color = Color.red;
            m_renderer.material.color = Color.red;
        }
        //if no pinch, and the cube was red, make it white again
        else if (cubeRender.material.color == Color.red) {
            cubeRender.material.color = Color.white;
            m_renderer.material.color = Color.white;
        }
    
        if (m_hands[1].GetFingerIsPinching(OVRHand.HandFinger.Middle))
            cubeRender.transform.localScale = new Vector3(.35f,.35f,.35f);
    
        if (m_isIndexStaying[1] && m_isThumbStaying[1]) {
            cubeRender.material.color = Color.yellow;
            if (!playerIsDragging)
            {
                var currentHandDist = Vector3.Distance(cubeCenterPoint, m_handsTranform[1].position);
                if (m_hands[1].GetFingerIsPinching(OVRHand.HandFinger.Index))
                {
                    initDist = Vector3.Distance(cubeCenterPoint, m_handsTranform[1].position);
                    initScale = cubeTransform.localScale;
                    playerIsDragging = true;
                }
            }
        }
    
        if (playerIsDragging)
        {
            var currentHandDist = Vector3.Distance(cubeCenterPoint, m_handsTranform[1].position);
            var scaleMultiplyer = currentHandDist/initDist;
            cubeTransform.localScale = new Vector3(initScale.x * scaleMultiplyer, initScale.y * scaleMultiplyer, initScale.z * scaleMultiplyer);
        }
    
        if (!m_hands[1].GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            playerIsDragging = false;
        }
        
        Debugger.Instance.Log("Scale:" + cubeTransform.localScale);*/
    }
    
    private void OnTriggerEnter(Collider collider) {
        //get hand associated with trigger
        int handIndexIdx = GetIndexFingerHandId(collider);
        
        if (handIndexIdx != -1 && handInTriggerZone == -1)
        {
            handInTriggerZone = handIndexIdx;
        }
    }
 
    private void OnTriggerExit(Collider collider) {
        int handIndexIdx = GetIndexFingerHandId(collider);
        
        if (handIndexIdx == handInTriggerZone && handInTriggerZone != -1)
        {
            handInTriggerZone = -1;
        }
    }
    
    private int GetIndexFingerHandId(Collider collider) {
        //Checking Oculus code, it is possible to see that physics capsules gameobjects always end with _CapsuleCollider
        if (collider.gameObject.name.Contains("_CapsuleCollider"))
        {
            //get the name of the bone from the name of the gameobject, and convert it to an enum value
            string boneName = collider.gameObject.name.Substring(0, collider.gameObject.name.Length - 16);
            OVRPlugin.BoneId boneId = (OVRPlugin.BoneId)Enum.Parse(typeof(OVRPlugin.BoneId), boneName);
 
            //if it is the tip of the Index
            if (boneId == OVRPlugin.BoneId.Hand_Index3)
                //check if it is left or right hand, and change color accordingly.
                //Notice that absurdly, we don't have a way to detect the type of the hand
                //so we have to use the hierarchy to detect current hand
                if (collider.transform.IsChildOf(m_hands[0].transform))
                {
                    return 0;
                }
                else if (collider.transform.IsChildOf(m_hands[1].transform))
                {
                    return 1;
                }
        }
        return -1;
    }
}