using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to render object only in Debug Mode 
/// </summary>
/// 
public class debugModeCheck : MonoBehaviour
{
    void Start()
    {
        if (!Debug.isDebugBuild)
        {
            GetComponent<Renderer>().enabled = false;
            enabled = false;
        }
    }
}
