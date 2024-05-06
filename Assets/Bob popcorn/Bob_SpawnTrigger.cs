

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Bob_SpawnTrigger : MonoBehaviour
{
    public float speed = 5f; // Movement speed of spawned objects
    public float minDistance = 2.0f; // Minimum distance to move towards the spawn trigger
    public float foodAmount = 5;
    public float biteSize = 0.001f;
    public float placementLineLength = 2.0f;

    public float creationTime;

    public List<Renderer> childrenRenderers;

    private Dictionary<float, GameObject> sortedBobs = new Dictionary<float, GameObject>();

    private float waitTime_After_finishedEating = 2;
    private bool finishedEating = false;

    // Function to calculate the distance between a point and a line segment
    private static float DistanceToPoint(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        // Calculate the direction of the line segment
        Vector2 lineDirection = lineEnd - lineStart;

        // Calculate the vector from the line start to the point
        Vector2 pointToStart = point - lineStart;

        // Calculate the projection of pointToStart onto the lineDirection
        float t = Vector2.Dot(pointToStart, lineDirection) / lineDirection.sqrMagnitude;

        // Clamp the parameter t to ensure the closest point lies within the line segment
        t = Mathf.Clamp01(t);

        // Calculate the closest point on the line segment
        Vector2 closestPoint = lineStart + t * lineDirection;

        // Calculate the distance between the given point and the closest point on the line segment
        float distance = Vector2.Distance(point, closestPoint);

        return distance;
    }

    void SortBobs()
    {
        // Find all GameObjects with the tag "bob"
        GameObject[] bobs = GameObject.FindGameObjectsWithTag("bob");

        // sort bobs on X axis
        sortedBobs.Clear();
        foreach (GameObject bob in bobs)
        {
            float x = bob.transform.position.x;
            while (sortedBobs.ContainsKey(x))
                x += 0.01f;
            sortedBobs.Add(x, bob);
        }
    }

    void Start()
    {
        creationTime = Time.time;
        // Get all Renderer components of children
        childrenRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        foodAmount = childrenRenderers.Count-1;
    }


    GameObject FindNewestObjectWithTag()
    {
        // Find all GameObjects with the specified tag
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("eten");

        // Track the newest object and its creation time
        GameObject newestObject = null;
        float newestObjectTime = float.MinValue;

        // Iterate through all objects with the tag
        foreach (GameObject obj in objectsWithTag)
        {
            // Get the creation time of the object
            float objectTime = obj.GetComponent<Bob_SpawnTrigger>().creationTime; // Assuming TimeTracker script is attached to each GameObject

            // Check if this object was created more recently
            if (objectTime > newestObjectTime)
            {
                newestObject = obj;
                newestObjectTime = objectTime;
            }
        }

        // If a newest object was found, you can do something with it
        if (newestObject != null)
        {
            UnityEngine.Debug.Log("Newest object with tag is: " + newestObject.name);
            // Do something with the newest object...
        }
        else
        {
            UnityEngine.Debug.Log("No objects with tag found.");
        }
        return newestObject;
    }

    // Enable or disable renderer of children based on foodAmount
    public void SetActiveImageBasedOnFoodAmount(int foodAmount)
    {
        if(foodAmount <= 0)
        {
            for (int i = 0; i < childrenRenderers.Count; i++)
            {
                childrenRenderers[i].enabled = false;
            }
            return;
        }
        // Ensure the foodAmount is within valid range of childrenRenderers count
        int clampedFoodAmount = Mathf.Clamp(foodAmount, 0, childrenRenderers.Count-1);

        // Enable the renderer for the child at clampedFoodAmount index
        for (int i = 0; i < childrenRenderers.Count; i++)
        {
            childrenRenderers[i].enabled = (i == clampedFoodAmount);
        }
    }

    void Update()
    {

        if (gameObject != FindNewestObjectWithTag())
            return;

        GameObject[] bobs = GameObject.FindGameObjectsWithTag("bob");

        if(foodAmount > 0.0f)
        foreach (GameObject bob in bobs)
        {
            Vector2 lineStart = new Vector2(transform.position.x - placementLineLength *0.5f, transform.position.y);
            Vector2 lineEnd = new Vector2(transform.position.x + placementLineLength * 0.5f, transform.position.y);
            float dist = DistanceToPoint(bob.transform.position, lineStart, lineEnd);
            if (dist > minDistance +0.1f)
                continue;
            // Get the Animator component attached to the bob GameObject
            Animator animator = bob.GetComponent<Animator>();
            if (animator == null)
                if (bob.GetComponentInChildren<Animator>())
                    animator = bob.GetComponentInChildren<Animator>();
            if (animator == null)
                continue;

           AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
           if (stateInfo.IsName("run"))
               continue;

            //take bites
            foodAmount -= Time.deltaTime * 0.75f;
            SetActiveImageBasedOnFoodAmount((int)foodAmount);
        }
        else
        {
            if (!finishedEating)
            {
                foreach (GameObject bob in bobs)
                {
                    // Get the Animator component attached to the bob GameObject
                    Animator animator = bob.GetComponent<Animator>();
                    if (animator == null)
                        if (bob.GetComponentInChildren<Animator>())
                            animator = bob.GetComponentInChildren<Animator>();
                    if (animator == null)
                        continue;
                    animator.Play("idle");
                    UnityEngine.Debug.Log("Eating finished");

                    PlayAudio playAudio = bob.GetComponentInChildren<PlayAudio>();
                    if(playAudio != null)
                    {
                        float randomValue = Random.Range(0.0f, (float)bobs.Count());
                        // Check if the random value is less than the chance
                        if (randomValue < bobs.Count() / 2)
                            playAudio.PlayHappySound();
                    }
                }
                waitTime_After_finishedEating = 2;
                finishedEating = true;
            }
            else
            {
                waitTime_After_finishedEating -= Time.deltaTime;
                if (waitTime_After_finishedEating <= 0)
                {
                    foreach (GameObject bob in bobs)
                    {
                        Character character = bob.GetComponent<Character>();
                        if (character)
                        {
                            FiducialController fidu = bob.GetComponent<FiducialController>();
                            character.overridenByOtherScript = false;
//                             if (fidu)
//                                 if (fidu.m_IsVisible)
                                    //character.enabled = true;
                        }

                        // Get the Animator component attached to the bob GameObject
                        Animator animator = bob.GetComponent<Animator>();
                        if (animator == null)
                            if (bob.GetComponentInChildren<Animator>())
                                animator = bob.GetComponentInChildren<Animator>();
                        if (animator == null)
                            continue;
                        animator.Play("run");
                    }
                    finishedEating = false;
                    Destroy(gameObject);
                    return;
                }
            }
        }

        if (finishedEating)
            return;

        if(bobs.Count() != sortedBobs.Count())
            SortBobs();

        // Loop through each bob GameObject
        //float t = 0;
        int id = 0;
        int rowCount = 3;
        foreach (GameObject bob in sortedBobs.Values)
        {
            // Get the Animator component attached to the bob GameObject
            Animator animator = bob.GetComponent<Animator>();
            if (animator == null)
                if (bob.GetComponentInChildren<Animator>()) 
                    animator = bob.GetComponentInChildren<Animator>();
            if (animator == null)
                continue;
                // Activate the "run" animation if the Animator component exists


            Character character = bob.GetComponent<Character>();
            if (!animator.gameObject.activeSelf)
                continue;

            if (character)
                character.overridenByOtherScript = true;
                //character.enabled = false;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            bool reached = stateInfo.IsName("eating");

            Vector3 targetPos = transform.position;


            int rowID = id / rowCount;
            float incr = rowID % 2 == 0 ? 0.5f : 0;
            float t = (float)(id % rowCount) / (float)rowCount;
            targetPos.x += Mathf.Lerp(placementLineLength * -0.5f, placementLineLength * 0.5f, t) + incr;
            targetPos.y += (float)rowID * 0.25f;
            id++;

            // Move the bob GameObject towards the spawn trigger
            if (!reached)
            {
                bob.transform.position = Vector3.MoveTowards(bob.transform.position, targetPos, speed * Time.deltaTime);
                if (character)
                {
                    if (targetPos.x - 0.05f >= bob.transform.position.x && character.facingRight)
                        character.Rotate();
                    else if (targetPos.x + 0.05f <= bob.transform.position.x && !character.facingRight)
                        character.Rotate();
                }
            }

            float dist = Vector3.Distance(targetPos, bob.transform.position);
            //if(!reached)
                reached = dist <= minDistance;
            if (reached)
            {
                if(!stateInfo.IsName("eating"))
                     animator.Play("eating", -1, UnityEngine.Random.value);
            }
            else
                animator.Play("run");
        }

    }
}
