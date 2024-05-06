using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Experimental.XR;

public class CameraSnap : MonoBehaviour
{
     Renderer topRenderer;
     Renderer[] childRenderers;
    public float tolerance;
    Camera cam;
    int width = Screen.width;
    int height = Screen.height;

    public Canvas canvasToHide;
    public string textureInputName = "_BaseMap";

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void StartSnapping()
    {
        StartCoroutine(Screenshot());
    }

    //creat a new texture and floodfill 
    public IEnumerator Screenshot( )
    {
        if(canvasToHide != null)
            canvasToHide.enabled = false;
        // TODO evt aftellen 

        cam.depth = 1;
        yield return new WaitForEndOfFrame();

        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.FloodFillArea(10, height/2, Color.clear, tolerance);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();

        //topRenderer.sharedMaterial.mainTexture = tex;
        topRenderer.material.SetTexture(textureInputName, tex);

        for (int i = 0; i < childRenderers.Length; i++)
        {
            if (childRenderers[i].name != "eye")
            {
                childRenderers[i].sharedMaterial.mainTexture = tex; //Norm
                childRenderers[i].material.SetTexture(textureInputName, tex);
                //childRenderers[i].sharedMaterial.color = Color.black;//--Norm----------------------------------------------------------
            }
        }
        cam.depth = -5;

        if (canvasToHide != null)
            canvasToHide.enabled = true;

        yield return null;
    }


    //geeft een GameObject vanuit een ander script  & laad alle child renderers in 
    public void LoadNew(GameObject newDrawing)
    {
        topRenderer = newDrawing.GetComponent<Renderer>();
        childRenderers = addToList.getChildren(topRenderer, true);
    } //load a drawing and all its child renderers

    public class addToList
    {
        public static Renderer[] getChildren(Renderer parent, bool recursive)
        {
            List<Renderer> items = new List<Renderer>();
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                items.Add(parent.transform.GetChild(i).gameObject.GetComponent<Renderer>());
                if (recursive)
                { // set true to go through the hiearchy.
                    items.AddRange(getChildren(parent.transform.GetChild(i).gameObject.GetComponent<Renderer>(), recursive));
                }
            }
            return items.ToArray();
        }
    } //add children to list
}
