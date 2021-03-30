using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLineScript : MonoBehaviour
{
    public static DebugLineScript Instance;
    public GameObject StarObj,EndObj;
    private LineRenderer lineRenderer;
    
    void Awake()
    {
        Instance = this;
    }

// Use this for initialization
    void Start () {
        lineRenderer = GetComponent<LineRenderer> ();
        lineRenderer.enabled = false;
    }

    public void setStartAndEnd(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition (0, start);
        lineRenderer.SetPosition (1, end);
    }

    public void setEnabled(Boolean enabled)
    {
        lineRenderer.enabled = enabled;
    }
    
    public void setColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}
