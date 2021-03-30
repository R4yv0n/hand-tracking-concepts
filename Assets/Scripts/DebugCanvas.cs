using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class DebugCanvas : MonoBehaviour
{
    public static DebugCanvas Instance;
    public Text[] textfields;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public Text GetTextField(int index)
    {
        return textfields[index];
    } 
}
