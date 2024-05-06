using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnLateUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (GetComponent<Renderer>())
            GetComponent<Renderer>().enabled = false;
    }
}
