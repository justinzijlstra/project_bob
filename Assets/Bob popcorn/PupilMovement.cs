
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

public class PupilMovement : MonoBehaviour
{
    public string firstChildName = "pupil l";
    public string secondChildName = "pupil r";
    public float eyeRadius = 0.1f;


    [Serializable]
    public class AnimationPupilTarget
    {
        public string animationStateName;
        public GameObject targetGameObject;
    }

    private Animator animator;
    private Transform firstChild;
    private Transform secondChild;


    public List<AnimationPupilTarget> animationPupilTargets; // List to hold pairs of animation clips for replacement


    void Start()
    {

        if (GetComponentInChildren<Animator>()) animator = GetComponentInChildren<Animator>();
        else animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on the current GameObject.");
        }

        // Find the first child GameObject
        firstChild = FindChildByName(firstChildName);

        // Find the second child GameObject
        secondChild = FindChildByName(secondChildName);
    }

    void LateUpdate()
    {
        // Ensure animator reference is set
        if (animator == null)
            return;

        if (firstChild == null)
            return;

            // Get the current state of the Animator
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Iterate through the animationPupilTargets list
        foreach (AnimationPupilTarget target in animationPupilTargets)
        {
            // Check if the current state matches a state name in the list
            if (stateInfo.IsName(target.animationStateName))
            {
                // Get the direction towards the target GameObject
                Vector3 direction = target.targetGameObject.transform.position - firstChild.transform.position;
                direction.z = 0f; // Ensure it's only in 2D space

                // Calculate the rotation angle towards the target
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                Vector3 offset = new Vector3(eyeRadius, 0, 0);

                // Rotate the vector around the z-axis by the specified angle
                offset = Quaternion.Euler(0, 0, angle) * offset;

//                 //compensate flip
//                 if (transform.localRotation.eulerAngles.y == 180)
//                     offset.x *= -1;

                Debug.Log("Ang: " + transform.localRotation.eulerAngles.y);
                //Debug.Log("Angle: " + angle);
                //Debug.Log("Offset: " + offset);
                firstChild.position += offset;

                if (secondChild != null)
                    secondChild.position += offset;

                // Break out of the loop since we've found a match
                break;
            }
        }
    }

    static List<Transform> GetAllChildren(Transform parent, List<Transform> transformList = null)
    {
        if (transformList == null) transformList = new List<Transform>();

        foreach (Transform child in parent)
        {
            transformList.Add(child);
            GetAllChildren(child, transformList);
        }
        return transformList;
    }

    Transform FindChildByName(string name)
    {
        List<Transform> siblings = GetAllChildren(transform);
        // Search for a sibling GameObject with the given name
        foreach (Transform sibling in siblings)
        {
            if (sibling.name == name)
            {
                return sibling;
            }
        }
        // Child not found
        Debug.LogWarning("Child with name '" + name + "' not found.");
        return null;
    }
}
