using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartScript : MonoBehaviour
{
    public OVRHand[] hands;
    public GameObject indicator;
    public GameObject highlight;
    public List<Canvas> selections;
    public LayerMask layer;
    public DebugLineScript lineRenderer;
    private int selected;
    private bool moveIndicator;
    private bool isPointing;
    private GameObject pointer;
    void Start()
    {
        HighlightSelected(2);
        pointer = new GameObject();
        pointer.transform.parent = hands[1].transform;
        pointer.transform.localPosition = new Vector3(-0.175f, 0, 0);
    }

    private void Update()
    {
        if (isPointing)
        {
            var start = pointer.transform.position;
            var direction = hands[1].PointerPose.forward;
            RaycastHit raycastHit = new RaycastHit();
            if (Physics.Raycast(start, direction, out raycastHit, Mathf.Infinity , layer))
            {
                lineRenderer.setStartAndEnd(start, raycastHit.point);
                lineRenderer.setEnabled(true);
                HighlightSelected(selections.IndexOf(raycastHit.collider.gameObject.GetComponent<Canvas>()));
            }
            else
            {
                lineRenderer.setEnabled(false);
            }
        }
    }

    public void HighlightSelected(int index)
    {
        if (index < selections.Count && index >= 0)
            selected = index;
        indicator.transform.SetParent(selections[selected].transform);
        indicator.transform.localPosition = new Vector3(0, indicator.transform.localPosition.y, 0);
        highlight.transform.SetParent(selections[selected].transform);
        highlight.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void ConfirmSelection()
    {
        switch (selected)
        {
            case 0:
                SceneManager.LoadScene("RockPaperScissor");
                break;
            case 1:
                SceneManager.LoadScene("Puzzlev2");
                break;
            case 2:
                SceneManager.LoadScene("CountingQuiz");
                break;
            case 3:
                SceneManager.LoadScene("Drawing");
                break;
        }
    }

    public void IsPointing(bool b)
    {
        isPointing = b;
    }
}
