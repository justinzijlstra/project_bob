using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class webcamDevices : MonoBehaviour
{
    // Gets the list of devices and prints them to the console.
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name + i);

    }
}
