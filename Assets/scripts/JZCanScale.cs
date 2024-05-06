using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JZCanScale : MonoBehaviour
{
    public bool only_scale_if_JZSO_Camera_Active = true;
    public float sizeMin = -9999f;
    public float sizeMax = 9999f;

    public JZSOCameraActive jZSOCameraActive;
    FiducialController fidu;
    public float scaleFactor;
    public Transform[] otherOnes;

    void Start()
    {
        fidu = GetComponent<FiducialController>();
    }

    void Update()
    {
        if (fidu.IsVisible && (!only_scale_if_JZSO_Camera_Active || jZSOCameraActive.camIsActive))
        {
            Vector3 scale = transform.localScale;
            if (fidu.RotationSpeed > 0)
            {
                transform.localScale += new Vector3(
                    scale.x * fidu.RotationSpeed * scaleFactor / 10,
                    scale.y * fidu.RotationSpeed * scaleFactor / 10, 
                    scale.z/* * fidu.RotationSpeed * scaleFactor / 10*/);
                transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, sizeMin, sizeMax), Mathf.Clamp(transform.localScale.y, sizeMin, sizeMax), Mathf.Clamp(transform.localScale.z, sizeMin, sizeMax));
            }
            if (fidu.RotationSpeed < 0)
            {
                transform.localScale += new Vector3(
                    scale.x * fidu.RotationSpeed * scaleFactor / 10,
                    scale.y * fidu.RotationSpeed * scaleFactor / 10,
                    scale.z/* * fidu.RotationSpeed * scaleFactor / 10*/);
                transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, sizeMin, sizeMax), Mathf.Clamp(transform.localScale.y, sizeMin, sizeMax), Mathf.Clamp(transform.localScale.z, sizeMin, sizeMax));    
            }
        }
    }
}
