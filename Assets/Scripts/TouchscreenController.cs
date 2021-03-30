using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
    public class TouchscreenController : MonoBehaviour {

        [SerializeField] private GameObject controlledCube = null;

        Transform cubeTransform;
 
        void GetCubeTransform(){
            GameObject go = GameObject.FindWithTag("TouchObject");
            if (go != null){
                cubeTransform = go.transform;
            } else {
                Debug.Log("TouchObject not found");
            }
        }

        private void Start() {   
            GetCubeTransform();
        }


        public void TransformControlledCube(InteractableStateArgs obj) {
            cubeTransform.localScale = new Vector3(3f, 3f, 3f);;
        }    
    }
}