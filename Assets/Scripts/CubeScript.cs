using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CubeScript : MonoBehaviour
{
    public Renderer m_renderer;
    public Transform m_transform;
    public OVRHand[] m_hands;
    public Transform[] m_handsTranform;
    public GameObject[] triggerZones;
    public GameObject[] usedTriggerZones;
    private Boolean playerIsDragging;
    private float initDistBetweenHandsX;
    private float initDistBetweenHandsY;
    private float initDistBetweenHandsZ;
    private float initScaleX;
    private float initScaleY;
    private float initScaleZ;


    void Start()
    {
        m_renderer = GetComponent<Renderer>();
        m_transform = GetComponent<Transform>();
        m_hands = new OVRHand[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRHandPrefab").GetComponent<OVRHand>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefab").GetComponent<OVRHand>()
        };
        m_handsTranform = new Transform[]
        {
            m_hands[0].GetComponent<Transform>(),
            m_hands[1].GetComponent<Transform>()
        };
        triggerZones = GameObject.FindGameObjectsWithTag("CubeTriggerZones");
        GetComponent<Collider>().isTrigger = true;
        playerIsDragging = false;
        usedTriggerZones = new GameObject[2];
    }
    
    void Update()
    {
        try
        {
            if (CheckForRightHandInTriggerZone(0) && CheckForRightHandInTriggerZone(1))
            {
                m_renderer.material.color = Color.green;
                if (m_hands[0].GetFingerIsPinching(OVRHand.HandFinger.Index) &&
                    m_hands[1].GetFingerIsPinching(OVRHand.HandFinger.Index) && !playerIsDragging)
                {
                    m_renderer.material.color = Color.yellow;
                    initDistBetweenHandsX = Math.Abs(m_handsTranform[0].position.x - m_handsTranform[1].position.x);
                    initDistBetweenHandsY = Math.Abs(m_handsTranform[0].position.y - m_handsTranform[1].position.y);
                    initDistBetweenHandsZ = Math.Abs(m_handsTranform[0].position.z - m_handsTranform[1].position.z);
                    initScaleX = m_transform.localScale.x;
                    initScaleY = m_transform.localScale.y;
                    initScaleZ = m_transform.localScale.z;

                    playerIsDragging = true;
                }
            }
            else
                m_renderer.material.color = Color.white;

            if (playerIsDragging)
            {
                var usedTriggerZonePosL = usedTriggerZones[0].GetComponent<Transform>().position;
                var usedTriggerZonePosR = usedTriggerZones[1].GetComponent<Transform>().position;
                var xIsScalable = usedTriggerZonePosL.x != usedTriggerZonePosR.x;
                var yIsScalable = usedTriggerZonePosL.y != usedTriggerZonePosR.y;
                var zIsScalable = usedTriggerZonePosL.z != usedTriggerZonePosR.z;

                var xScaleMultiplier = xIsScalable ? Math.Abs(m_handsTranform[0].position.x - m_handsTranform[1].position.x) /
                                       initDistBetweenHandsX : 1;
                var yScaleMultiplier = yIsScalable ? Math.Abs(m_handsTranform[0].position.y - m_handsTranform[1].position.y) /
                                       initDistBetweenHandsY : 1;
                var zScaleMultiplier = zIsScalable ? Math.Abs(m_handsTranform[0].position.z - m_handsTranform[1].position.z) /
                                                     initDistBetweenHandsZ : 1;
                m_transform.localScale = new Vector3(initScaleX * xScaleMultiplier, initScaleY * yScaleMultiplier,
                    initScaleZ * zScaleMultiplier);
            }

            if (!m_hands[0].GetFingerIsPinching(OVRHand.HandFinger.Index) &&
                !m_hands[1].GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                playerIsDragging = false;
            }
        }
        catch (Exception e)
        {
            VRDebugger.Instance.Log(e);
        }
    }

    private Boolean CheckForRightHandInTriggerZone(int handIdx)
    {
        foreach (var triggerZone in triggerZones)
        {
            if (triggerZone.GetComponent<SphereTrigger>().handInTriggerZone == handIdx)
            {
                usedTriggerZones[handIdx] = triggerZone;
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider collider)
    {
    }
    
    private void OnTriggerExit(Collider collider)
    {
    }
 
    private int GetIndexFingerHandId(Collider collider)
    {
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