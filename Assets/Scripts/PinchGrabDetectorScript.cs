using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PinchGrabDetectorScript : MonoBehaviour
{
    [Header("Left hand on index 0 and right hand on 1")]
    public OVRHand[] hands;
    public OVRHand.HandFinger fingerToCheck;
    public float pinchThreshold;
    public float resizeRangeThreshold;
    public LayerMask objectLayer;
    public bool disableScaleOnX;
    public bool disableScaleOnY;
    public bool disableScaleOnZ;
    public List<OVRPlugin.BoneId> viableBonesToDetect;
    private GameObject[] palmRefObjs;
    private Renderer[] handsRenderer;
    private float initHandDistance;
    private Vector3 initGameobjectScale;
    private bool initValuesSet = false;
    private Transform initParent;
    private int initHandGrabId = -1;
    private float initCubeToGrabHandDist;
    private Vector3 initCubePos;

    void Start()
    {
        gameObject.transform.parent = initParent;
        handsRenderer = new[]
        {
            hands[0].GetComponent<Renderer>(),
            hands[1].GetComponent<Renderer>()
        };
        StartCoroutine(DelayRoutine(2f, SetupHandStuff));
    }
    
    private IEnumerator DelayRoutine(float delay, Action actionToDo)
    {
        Debug.Log("delaying...");
        yield return new WaitForSeconds(delay);
        actionToDo.Invoke();
    }

    private void SetupHandStuff()
    {
        Debug.Log("setting hand stuff up");
        //create empty objects as reference point to check the hands direction
        //palm facing up
        palmRefObjs = new[] {new GameObject(), new GameObject()};
        SetupPalm(0);
        SetupPalm(1);
    }

    void Update()
    {
        if (initHandGrabId > -1)
        {
            var pinchHandId = initHandGrabId;
            var otherHandId = initHandGrabId == 0 ? 1 : 0;

            //Send a raycast from palm to palmRefObj to see if an this object is in range
            var handCenter = handsRenderer[otherHandId].bounds.center;
            var rayCastHit = new RaycastHit();
            var direction = palmRefObjs[otherHandId].transform.position - handCenter;
            var ray = new Ray(handCenter, direction);
            DebugLineScript.Instance.setStartAndEnd(handCenter, direction*resizeRangeThreshold);
            DebugLineScript.Instance.setEnabled(true);
            if (Physics.SphereCast(ray, 0.02f, out rayCastHit, resizeRangeThreshold, objectLayer))
            {
                DebugLineScript.Instance.setStartAndEnd(handCenter, rayCastHit.point);
                if (rayCastHit.collider.gameObject.Equals(gameObject))
                {
                    if (!initValuesSet)
                    {
                        initHandDistance = Vector3.Distance(handCenter, gameObject.transform.position);
                        initGameobjectScale = gameObject.transform.localScale;
                        initValuesSet = true;
                        initCubePos = gameObject.transform.position;
                        initCubeToGrabHandDist = Vector3.Distance(hands[pinchHandId].transform.position, gameObject.transform.position);
                    }
                    //Set scale of cube depending on the distance between other hand and cube
                    var scaleMultiplier = Vector3.Distance(handCenter, gameObject.transform.position) /
                                          initHandDistance;
                    gameObject.transform.localScale = new Vector3(
                        initGameobjectScale.x * (!disableScaleOnX ? scaleMultiplier : 1f), 
                        initGameobjectScale.y * (!disableScaleOnY ? scaleMultiplier : 1f),
                        initGameobjectScale.z * (!disableScaleOnZ ? scaleMultiplier : 1f));
                    //TODO workout scaling and moving object to keep offset to hand
                    //ScaleAround(gameObject, hands[pinchHandId].transform.position, newScale);
                    
                    
                    //move object to match scale when grabbed
                    /*if (rayCastHit.collider.gameObject.transform.parent.Equals(hands[pinchHandId].transform))
                    {
                        var distMultiplier =
                            Vector3.Distance(hands[pinchHandId].transform.position, gameObject.transform.position) /
                            initCubeToGrabHandDist;
                        gameObject.transform.position = Vector3.LerpUnclamped(hands[pinchHandId].transform.position, initCubePos,
                            distMultiplier);
                    }*/
                }
                else
                {
                    initValuesSet = false;
                }
            }
            else
            {
                DebugLineScript.Instance.setEnabled(false);
                initValuesSet = false;
            }
        }
        else
        {
            initValuesSet = false;
        }
    }

    private void SetupPalm(int handId)
    {
        palmRefObjs[handId].transform.SetParent(hands[handId].transform);
        //move point down or up depending on the hand cause right spawns with palm down and left with palm up
        var yOffset = handId == 0 ? resizeRangeThreshold : resizeRangeThreshold*-1;
        palmRefObjs[handId].transform.localPosition = new Vector3(0, yOffset, 0);
    }
    public void CheckIfHandIsPinching(int handId)
    {
        if (hands[handId].GetFingerPinchStrength(fingerToCheck) >= pinchThreshold)
        {
            initHandGrabId = handId;
            gameObject.transform.SetParent(hands[handId].transform);
        }
        else
        {
            initHandGrabId = -1;
            gameObject.transform.SetParent(initParent);
        }
    }

    private int GetHandId(Collider collider)
    {
        // all bonecolliders have "_CapsuleCollider" in name
        if (collider.gameObject.name.Contains("_CapsuleCollider"))
        {
            return collider.transform.IsChildOf(hands[1].transform) ? 1 : 0;
        }
        return -1;
    }

    private bool CheckForViableBone(Collider other)
    {
        OVRPlugin.BoneId boneId = OVRPlugin.BoneId.Invalid;
        // all collider from oculus api end with _CapsuleCollider
        if (other.gameObject.name.Contains("_CapsuleCollider"))
        {
            //get the name of the bone from the name of the gameobject, and convert it to an enum value
            string boneName = other.gameObject.name.Substring(0, other.gameObject.name.Length - 16);
            boneId = (OVRPlugin.BoneId) Enum.Parse(typeof(OVRPlugin.BoneId), boneName);
        }
        return viableBonesToDetect.Contains(boneId);
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (gameObject.layer.Equals(LayerMask.NameToLayer("Obj")) && CheckForViableBone(other))
        {
            var handId = GetHandId(other);
            
            Debug.Log(CheckIfHandAlreadyGrabbing(handId));
            //if the child count is higher than the init count the hand is already holding an object
            if (handId > -1 && !CheckIfHandAlreadyGrabbing(handId))
            {
                CheckIfHandIsPinching(handId);
            }
        }
        else if (!gameObject.layer.Equals(LayerMask.NameToLayer("Obj")))
        {
            initHandGrabId = -1;
            initValuesSet = false;
            gameObject.transform.parent = initParent;
        }
    }

    private bool CheckIfHandAlreadyGrabbing(int handId)
    {
        foreach (Transform child in hands[handId].transform)
        {
            if (child.gameObject.layer.Equals(LayerMask.NameToLayer("Obj")) && !child.gameObject.Equals(gameObject))
            {
                return true;
            }
        }
        return false;
    }

    /*private void OnTriggerExit(Collider collider)
    {
        if (CheckForViableBone(collider))
        {
            var handId = GetHandId(collider);
            if (handId > -1 && handId == initHandGrabId)
            {
                initHandGrabId =  -1;
            }
        }
    }*/
    
    /*public void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.transform.localPosition;
        Vector3 B = pivot;
 
        Vector3 C = A - B; // diff from object pivot to desired pivot/origin
 
        float RS = newScale.x / target.transform.localScale.x; // relataive scale factor
 
        // calc final position post-scale
        Vector3 FP = B + C * RS;
 
        // finally, actually perform the scale/translation
        target.transform.localScale = newScale;
        target.transform.localPosition = FP;
    }*/
}
