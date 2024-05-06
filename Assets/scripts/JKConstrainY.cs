using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKConstrainY : MonoBehaviour
{
    private Vector3 m_spawnPos;
    public GameObject endMarkerPrefab;
    public bool isSwimmingDuck;

    // Start is called before the first frame update
    void Start()
    {
        m_spawnPos = transform.position;
    }


    void EnableRenderersRecursively(Transform currentTransform, bool _enabled)
    {
        Renderer currentRenderer = currentTransform.gameObject.GetComponent<Renderer>();
        if (currentRenderer != null)
        {
            currentRenderer.enabled = _enabled;
        }

        foreach (Transform child in currentTransform)
        {
            EnableRenderersRecursively(child, _enabled);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(endMarkerPrefab == null)
            return;

        bool show = (isSwimmingDuck && endMarkerPrefab.transform.position.y <= (m_spawnPos.y - 0.5f)) || (!isSwimmingDuck && endMarkerPrefab.transform.position.y > (m_spawnPos.y - 0.5f));
        EnableRenderersRecursively(transform, show);

    //foreach (Transform child in transform)
     //  child.gameObject.SetActive(show);


    }
    void LateUpdate()
    {
        if(!isSwimmingDuck)
            transform.position = new Vector3(transform.position.x, m_spawnPos.y, transform.position.z);
    }
}
