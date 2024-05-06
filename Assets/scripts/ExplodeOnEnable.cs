using UnityEngine;

public class ExplodeOnEnable : MonoBehaviour
{
    bool firsTimeEnabled = true;

    void OnEnable()
    {
        // Get all Animator components in the scene
        Animator[] animators = FindObjectsOfType<Animator>();

        // Loop through all Animator components
        if(!firsTimeEnabled)
        foreach (Animator animator in animators)
        {
            // Check if the Animator has a trigger named "trigger_explode"
            if (AnimatorHasTrigger(animator, "trigger_explode"))
            {
                // Trigger the "trigger_explode" parameter
                animator.SetTrigger("trigger_explode");
            }
        }

        firsTimeEnabled = false;
    }

    bool AnimatorHasTrigger(Animator animator, string triggerName)
    {
        // Get all parameters of the Animator
        AnimatorControllerParameter[] parameters = animator.parameters;

        // Loop through all parameters
        foreach (AnimatorControllerParameter parameter in parameters)
        {
            // Check if the parameter is a trigger and has the specified name
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == triggerName)
            {
                return true;
            }
        }
        return false;
    }
}