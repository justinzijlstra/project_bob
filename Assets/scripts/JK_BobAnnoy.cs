
using UnityEngine;

public class JK_BobAnnoy : MonoBehaviour
{
    public GameObject targetObject;
    public float triggerDistance = 5f;
    public float calmDownDistance = 6f;
    public string animationTrigger_Left = "ActivateAnimationLeft";
    public string animationTrigger_Right = "ActivateAnimationRight";
    public string animationTrigger_Up = "ActivateAnimationUp";
    public string animationTrigger_angry = "trigger_angry";

    public int annoyLevel = 0;
    public bool triggered = false;
    private Animator animator;
    public int annoyTimer = 0;
    public int cooldownTimer = 0;
    
    private Character characterComponent;
    private FiducialController target_fiduController;
    private string storedState = "idle";

    private bool agitated = false;

    // Start is called before the first frame update
    void Start()
    {
        if(targetObject != null)
            target_fiduController = targetObject.GetComponent<FiducialController>();
        characterComponent = GetComponent<Character>();
        if (animator == null)
        {
            if (GetComponentInChildren<Animator>()) animator = GetComponentInChildren<Animator>();
            else animator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObject != null)
        {
            // Calculate the distance between the current GameObject and the targetObject
            float distance = Vector3.Distance(transform.position, targetObject.transform.position);
            if (!targetObject.activeSelf || targetObject.GetComponent<Renderer>() && targetObject.GetComponent<Renderer>().enabled == false || (target_fiduController != null && !target_fiduController.m_IsVisible) )
                distance = 99999;

            // If the distance is below the threshold, trigger the animation

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (distance < calmDownDistance)
            {
                bool allowAgitation = (characterComponent.idle && stateInfo.IsName("idle") || stateInfo.IsName("agitated"));
                if (allowAgitation)
                {
                    agitated = true;
                    //characterComponent.enabled = false;
                    if (stateInfo.IsName("idle") /*|| stateInfo.IsName("walk") || stateInfo.IsName("run")*/)
                    {
                        storedState = stateInfo.IsName("idle") ? "idle" : stateInfo.IsName("walk") ? "walk" : "run";
                        animator.Play("agitated");
                    }
                }
                if (!characterComponent.idle)
                    agitated = false;

            }

            if (distance < triggerDistance)
            {
                annoyTimer++;
                if(annoyTimer > 50)
                {
                    annoyTimer = 0;
                    triggered = false;
                }
                Vector3 dir = targetObject.transform.position - transform.position;
                //UnityEngine.Debug.Log("Distance is below threshold.");

                if (agitated)
                if (animator != null && !triggered)
                {
                    if(stateInfo.IsName("idle"))
                        animator.Play("agitated");

                    triggered = true;
                    characterComponent.enabled = false;
                    annoyLevel++;
                    cooldownTimer = 300 * annoyLevel;
                    animator.SetInteger("annoyLevel", annoyLevel);

                    if (Mathf.Abs(dir.x) < triggerDistance * 0.5f && dir.y > 0)
                        animator.SetTrigger(animationTrigger_Up);
                    else if (dir.x > 0)
                        animator.SetTrigger(animationTrigger_Right);
                    else if (dir.x <= 0)
                        animator.SetTrigger(animationTrigger_Left);
                    if (annoyLevel >= 2)
                    {
                        animator.SetTrigger(animationTrigger_angry);
                        annoyLevel = 0;
                    }
                    
                }
            }
            else
            {
                if (agitated && distance > calmDownDistance)
                {
                    if(cooldownTimer < 0)
                    {
                        animator.Play(storedState);
                        agitated = false;
                        if (!stateInfo.IsName("explode"))
                            characterComponent.enabled = true;
                    }

                }
                    

                cooldownTimer--;
                triggered = false;
                annoyTimer = 0;
                if(cooldownTimer == 1)
                {
                    annoyLevel = 0;
                    if (!stateInfo.IsName("explode"))
                        characterComponent.enabled = true;
                }
                    
            }
                
        }
        else
        {
            Debug.LogWarning("Target object not assigned. Please assign a target object in the inspector. obj: " + gameObject.name);
        }
    }
}
