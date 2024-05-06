using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float WalkSpeed, RunSpeed;
    [Min(0.01f)]
    public float stopRunningUntilThisDist;
    public bool singleSprite, controlRotation, controlPosition, facingRight;
    public bool useXFlip = false;

    public bool orient_towards_marker = false;
    public GameObject endMarkerPrefab;
    string check;
    float speed, distance;
    float distanceToMove = 0.5f;
    GameObject endMarker;
    public bool idle;
    Animator animator;
    FiducialController endMarkerFC, fidu;
    SpriteRenderer spriteRenderer;
    bool debugMode = true;
    Vector3 startScale;

    public bool updateAnimator = true;
    public bool overridenByOtherScript = false;

    void Start()
    {
       if(singleSprite) spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        endMarker = SetEndMarker();
        fidu = GetComponent<FiducialController>();
        if(endMarker.name == "testMarker")
            debugMode = true;
        startScale = transform.localScale;
    }

    void ResetTriggers()
    {
        if (animator == null)
            return;

        // Get the number of parameters in the Animator
        int paramCount = animator.parameterCount;

        // Loop through all parameters
        for (int i = 0; i < paramCount; i++)
        {
            // Get the type of the parameter at index i
            AnimatorControllerParameterType paramType = animator.GetParameter(i).type;

            // If the parameter is a trigger, reset it
            if (paramType == AnimatorControllerParameterType.Trigger)
            {
                string paramName = animator.GetParameter(i).name;
                animator.ResetTrigger(paramName);
            }
        }
    }

    void OnEnable()
    {
        //reset all animation states
        if (animator != null)
        {
            ResetTriggers();
        }
        //reset overriden state.
        overridenByOtherScript = false;
    }

    void Update()
    {
        if(overridenByOtherScript)
            return;

        if ((debugMode || fidu.m_IsVisible) && animator == null) GetAnimator();
        CheckGrounded();
        if(orient_towards_marker) 
            OrientTowardsMarker();
        else
            Flip();
    }

    private void OrientTowardsMarker()
    {
        Vector3 newScale = startScale;
        transform.right = endMarker.transform.position - transform.position;
        if (endMarker.transform.position.x < transform.position.x)
            newScale.y*= -1.0f;
        transform.localScale = newScale;
        transform.Rotate(new Vector3(0, +180, 0));
    }

    private void CheckGrounded()
    {
        if (Vector3.Distance(transform.position, endMarker.transform.position) > distanceToMove) idle = false;
        if (transform.position == endMarker.transform.position) idle = true;
        if (!idle) transform.position = Vector3.MoveTowards(transform.position, endMarker.transform.position, speed * Time.deltaTime); //move to target

        SetAnimation(idle);
    }

    private void SetAnimation(bool idle)
    {
        if (!updateAnimator)
            return;
        distance = Vector3.Distance(transform.position, endMarker.transform.position);

        if (idle)
        {
            SetAnimationAndSpeed("idle", 0f);
            return;
        }

        if (distance > 0.1f && distance < stopRunningUntilThisDist) SetAnimationAndSpeed("walk", WalkSpeed);
        else if (distance > stopRunningUntilThisDist) SetAnimationAndSpeed("run", RunSpeed);
    }

    private void GetAnimator()
    {
        if (animator == null)
        {
            if (GetComponentInChildren<Animator>()) animator = GetComponentInChildren<Animator>();
            else animator = GetComponent<Animator>();
        }
    }

    private GameObject SetEndMarker()
    {
        GameObject tempEndMarker = Instantiate(endMarkerPrefab);
        tempEndMarker.name = gameObject.name + "_MARKER";
        endMarkerFC = tempEndMarker.GetComponent<FiducialController>();
        endMarkerFC.MarkerID = GetComponentInParent<FiducialController>().MarkerID;
        endMarkerFC.IsRotationMapped = controlRotation;
        endMarkerFC.IsPositionMapped = controlPosition;
        return tempEndMarker;
    }


    public void SetAnimationAndSpeed(string animation, float setSpeed)
    {
        //Debug.Log("SetAnimationAndSpeed: " + animation + " " + setSpeed);
        if (animation != check)
        {
            speed = setSpeed;
            if (!animator) return;

            if (debugMode || fidu.m_IsVisible) 
                animator.Play(animation);
            check = animation;
        }
    }

    private void Flip()
    {
        if (endMarker.transform.position.x - 0.05f >= transform.position.x && facingRight) Rotate();
        else if (endMarker.transform.position.x + 0.05f <= transform.position.x && !facingRight) Rotate();
    }
    public void Rotate()
    {
        if (singleSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            return;
        }
        facingRight = !facingRight;

        if(useXFlip)
        {
            Vector3 newScale = startScale;
            if (!facingRight)
                newScale.x *= -1.0f;
            transform.localScale = newScale;
        }
        else
            transform.Rotate(new Vector3(0, +180, 0));

    }
}
