using System;
using System.Collections;
using System.Collections.Generic;
using OVR.OpenVR;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DrawingScript : MonoBehaviour
{
    public Image canvasImage;
    public List<Sprite> sprites;
    public Image canvasWinIndicator;
    public List<Sprite> winIndicator;
    public Sprite secretImage;
    // Start is called before the first frame update
    void Start()
    {
        RandomizeImage();
        canvasWinIndicator.enabled = false;
    }

    public void CompareDrawing(string recognizedDrawing)
    {
        canvasWinIndicator.enabled = false;
        if (canvasImage.sprite.name.Equals(recognizedDrawing))
        {
            SetWinIndicator(0);
        }
        else
        {
            SetWinIndicator(1);
        }
    }
    
    public void RandomizeImage()
    {
        canvasImage.enabled = true;
        canvasImage.sprite = sprites[Random.Range(0, sprites.Count)];
    }

    public void SetWinIndicator(int index)
    {
        canvasWinIndicator.enabled = true;
        canvasWinIndicator.sprite = winIndicator[index];
        canvasImage.enabled = false;
        StartCoroutine(DelayRoutine(2f, DisableAndRandomize));
    }

    private IEnumerator DelayRoutine(float delay, Action actionToDo)
    {
        yield return new WaitForSeconds(delay);
        actionToDo.Invoke();
    }

    public void DisableWinIndicator()
    {
        canvasWinIndicator.enabled = false;
    }

    public void DisableAndRandomize()
    {
        RandomizeImage();
        DisableWinIndicator();
    }

    public void Secret()
    {
        canvasImage.sprite = secretImage;
        StartCoroutine(DelayRoutine(3f, RandomizeImage));
    }
}
