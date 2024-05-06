using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YHeightRenderOrder : MonoBehaviour
{
    public int renderOrderOffset = 0;
    bool scaleTransform = true;
    public bool adjustZ = false;
    Vector3 startScale;
    private Transform childTransform = null;

    float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        // First, normalize the value within the range fromMin to fromMax
        float normalizedValue = Mathf.InverseLerp(fromMin, fromMax, value);

        // Then, map the normalized value to the range between toMin and toMax
        return Mathf.Lerp(toMin, toMax, normalizedValue);
    }

    void Start()
    {
        startScale = transform.localScale;
        if (GetComponent<Character>() != null && transform.childCount != 0)
        {
            childTransform = transform.GetChild(0);
            startScale = childTransform.localScale;
        }
    }

    void LateUpdate()
    {
        if(scaleTransform)
        {
            float minY = -5f;
            float maxY = 5f;
            float clampedValue = Mathf.Clamp(transform.position.y, minY, maxY);
            float mappedValue = Map(clampedValue, minY, maxY, 1.5f, 0.75f);
            if(childTransform)
                childTransform.localScale = startScale * mappedValue;
            else
                transform.localScale = startScale * mappedValue;
        }

        if(adjustZ)
        {
            Vector3 newPosition = transform.position;

            // Set the new z position
            newPosition.z = (Mathf.RoundToInt(-transform.position.y * 10.0f) + renderOrderOffset); ;
            // Assign the modified position back to the transform
            transform.position = newPosition;
        }

        // Get the renderer of the current GameObject
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            // Set the sorting order of the renderer
            renderer.sortingOrder = (Mathf.RoundToInt(-transform.position.y * 100.0f) + renderOrderOffset * 10) ;
        }
    }
}
