using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Transform _playerTrans;
    private OVRHand[] _hands;
    private Transform _leftHandMiddle1BoneTran;
    private OVRBone _leftHandMiddle1Bone;
    private Transform _leftHandMiddle3BoneTran;
    private OVRBone _leftHandMiddle3Bone;
    private Boolean _isTeleAiming;
    private DebugLineScript _lineScript;
    private Transform _pointerTrans;
    private DebugLineScript _debugLineScript;
    
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _playerTrans = GetComponent<Transform>();
            _hands = new OVRHand[]
            {
                GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRHandPrefab").GetComponent<OVRHand>(),
                GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefab").GetComponent<OVRHand>()
            };
            _leftHandMiddle1Bone = _hands[0].GetComponent<OVRSkeleton>().Bones
                .Where(it => it.Id == OVRSkeleton.BoneId.Hand_Middle1).First();
            _leftHandMiddle3Bone = _hands[0].GetComponent<OVRSkeleton>().Bones
                .Where(it => it.Id == OVRSkeleton.BoneId.Hand_Middle3).First();
            _isTeleAiming = false;
            _lineScript = GameObject.FindWithTag("TeleportLine").GetComponent<DebugLineScript>();
            _pointerTrans = GameObject.FindWithTag("Pointer").GetComponent<Transform>();
            _pointerTrans.forward = Quaternion.Euler(0, 90, 0) * _pointerTrans.forward;
            _debugLineScript = GameObject.FindWithTag("DebugLine").GetComponent<DebugLineScript>();
            VRDebugger.Instance.Log(_debugLineScript.ToString());
            _debugLineScript.setStartAndEnd(_pointerTrans.position, new Vector3(0, 0, 0));
            _debugLineScript.setEnabled(true);
            _debugLineScript.setColor(Color.red);
            }
        catch (Exception e)
        {
            VRDebugger.Instance.Log(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (_hands[1].GetFingerIsPinching(OVRHand.HandFinger.Middle) && !_isTeleAiming)
            {
                _isTeleAiming = true;
            }

            if (_isTeleAiming)
            {
                var rayCastHit = new RaycastHit();
                var destinationPos = _playerTrans.position;
                var ray = new Ray(_pointerTrans.position, _pointerTrans.forward);
                if (Physics.Raycast(ray, out rayCastHit, Mathf.Infinity, LayerMask.NameToLayer("Teleport"))) 
                {
                    //VRDebugger.Instance.Log(rayCastHit.collider.name);
                    destinationPos = rayCastHit.point;
                    _lineScript.setStartAndEnd(_pointerTrans.position, rayCastHit.point);
                    _lineScript.setEnabled(true);
                }
                else
                {
                    //_lineScript.setEnabled(false);
                }
                if (!_hands[1].GetFingerIsPinching(OVRHand.HandFinger.Middle))
                {
                    _playerTrans.position = new Vector3(destinationPos.x, _playerTrans.position.y, destinationPos.z);
                    _isTeleAiming = false;
                    //_lineScript.setEnabled(false);
                }
            }
        }
        catch (Exception e)
        {
            VRDebugger.Instance.Log(e);
        }
    }
}
