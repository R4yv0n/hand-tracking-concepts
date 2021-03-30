using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public LineRenderer LaserPrefab;
    public Transform playerTrans;
    public Transform playerHandL;
    public OVRHand playerHandR;
    public Transform playerHandRTrans;
    public LayerMask layerMask;
    public CharacterController characterController;
    private LineRenderer aimLaser;
    private Vector3 destinationPoint;
    private Boolean currentDestinationIsValid;

    void Awake()
    {
        LaserPrefab.gameObject.SetActive(false);
        aimLaser = Instantiate(LaserPrefab);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (aimLaser.gameObject.active)
        {
            aimLaser.material.color = CheckForValidTarget() ? Color.green : Color.red;
        }
    }

    public void InitiateTeleport()
    {
        aimLaser.gameObject.SetActive(true);
    }
    
    public void TeleportToDestination()
    {
        if (currentDestinationIsValid)
        {
            var character = characterController;
            var characterTrans = character.transform;
            destinationPoint.y += character.height + 100;
            characterTrans.position = destinationPoint;
        }
        aimLaser.gameObject.SetActive(false);
    }
    
    public void CancelTeleport()
    {
        aimLaser.gameObject.SetActive(false);
    }

    void UpdateAimData(Vector3 start, Vector3 end)
    {
        aimLaser.SetPosition(0, start);
        aimLaser.SetPosition(1, end);
    }

    Boolean CheckForValidTarget()
    {
        var rayCastHit = new RaycastHit();
        if (Physics.Raycast(playerHandRTrans.position, playerHandR.PointerPose.forward, out rayCastHit, 100f, layerMask))
        {
            
            destinationPoint = rayCastHit.point;
            UpdateAimData(playerHandRTrans.position, destinationPoint);
            var character = characterController;
            destinationPoint.y += character.height;
            VRDebugger.Instance.Log(rayCastHit.point + " - " +  destinationPoint + " - " + character.transform.position);
            currentDestinationIsValid = true;
            return true;
        }
        UpdateAimData(playerHandRTrans.position, playerHandR.PointerPose.forward*1000000);
        currentDestinationIsValid = false;
        return false;
    }
}
