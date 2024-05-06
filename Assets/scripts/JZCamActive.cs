using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JZCamActive : MonoBehaviour
{
    FiducialController fiducialController;
    public JZSOCameraActive jZSOCameraActive;
    // Start is called before the first frame update
    void Start()
    {
        fiducialController = GetComponent<FiducialController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fiducialController.m_IsVisible) jZSOCameraActive.camIsActive = true;
        else jZSOCameraActive.camIsActive = false;
    }
}
