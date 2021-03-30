using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Collider))]
public class ObjectResize : MonoBehaviour
{
    public ObjDetectorScript objDetector;
    public bool disableScaleOnX = false;
    public bool disableScaleOnY = false;
    public bool disableScaleOnZ = false;
    private Transform objTrans;
    private OVRHand[] hands;
    private Renderer[] handsRenderer;
    private Boolean leftHandResizeMode = false;
    private Boolean rightHandResizeMode = false;
    private Boolean cubeIsBetweenHands = false;
    private Vector3 initRotationDiffEuler;
    private float initHandDistance = 1f;
    private Vector3 initScale;
    private Boolean playerIsGrabbing = false;
    // Start is called before the first frame update
    void Start()
    {
        objTrans = GetComponent<Transform>();
        hands = new OVRHand[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRHandPrefabL").GetComponent<OVRHand>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefabR").GetComponent<OVRHand>()
        };
        handsRenderer = new Renderer[]
        {
            hands[0].GetComponent<Renderer>(),
            hands[1].GetComponent<Renderer>()
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (cubeIsBetweenHands)
        {
            //Set position of the cube to keep it in the middle of the two hands
            objTrans.position = handsRenderer[1].bounds.center +
                                 (handsRenderer[0].bounds.center - handsRenderer[1].bounds.center) / 2;

            //Set scale of cube depending on the distance between hands
            var scaleMultiplier = Vector3.Distance(handsRenderer[1].bounds.center, handsRenderer[0].bounds.center) /
                                  initHandDistance;
            objTrans.localScale = new Vector3(
                initScale.x * (!disableScaleOnX ? scaleMultiplier : 1f), 
                initScale.y * (!disableScaleOnY ? scaleMultiplier : 1f),
                initScale.z * (!disableScaleOnZ ? scaleMultiplier : 1f));
        }
    }

    public void CubeIsBetweenHands(Boolean isBetween)
    {
        //initial distance between hands and scale of Cube is needed to scale the cube 
        initHandDistance = Vector3.Distance(handsRenderer[1].bounds.center, handsRenderer[0].bounds.center);
        initScale = objTrans.transform.localScale;
        
        cubeIsBetweenHands = isBetween;
    }

    private Boolean IsHandGrabbing(int handId)
    {
        return objDetector.GetHandState(handId);
    }

    private bool CheckIfOtherObjectsAreGrabbed(int handId)
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("TouchObject"))
        {
            if (obj.transform.parent && obj.transform.parent.Equals(hands[handId].transform))
            {
                return true;
            }
        }
        return false;
    }
    private void OnTriggerStay(Collider collider)
    {
        if (gameObject.layer.Equals(LayerMask.NameToLayer("Obj")) && collider.gameObject.name.Contains("_CapsuleCollider"))
        {
            //get the name of the bone from the name of the gameobject, and convert it to an enum value
            string boneName = collider.gameObject.name.Substring(0, collider.gameObject.name.Length - 16);
            OVRPlugin.BoneId boneId = (OVRPlugin.BoneId) Enum.Parse(typeof(OVRPlugin.BoneId), boneName);
            
            // collider is index finger
            if (boneId == OVRPlugin.BoneId.Hand_Index3)
            {
                var handId = collider.transform.IsChildOf(hands[1].transform) ? 1 : 0;
                if (IsHandGrabbing(handId) && !CheckIfOtherObjectsAreGrabbed(handId))
                {
                    // set parent of obj to rotate and move depending on hand
                    objTrans.SetParent(hands[handId].transform);
                }
            }
        }
    }
}
