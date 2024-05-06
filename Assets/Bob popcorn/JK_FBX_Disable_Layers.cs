using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JK_FBX_Disable_Layers : MonoBehaviour
{
    private List<SiblingInfo> siblings = new List<SiblingInfo>();

    void Start()
    {
        // Find all siblings recursively
        FindSiblings(transform);
    }

    void FindSiblings(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child != transform)
            {
                siblings.Add(new SiblingInfo(child));
            }

            // Recursive call to find children's siblings
            FindSiblings(child);
        }
    }

    void LateUpdate()
    {
        foreach (var siblingInfo in siblings)
        {
            Transform child = siblingInfo.Child;
            bool isEnabledOnStart = siblingInfo.IsEnabledOnStart;

            // Check conditions for disabling/enabling the GameObject
            if (child.localPosition.x < -999 || child.localPosition.x > 999 || Mathf.Abs(child.localScale.x) < 0.01f)
            {
                if (child.gameObject.activeSelf)
                {
                    siblingInfo.DecrementTimer();
                    if (siblingInfo.Timer <= 0)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
            else if (gameObject.activeSelf) //only allow children to trigger back on, if the parent itself is active.
            {
                if (!child.gameObject.activeSelf && isEnabledOnStart)
                {
                    siblingInfo.ResetTimer();
                    child.gameObject.SetActive(true);
                }
            }
        }
    }
}

public class SiblingInfo
{
    public Transform Child { get; private set; }
    public bool IsEnabledOnStart { get; private set; }
    public int Timer { get; private set; }

    public SiblingInfo(Transform child)
    {
        Child = child;
        IsEnabledOnStart = child.gameObject.activeSelf;
        Timer = 10; // Initial timer value
    }

    public void DecrementTimer()
    {
        Timer--;
    }

    public void ResetTimer()
    {
        Timer = 10;
    }
}
