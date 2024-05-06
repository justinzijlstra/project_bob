/*
Copyright (c) 2012 André Gröschel

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FiducialController : MonoBehaviour
{
    public int MarkerID = 0;

    public enum RotationAxis { Forward, Back, Up, Down, Left, Right };

    //translation
    public bool IsPositionMapped = false;
    public bool InvertX = false;
    public bool InvertY = false;
    public bool grounded = false;
    public bool templateCam = false;
    public bool full = false;
    
    //rotation
    public bool IsRotationMapped = false;
    public bool UseRotation = false;
    public bool AutoHideGO = false;
    private bool m_ControlsGUIElement = false;
    public float DegreesToAction;
    float lastDegree, eulerDegree;
    float rotationDifference;
    float totalRotation;
    public float hideDelay = 1f;

    public float CameraOffset = 10;
    public RotationAxis RotateAround = RotationAxis.Back;
    private UniducialLibrary.TuioManager m_TuioManager;
    private Camera m_MainCamera;
    public float camY;
    public float camX;
    [Serializable]
    public class ExampleEvent : UnityEvent { }

    public ExampleEvent onRotateForward = new ExampleEvent();
    public ExampleEvent onRotateBackward = new ExampleEvent();

    private float JZAxisZ;

    //members
    public Vector2 m_ScreenPosition;
    private Vector3 m_WorldPosition;
    private Vector2 m_Direction;
    private float m_Angle;
    private float m_AngleDegrees;
    private float m_Speed;
    private float m_Acceleration;
    private float m_RotationSpeed;
    private float m_RotationAcceleration;
    public bool m_IsVisible = false;

    public float RotationMultiplier = 1;

    private Character m_characterComponent;

    private AudioSource m_audioSource;


    public virtual void Awake()
    {

#if UNITY_STANDALONE_LINUX
     InvertX = true;
     RotateAround = RotationAxis.Forward;
#endif

        JZAxisZ = transform.position.z;
        this.m_TuioManager = UniducialLibrary.TuioManager.Instance;
        //uncomment next line to set port explicitly (default is 3333)
        //tuioManager.TuioPort = 7777;

        this.m_TuioManager.Connect();

        //check if the game object needs to be transformed in normalized 2d space
        if (isAttachedToGUIComponent())
        {
            Debug.LogWarning("Rotation of GUIText or GUITexture is not supported. Use a plane with a texture instead.");
            this.m_ControlsGUIElement = true;
        }

        this.m_ScreenPosition = Vector2.zero;

        this.m_WorldPosition = Vector3.zero;
        this.m_Direction = Vector2.zero;
        this.m_Angle = 0f;
        this.m_AngleDegrees = 0;
        this.m_Speed = 0f;
        this.m_Acceleration = 0f;
        this.m_RotationSpeed = 0f;
        this.m_RotationAcceleration = 0f;
        this.m_IsVisible = false; //dit was true
    }

    public virtual void Start()
    {
        m_audioSource = GetComponent<AudioSource>();

        m_characterComponent = GetComponent<Character>();
        //get reference to main camera
        this.m_MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
   
        //check if the main camera exists
        if (this.m_MainCamera == null)
        {
            Debug.LogError("There is no main camera defined in your scene.");
        }
        GameObject manager = GameObject.FindGameObjectWithTag("Manager");
    }

    public virtual void Update()
    {
        if(UseRotation)GetRotation();
        if (this.m_TuioManager.IsConnected && this.m_TuioManager.IsMarkerAlive(MarkerID))
        {
 
            TUIO.TuioObject marker = this.m_TuioManager.GetMarker(MarkerID);

            //update parameters
            this.m_ScreenPosition.x = marker.getX();
            this.m_ScreenPosition.y = marker.getY();
            this.m_Angle = marker.getAngle() * RotationMultiplier;
            this.m_AngleDegrees = marker.getAngleDegrees() * RotationMultiplier;
            this.m_Speed = marker.getMotionSpeed();
            this.m_Acceleration = marker.getMotionAccel();
            this.m_RotationSpeed = marker.getRotationSpeed() * RotationMultiplier;
            this.m_RotationAcceleration = marker.getRotationAccel();
            this.m_Direction.x = marker.getXSpeed();
            this.m_Direction.y = marker.getYSpeed();
            this.m_IsVisible = true;
            //set game object to visible, if it was hidden before
            ShowGameObject();
            //update transform component
            UpdateTransform();
        }
        else //automatically hide game object when marker is not visible
        {
            if (this.AutoHideGO) StartCoroutine(HideDelay(hideDelay));
        }
    }

    IEnumerator HideDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        HideGameObject();
        yield return null;
    }


    void OnApplicationQuit(){if (this.m_TuioManager.IsConnected) this.m_TuioManager.Disconnect();}

    public virtual void UpdateTransform()
    {
        //position mapping
        if (this.IsPositionMapped)
        {
            //calculate world position with respect to camera view direction
            float xPos = this.m_ScreenPosition.x;
            float yPos = this.m_ScreenPosition.y;
            if (this.InvertX) xPos = 1 - xPos;
            if (this.InvertY) yPos = 1 - yPos;

            if (this.m_ControlsGUIElement) transform.position = new Vector3(xPos, 1 - yPos, 0);
            else
            {
                Vector3 position = new Vector3(xPos * Screen.width,
                    (1 - yPos) * Screen.height, this.CameraOffset);
                this.m_WorldPosition = this.m_MainCamera.ScreenToWorldPoint(position);
                //worldPosition += cameraOffset * mainCamera.transform.forward;
                this.m_WorldPosition = new Vector3(this.m_WorldPosition.x , this.m_WorldPosition.y, JZAxisZ);
                transform.position = this.m_WorldPosition;
            }
        }

        //rotation mapping
        if (this.IsRotationMapped)
        {
            Quaternion rotation = Quaternion.identity;

            switch (this.RotateAround)
            {
                case RotationAxis.Forward:
                    rotation = Quaternion.AngleAxis(this.m_AngleDegrees, Vector3.forward);
                    break;
                case RotationAxis.Back:
                    rotation = Quaternion.AngleAxis(this.m_AngleDegrees, Vector3.back);
                    break;
                case RotationAxis.Up:
                    rotation = Quaternion.AngleAxis(this.m_AngleDegrees, Vector3.up);
                    break;
                case RotationAxis.Down:
                    rotation = Quaternion.AngleAxis(this.m_AngleDegrees, Vector3.down);
                    break;
                case RotationAxis.Left:
                    rotation = Quaternion.AngleAxis(this.m_AngleDegrees, Vector3.left);
                    break;
                case RotationAxis.Right:
                    rotation = Quaternion.AngleAxis(this.m_AngleDegrees, Vector3.right);
                    break;
            }
            transform.rotation = rotation;
        }
    }

    private void GetRotation()
    {
        float eulers = m_AngleDegrees;
        if (eulers != eulerDegree)
        {
            rotationDifference = eulerDegree - eulers;
            if (Mathf.Abs(rotationDifference) < 50) totalRotation += rotationDifference;
            eulerDegree = eulers;

        }
        if (totalRotation > lastDegree + DegreesToAction)
        {
            lastDegree = totalRotation;
            onRotateForward.Invoke();
        }
        if (totalRotation < lastDegree - DegreesToAction)
        {
            lastDegree = totalRotation;
            onRotateBackward.Invoke();
        }
    }

    public virtual void ShowGameObject()
    {
		StopAllCoroutines();

        if (this.m_ControlsGUIElement)
        {
            //show GUI components
            if (gameObject.GetComponent<Text>() != null && !gameObject.GetComponent<Text>().enabled) gameObject.GetComponent<Text>().enabled = true;
            if (gameObject.GetComponent<Image>() != null && !gameObject.GetComponent<Image>().enabled) gameObject.GetComponent<Image>().enabled = true;
        }
        else
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            if (gameObject.GetComponent<Renderer>() != null && !gameObject.GetComponent<Renderer>().enabled) gameObject.GetComponent<Renderer>().enabled = true;
            if (gameObject.GetComponent<Camera>() != null && !gameObject.GetComponent<Camera>().enabled) gameObject.GetComponent<Camera>().enabled = true;
        }

        if (m_characterComponent != null)
            m_characterComponent.enabled = true;

        if (m_audioSource != null)
            m_audioSource.enabled = true;
        
    }
    public virtual void HideGameObject()
    {
        if (!AutoHideGO) return; 
        this.m_IsVisible = false;


        if (this.m_ControlsGUIElement)
        {
            //hide GUI components
            if (gameObject.GetComponent<Text>() != null && gameObject.GetComponent<Text>().enabled) gameObject.GetComponent<Text>().enabled = false;
            if (gameObject.GetComponent<Image>() != null && gameObject.GetComponent<Image>().enabled) gameObject.GetComponent<Image>().enabled = false;
        }
        else
        {
            //set 3d game object to visible, if it was hidden before
            foreach (Transform child in transform)  child.gameObject.SetActive(false);
            if (gameObject.GetComponent<Renderer>() != null && gameObject.GetComponent<Renderer>().enabled) gameObject.GetComponent<Renderer>().enabled = false;
            if (gameObject.GetComponent<Camera>() != null && gameObject.GetComponent<Camera>().enabled) gameObject.GetComponent<Camera>().enabled = false;
        }

        if (m_audioSource != null)
            m_audioSource.enabled = false;
    }
#region Getter

    public bool isAttachedToGUIComponent()
    {
        return (gameObject.GetComponent<Text>() != null || gameObject.GetComponent<Image>() != null);
    }
    public Vector2 ScreenPosition
    {
        get { return this.m_ScreenPosition; }
    }
    public Vector3 WorldPosition
    {
        get { return this.m_WorldPosition; }
    }
    public Vector2 MovementDirection
    {
        get { return this.m_Direction; }
    }
    public float Angle
    {
        get { return this.m_Angle; }
    }
    public float AngleDegrees
    {
        get { return this.m_AngleDegrees; }
    }
    public float Speed
    {
        get { return this.m_Speed; }
    }
    public float Acceleration
    {
        get { return this.m_Acceleration; }
    }
    public float RotationSpeed
    {
        get { return this.m_RotationSpeed; }
    }
    public float RotationAcceleration
    {
        get { return this.m_RotationAcceleration; }
    }
    public bool IsVisible
    {
        get { return this.m_IsVisible; }
    }
#endregion
}
