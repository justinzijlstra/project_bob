using UnityEngine;

public class AddScriptToChild : MonoBehaviour
{
    // Reference to the script you want to add to the child layer
    public MonoBehaviour scriptToAdd;

    void Start()
    {
        // Call a method to add the script to all child GameObjects
        AddScriptToChildren();
    }

    void AddScriptToChildren()
    {
        // Iterate through all child GameObjects
        foreach (Transform child in transform)
        {
            // Check if the child already has the script attached
            if (child.GetComponent(scriptToAdd.GetType()) == null)
            {
                // Add the script to the child GameObject
                child.gameObject.AddComponent(scriptToAdd.GetType());

                // Optionally, you can set specific properties of the added script here
                // For example: (child.GetComponent(scriptToAdd.GetType()) as YourScriptType).YourProperty = YourValue;
            }
        }
    }
}
