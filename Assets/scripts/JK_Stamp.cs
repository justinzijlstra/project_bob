using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JK_Stamp : MonoBehaviour
{


    public bool cloned = false;
    public bool vis;
    [SerializeField]
    private List<GameObject> stamp_prefabList = new List<GameObject>();

    public bool limitAmountofStamps = false;
    public int max_stamps = 5;

    FiducialController fidu;

    private List<GameObject> stamps = new List<GameObject>();

    void Start()
    {
        fidu = GetComponent<FiducialController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (fidu.m_IsVisible)
        {
            bool toManyStamps = false;
            if (limitAmountofStamps)
            {
                int count = GameObject.FindGameObjectsWithTag("eten").Length;
                if (count >= max_stamps)
                    toManyStamps = true;
            }
                

            StopAllCoroutines();
            if (!cloned && !toManyStamps)
            {
                cloned = true;
                Vector3 temp = transform.position;
                //temp.z = -4;
                GameObject stamp = stamp_prefabList[Random.Range(0, stamp_prefabList.Count)];
                GameObject tempStamp = Instantiate(stamp, temp, transform.rotation * Quaternion.Euler(0f, 0f, 0f));
                stamps.Add(tempStamp);
                //tempStamp.transform.localScale -= new Vector3(0.3f, 0.3f, 0.3f);
            }
        }
        if (!fidu.m_IsVisible && cloned == true)
        {
            //StartCoroutine(deleteStamps());
            cloned = false;
        }
    }

    //IEnumerator deleteStamps()
    //{
    //    yield return new WaitForSeconds(10);
    //    foreach (GameObject stamp in stamps)
    //    {
    //        Destroy(stamp);
    //    }
    //    stamps.Clear();
    //}
}
