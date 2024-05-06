using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class ScanCamSize : MonoBehaviour
{
    new Camera camera;
    public GameObject LinksOnder, RechtsBoven, LinksBoven, RechtsOnder;
    float camSize;
    public float growRate;
    public float scaleMultiplier = 1.0f;
    private LineRenderer lineRenderer;
    Vector3[] positions = new Vector3[4];
    FiducialController fiducialController;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        camera = GetComponent<Camera>();
        camera.orthographicSize *= scaleMultiplier;
        fiducialController = GetComponent<FiducialController>();
        ResetBorders();
    }

    // Update is called once per frame
    void Update()
    {
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 1f, 8f);
        if (camSize != camera.orthographicSize) ResetBorders();
        if (Input.GetKeyDown(KeyCode.Z)) ResetBorders();
        if(fiducialController.RotationSpeed > 0) camera.orthographicSize += growRate;
        if (fiducialController.RotationSpeed < 0) camera.orthographicSize -= growRate;
    }
    
    private void ResetBorders()
    {
        camSize = camera.orthographicSize;
        GetCameraCornerPositions(positions);
        SetCornerPositions(positions);
        RenderLines(positions);

    }

    private void GetCameraCornerPositions(Vector3[] positions)
    {
        positions[0] = camera.ScreenToWorldPoint(new Vector3(0, 0, 13)); //LnksOnder
        positions[2] = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, 13)); //Rechtsboven
        positions[1] = camera.ScreenToWorldPoint(new Vector3(0, camera.pixelHeight, 13)); //LnksBoven
        positions[3] = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, 0, 13)); //Rechtsboven
    }

    private void SetCornerPositions(Vector3[] positions)
    {
        LinksOnder.transform.position = positions[0];
        LinksBoven.transform.position = positions[1];
        RechtsBoven.transform.position = positions[2];
        RechtsOnder.transform.position = positions[3];
    }

    private void RenderLines(Vector3[] points)
    {
        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .1f;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        lineRenderer.loop = true;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }
}


  