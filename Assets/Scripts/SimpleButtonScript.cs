using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class SimpleButtonScript : MonoBehaviour
{   
    public UnityEvent onPushed;
    public UnityEvent onReleased;
    public List<OVRPlugin.BoneId> boneIdToPress;
    private Renderer buttonRenderer;

    private void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        buttonRenderer.material.color = Color.blue;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (CheckForCorrectBoneCollider(collider))
        {
            buttonRenderer.material.color = Color.red;
            onPushed.Invoke();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (CheckForCorrectBoneCollider(collider))
        {
            buttonRenderer.material.color = Color.blue;
            onReleased.Invoke();
        }
    }

    private Boolean CheckForCorrectBoneCollider(Collider collider)
    {
        OVRPlugin.BoneId boneId = OVRPlugin.BoneId.Invalid;
        // all collider from oculus api end with _CapsuleCollider
        if (collider.gameObject.name.Contains("_CapsuleCollider"))
        {
            //get the name of the bone from the name of the gameobject, and convert it to an enum value
            string boneName = collider.gameObject.name.Substring(0, collider.gameObject.name.Length - 16);
            boneId = (OVRPlugin.BoneId) Enum.Parse(typeof(OVRPlugin.BoneId), boneName);
        }
        return boneIdToPress.Contains(boneId);
    }
}
