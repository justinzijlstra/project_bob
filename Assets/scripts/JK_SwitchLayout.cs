using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class JK_SwitchLayout : MonoBehaviour
{

    public Transform LayoutsParentObject;

    public JZLoadFromExternalV2 additional_ExternalBackgroundsLoader;
    bool texturesLoaded = false;

    public int currentItem = 0;
    public int itemCount = 0;

    public int layOuts_InScene_ItemCount = 0;
    public int layOuts_External_ItemCount = 0;

    Renderer renderer = null;
    

    private IEnumerator WaitForOtherScript()
    {
        // Wait until the condition from other script becomes true
        while (!additional_ExternalBackgroundsLoader.finishedLoading)
        {
            yield return null;
        }

        // Now the condition is true, continue with your logic
        UnityEngine.Debug.Log("background textures from additional_ExternalBackgroundsLoader finished loading");

        layOuts_External_ItemCount = additional_ExternalBackgroundsLoader.images.Count;
        itemCount += layOuts_External_ItemCount;

        texturesLoaded = true;
    }

    void Start()
    {
        if (LayoutsParentObject)
        {
            itemCount += LayoutsParentObject.childCount;
            layOuts_InScene_ItemCount = itemCount;
        }

        if (additional_ExternalBackgroundsLoader)
            StartCoroutine(WaitForOtherScript());
        else
            texturesLoaded = true;

        renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (!UnityEngine.Application.isEditor)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Next(false);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Next(true);
    }

    public void Next(bool next)// show Next or previous item (texture or video)
    {
        if (!texturesLoaded)
            return;

        if (itemCount == 0)
            return;

        if (next) currentItem++;
        if (!next) currentItem--;
        if (currentItem < 0) 
            currentItem = itemCount - 1;
        currentItem %= itemCount;

        //first handle layouts in scene ////////
        int id = 0;
        foreach (Transform child in LayoutsParentObject)
        {
            child.gameObject.SetActive(false);
            if (id == currentItem)
                child.gameObject.SetActive(true);
            id++;
        }
        //////////////////////////////////////////////
        

        //then handle external layouts//////////////////
        if (renderer == null)
            return;

        bool isAtExternalLayout = currentItem >= layOuts_InScene_ItemCount;
        //only activate renderer if we draw an external layout
        renderer.enabled = isAtExternalLayout;
        if (isAtExternalLayout)
        {
            int currentExternalItem = currentItem - layOuts_InScene_ItemCount;
            //set the other item id manually
            additional_ExternalBackgroundsLoader.currentItem = currentExternalItem;
            //move back and forth to update the texture
            additional_ExternalBackgroundsLoader.Next(!next);
            additional_ExternalBackgroundsLoader.Next(next);

        }
        ///////////////////////////////////////////////////////




    }

}
