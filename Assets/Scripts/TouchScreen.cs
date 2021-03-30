using System;
using System.Collections;
using System.Collections.Generic;
using OculusSampleFramework;
using UnityEngine;
using UnityEngine.Assertions;

public class TouchScreen : Interactable
{

    private Transform _cubeTransform;
    private Renderer _cubeRenderer;
    private Transform _touchTransform;
    private Renderer _touchRender;

    void GetCubeTransform(){
        GameObject go = GameObject.FindGameObjectWithTag("TouchObject");
        if (go != null){
            _cubeTransform = go.GetComponent<Transform>();
        } else {
            Debug.Log("TouchObject not found");
        }
    }
    
    void GetCubeRenderer(){
        GameObject go = GameObject.FindWithTag("TouchObject");
        if (go != null){
            _cubeRenderer = go.GetComponent<Renderer>();
        } else {
            Debug.Log("TouchObject not found");
        }
    }

    protected void Start()
    {
        Debug.Log("Logger funzt");
        GetCubeTransform();
        GetCubeRenderer();
        _touchTransform = GameObject.FindWithTag("TouchScreen").GetComponent<Transform>();
        _touchRender = GameObject.FindGameObjectWithTag("TouchScreen").GetComponent<Renderer>();
    }

    protected override void Awake() {
        Debug.Log("Logger fuzt");
        GetCubeTransform();
        GetCubeRenderer();
        _touchTransform = GameObject.FindWithTag("TouchScreen").GetComponent<Transform>();
        _touchRender = GameObject.FindGameObjectWithTag("TouchScreen").GetComponent<Renderer>();
	}

    public override void UpdateCollisionDepth(InteractableTool interactableTool,
        InteractableCollisionDepth oldCollisionDepth,
        InteractableCollisionDepth newCollisionDepth)
        {
            _cubeRenderer.material.color = Color.black;
            _touchRender.material.color = Color.black;
            _touchTransform.localScale = new Vector3(100f, 1f, 0.05f);
            if (interactableTool.IsRightHandedTool && newCollisionDepth == InteractableCollisionDepth.Action || oldCollisionDepth == InteractableCollisionDepth.Action)
            {
                GetCubeTransform();
                GetCubeRenderer();
                _cubeTransform.localScale = new Vector3(5f, 5f, 5f);
                Debug.Log("Touched");
                _cubeRenderer.material.color = Color.red;
                _touchRender.material.color = Color.red;
                _touchTransform.localScale = new Vector3(1f, 1f, 0.05f);
            }
            else
            {
                GetCubeTransform();
                GetCubeRenderer();
                _cubeTransform.localScale = new Vector3(1f, 1f, 1f);
                _cubeTransform.Rotate(15.0f, 0.0f, 0.0f, Space.Self);
                Debug.Log("NotTouched");
                _cubeRenderer.material.color = Color.blue;
                _touchRender.material.color = Color.blue;
            }
        }
}

