using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class DualAnimationClip
{
    public AnimationClip originalClip;
    public AnimationClip replacementClip;
}

public class RuntimeAnimationController : MonoBehaviour
{
    public Animator originalAnimatorController; // Reference to the original Animator Controller
    public List<DualAnimationClip> dualAnimationClips; // List to hold pairs of animation clips for replacement

    void Start()
    {
        // Call the function to modify the existing Animator Controller at runtime
        ApplyAnimationClips();
    }

    void ApplyAnimationClips()
    {
        // Make sure there is an Animator component on the current GameObject
        Animator animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the current GameObject.");
            return;
        }
        originalAnimatorController = animator;

        // Duplicate the original Animator Controller
        AnimatorOverrideController overrideController = new AnimatorOverrideController(originalAnimatorController.runtimeAnimatorController);

        // Assign the Animator Override Controller to the Animator component
        animator.runtimeAnimatorController = overrideController;

        // Assign animation clips to different states
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        foreach (var dualAnimationClip in dualAnimationClips)
        {
            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(dualAnimationClip.originalClip, dualAnimationClip.replacementClip));
        }

        // Apply the overrides to the Animator Override Controller
        overrideController.ApplyOverrides(overrides);
    }
}
