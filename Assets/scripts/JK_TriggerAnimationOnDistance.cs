using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JK_TriggerAnimationOnDistance : MonoBehaviour
{
    public GameObject targetObject;
    public float triggerDistance = 5f;
    public string animationTrigger = "ActivateAnimation";

    private Animator animator;
    private Character characterComponent;

    void Start()
    {
        characterComponent = GetComponent<Character>();
        if (animator == null)
        {
            if (GetComponentInChildren<Animator>()) animator = GetComponentInChildren<Animator>();
            else animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (targetObject != null)
        {
            if (targetObject.GetComponent<Renderer>() && targetObject.GetComponent<Renderer>().enabled == false)
                return;

            // Calculate the distance between the current GameObject and the targetObject
            float distance = Vector3.Distance(transform.position, targetObject.transform.position);

            // If the distance is below the threshold, trigger the animation
            if (distance < triggerDistance)
            {
                UnityEngine.Debug.Log("Distance is below threshold.");
                if (animator != null)
                {
                    // Trigger the animation
                    animator.SetTrigger(animationTrigger);
                    characterComponent.enabled = false;
                }
            }
        }
        else
        {
            Debug.LogError("Target object not assigned. Please assign a target object in the inspector.");
        }
    }
}