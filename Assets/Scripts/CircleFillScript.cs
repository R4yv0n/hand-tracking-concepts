using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleFillScript : MonoBehaviour
{
    public Canvas imageParent;
    public Image circleImage;
    public Image circleImageFill;
    [SerializeField] [Range(0, 1)] private float progress = 0f;

    private void Start()
    {
        circleImage.enabled = false;
        circleImageFill.enabled = false;
    }

    void Update()
    {
        circleImageFill.fillAmount = progress;
        if (progress <= 0)
        {
            circleImage.enabled = false;
            circleImageFill.enabled = false;
        }
        else
        {
            circleImage.enabled = true;
            circleImageFill.enabled = true;
        }
    }

    public void SetProgress(float value)
    {
        progress = value;
    }

    public void StartProgress()
    {
        circleImage.enabled = true;
        circleImageFill.enabled = true;
        progress = 0f;
    }

    public void StopProgress()
    {
        circleImage.enabled = false;
        circleImageFill.enabled = false;
        progress = 0f;
    }

    public void SetParent(Transform parentTrans)
    {
        imageParent.transform.SetParent(parentTrans);
        imageParent.transform.localPosition = new Vector3(0,0,0);
        imageParent.transform.localRotation = Quaternion.Euler(90,0, 0);
    }
}
