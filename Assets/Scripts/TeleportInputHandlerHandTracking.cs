using System;
using UnityEngine;

public class TeleportInputHandlerHandTracking : TeleportInputHandler {
    public Transform leftHand;
    public Transform rightHand;
    public GestureDetection gestureDetection;
    public bool fastTeleport = false;

    public override LocomotionTeleport.TeleportIntentions GetIntention()
	{
		if (!isActiveAndEnabled)
		{
			return LocomotionTeleport.TeleportIntentions.None;
		}
		
		if (LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
		{
			/*if (gestureDetection.GetTeleportGestureDetected())
			{
				return fastTeleport ? LocomotionTeleport.TeleportIntentions.Teleport : LocomotionTeleport.TeleportIntentions.PreTeleport;
			}*/
		}
		
		/*if (LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport && gestureDetection.GetTeleportGestureReleased())
		{
			return LocomotionTeleport.TeleportIntentions.Teleport;
		}*/

		if (LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
		{
			return LocomotionTeleport.TeleportIntentions.PreTeleport;
		}

		return LocomotionTeleport.TeleportIntentions.None;
	}
    
    public override void GetAimData(out Ray aimRay)
    {
	    /*OVRInput.Controller sourceController = AimingController;
	    if(sourceController == OVRInput.Controller.Touch)
	    {
		    sourceController = InitiatingController;
	    }
	    Transform t = (sourceController == OVRInput.Controller.LTouch) ? LeftHand : RightHand;*/
	    aimRay = new Ray(rightHand.position, rightHand.forward);
    }
}
