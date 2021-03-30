using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AnswerEvaluatorScript : MonoBehaviour
{
    public OVRHand[] hands;
    public GestureDetection[] gestureDetectors;
    public Text answerText;
    public Collider spawnArea;
    private bool answerLocked = false;
    private bool[] handCountDetected = {false, false};
    private int[] handCount = {0, 0};
    private int answer;
    private List<GameObject> objectsToCount = new List<GameObject>();
    private int objectsToCountAmount = 0;
    // Start is called before the first frame update
    void Start()
    {
        var transparentRed = Color.red;
        transparentRed.a = 0.3f;
        GetComponent<Renderer>().material.color = transparentRed;
        
        InitObjects();
        SpawnRandomObjects();
        SetStartText();
    }
    

    // Update is called once per frame
    void Update()
    {
        Debug.Log("AnswerEval bools: " + handCountDetected[0] + handCountDetected[1] +
                  (handCountDetected[0] || handCountDetected[1]));
        if (handCountDetected[0] || handCountDetected[1])
        {
            var answerL = 0;
            var answerR = 0;
            
            if (handCount[0] > -1)
            {
                answerL = handCount[0];
            }
            if (handCount[1] > -1)
            {
                answerR = handCount[1];
            }
            
            answer = answerL + answerR;
            answerText.text = "Your Answer is: " + answer + "\n Remove your hands from the detection area to lock in your answer.";
        }

        if (answerLocked)
        {
            if (answer == objectsToCountAmount)
            {
                answerText.text = "You're correct. There are " + objectsToCountAmount + " spheres.";
            }
            else
            {
                answerText.text = "You're wrong. There are " + objectsToCountAmount + " spheres. \n Press the reset button to try again.";
            }
        }
    }

    public void SetHandCount(int handId)
    {
        handCountDetected[handId] = handCount[handId] > -1;
        int.TryParse(gestureDetectors[handId].currentGesture.name, out handCount[handId]);
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        var handId = GetHandId(collider, OVRPlugin.BoneId.Hand_WristRoot);
        if (handId > -1)
        {
            //Debug.Log("Detector: " + handId + " active");
            answerLocked = false;
            var transparentBlue = Color.blue;
            transparentBlue.a = 0.3f;
            GetComponent<Renderer>().material.color = transparentBlue;
            gestureDetectors[handId].SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        var handId = GetHandId(collider, OVRPlugin.BoneId.Hand_WristRoot);
        if (handId > -1)
        {
            //Debug.Log("Detector: " + handId + " deactivated");
            var transparentRed = Color.red;
            transparentRed.a = 0.3f;
            GetComponent<Renderer>().material.color = transparentRed;
            gestureDetectors[handId].SetActive(false);
            gestureDetectors[handId].StopTimer();

            if (answer > 0)
            {
                answerLocked = true;
            }
        }
    }

    private int GetHandId(Collider collider, OVRPlugin.BoneId boneIdToCheck)
    {
        OVRPlugin.BoneId boneId = OVRPlugin.BoneId.Invalid;
        // all collider from oculus api end with _CapsuleCollider
        if (collider.gameObject.name.Contains("_CapsuleCollider"))
        {
            //get the name of the bone from the name of the gameobject, and convert it to an enum value
            string boneName = collider.gameObject.name.Substring(0, collider.gameObject.name.Length - 16);
            boneId = (OVRPlugin.BoneId) Enum.Parse(typeof(OVRPlugin.BoneId), boneName);

            if (boneId == boneIdToCheck)
            {
                if (collider.transform.IsChildOf(hands[0].transform))
                {
                    return 0;
                } 
                if (collider.transform.IsChildOf(hands[1].transform))
                {
                    return 1;
                }
            }
        }
        return -1;
    }

    private void InitObjects()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            sphere.transform.SetParent(spawnArea.transform);
            sphere.GetComponent<Renderer>().material.color = Color.blue;
            sphere.GetComponent<MeshRenderer>().enabled = false;
            objectsToCount.Add(sphere);
        }
    }
    private void SpawnRandomObjects()
    {
        objectsToCountAmount = Random.Range(1, 10);

        for (int i = 0; i < objectsToCountAmount; i++)
        {
            objectsToCount[i].GetComponent<MeshRenderer>().enabled = true;
            var spawnPoint = new Vector3(
                Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y), // was min.z and max.y
                Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)); // way min.y and min.y   reminder in case spawn area is fucked up -----------------------------------------
            objectsToCount[i].transform.position = spawnPoint;
        }
    }

    private void SetStartText()
    {
        answerText.text = "Put your hands in the red zone enter your answer \n " +
                     "and wait for the respective timer to lock your answer \n" +
                     "the amount will be both hand's sum";
    }

    private void ResetCountObjects()
    {
        foreach (var obj in objectsToCount)
        {
            obj.GetComponent<MeshRenderer>().enabled = false;
        }
        SpawnRandomObjects();
    }

    public void ResetGame()
    {
        answerLocked = false;
        handCountDetected[0] = false;
        handCountDetected[1] = false;
        handCount[0] = 0;
        handCount[1] = 0;
        ResetCountObjects();
        SetStartText();
    }
}
