using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scan : MonoBehaviour
{
    public GameObject scanCam;
    public Texture emptySprite;
    public Scan otherSide;
    public bool isEmpty = true;
    public string textureInputName = "_BaseMap";

    Renderer rend; 
    Renderer rend_object2; 
    CameraSnap cameraSnap;
    public FiducialController fiducialController;
    public GameObject additional_Object;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if(additional_Object != null)
            rend_object2 = additional_Object.GetComponent<Renderer>();

        cameraSnap = scanCam.GetComponent<CameraSnap>();
        if (fiducialController == null)
            fiducialController = GetComponent<FiducialController>();

        ScanCamSize scanCamSize = scanCam.GetComponent<ScanCamSize>();
        if (scanCamSize != null)
            transform.localScale *= scanCamSize.scaleMultiplier;

        //to thest if it finds the texture
        //SetEmpty();
    }
    public void SetEmpty() { 
        rend.material.SetTexture(textureInputName, emptySprite);
        if(rend_object2 != null)
            rend_object2.material.SetTexture(textureInputName, emptySprite);
        isEmpty = true;
    }
    private void Update()
    {
        bool fiducialVisible = true;
        if(fiducialController != null)
            fiducialVisible = fiducialController.m_IsVisible;

        if (isEmpty == true && fiducialVisible && scanCam.activeSelf)
        {
            isEmpty = false;
            if(otherSide != null)
                otherSide.SetEmpty();
            ScanDrawing();
        }
    }
    private void ScanDrawing()
    {
        cameraSnap.LoadNew(gameObject);
//         if(additional_Object != null)
//             cameraSnap.LoadNew(additional_Object);

        cameraSnap.StartSnapping();
    }
}
