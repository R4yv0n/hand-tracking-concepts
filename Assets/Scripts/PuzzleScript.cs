using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PuzzleScript : MonoBehaviour
{
    public List<GameObject> puzzleParts;
    public Collider randomPosArea;
    public float minRandoScale;
    public float maxRandoScale;
    public float positionThreshold;
    public float scaleThreshold;
    public float rotationThreshold;
    public UnityEvent onFinished;
    private List<Vector3> originalPuzzlePos = new List<Vector3>();
    private List<Vector3> originalPuzzleScale = new List<Vector3>();
    private List<Vector3> originalPuzzleScaleLossless = new List<Vector3>();
    private List<Quaternion> originalPuzzleRot = new List<Quaternion>();
    private List<GameObject> unusedParts = new List<GameObject>();
    private Transform initParent;

    /*
    // To figure out the right theshold values
    // values doing testing while trying to fit in a part:
    // DistanceAvg: 0.03776955
    // RotationAvg: 10.45492
    // ScaleAvg: 0.971984
    // Scale without parent changing: 100 to 110
    public List<float> debugDistance;                  
    public List<float> debugRotMag;
    public List<float> debugScaleMag;
    public float debugAvgDist;
    public float debugAvgRot;
    public float debugAvgScale;
    

    private void Update()
    {
        
        //Threshold testing
        debugAvgDist = debugDistance.Average();
        debugAvgRot = debugRotMag.Average();
        debugAvgScale = debugScaleMag.Average();
        
        
    }
    */

    void Start()
    {
        /*
        //Threshold testing
        debugDistance.Add(1f);
        debugRotMag.Add(1f);
        debugScaleMag.Add(1f);
        */
        
        foreach (var p in puzzleParts)
        {
            //Save original transform values to check for correct fitting pieces
            var pTrans = p.GetComponent<Transform>();
            originalPuzzlePos.Add(pTrans.position);
            originalPuzzleRot.Add(pTrans.rotation);
            originalPuzzleScale.Add(pTrans.lossyScale);
            //also save local scale cause lossy is to inaccurate put the pieaces perfectly back
            originalPuzzleScaleLossless.Add(pTrans.localScale);
        }
        initParent = puzzleParts.First().transform.parent;

        RandomizePuzzle();
    }

    void RandomizePuzzle()
    {
        var partToSkip = Random.Range(0, puzzleParts.Count);
        unusedParts.Clear();
        for (int i = 0; i < puzzleParts.Count; i++)
        {
            if (partToSkip != i)
            {
                var partTrans = puzzleParts[i].GetComponent<Transform>();
                var spawnPoint = new Vector3(
                    Random.Range(randomPosArea.bounds.min.x, randomPosArea.bounds.max.x),
                    Random.Range(randomPosArea.bounds.min.y, randomPosArea.bounds.max.y),
                    Random.Range(randomPosArea.bounds.min.z, randomPosArea.bounds.max.z));
                partTrans.position = spawnPoint;
                partTrans.rotation = Random.rotation;
                var randomScale = Random.Range(minRandoScale, maxRandoScale);
                partTrans.localScale = new Vector3(randomScale, randomScale, randomScale);
                puzzleParts[i].layer = LayerMask.NameToLayer("Obj");
                unusedParts.Add(puzzleParts[i]);
            }
            else
            {
                puzzleParts[i].layer = LayerMask.NameToLayer("Default");
            }
        }
    }
    
    void CheckPuzzlePieceFitting(GameObject part)
    {
        var partIndex = puzzleParts.IndexOf(part);
        var partTrans = part.GetComponent<Transform>();
        /*
        //Threshold testing
        debugDistance.Add(Vector3.Distance(originalPuzzlePos[partIndex], partTrans.position));
        debugRotMag.Add(Quaternion.Angle(originalPuzzleRot[partIndex], partTrans.rotation));
        debugScaleMag.Add(Math.Abs(originalPuzzleScale[partIndex].magnitude / partTrans.lossyScale.magnitude));
        */
        if (Vector3.Distance(originalPuzzlePos[partIndex], partTrans.position) <= positionThreshold &&
            Math.Abs(originalPuzzleScale[partIndex].magnitude / partTrans.lossyScale.magnitude) <= scaleThreshold &&
            Quaternion.Angle(originalPuzzleRot[partIndex], partTrans.rotation) <= rotationThreshold)
        {
            part.layer = LayerMask.NameToLayer("Default");
            part.GetComponent<DebugLineScript>().setEnabled(false);
            partTrans.SetParent(initParent);
            partTrans.localScale = originalPuzzleScaleLossless[partIndex];
            partTrans.position = originalPuzzlePos[partIndex];
            partTrans.rotation = originalPuzzleRot[partIndex];
            unusedParts.Remove(part);
        }

        if (unusedParts.Count == 0)
        {
            onFinished?.Invoke();
        }
    }
    
    // will be called by button
    public void ResetPuzzle()
    {
        foreach (var part in puzzleParts)
        {
            var partIndex = puzzleParts.IndexOf(part);
            part.transform.SetParent(initParent);
            part.transform.localScale = originalPuzzleScaleLossless[partIndex];
            part.transform.position = originalPuzzlePos[partIndex];
            part.transform.rotation = originalPuzzleRot[partIndex];
            part.layer = LayerMask.NameToLayer("Obj");
            part.GetComponent<Collider>().enabled = true;
        }
        RandomizePuzzle();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Obj")))
        {
            CheckPuzzlePieceFitting(collider.gameObject);
        }
    }
}


