using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;
using UnityEngine.Networking;
using static System.Net.Mime.MediaTypeNames;

public class JZLoadFromExternalV2 : MonoBehaviour
{
	enum sourceElementType { Video, Texture };
	sourceElementType sourceType;

    //TODO: Turned off stuff is the ligth version 

#if UNITY_STANDALONE_WIN
    string filesLocation = @"C:/StoryScopeMedia/";
#endif
#if UNITY_STANDALONE_LINUX
    string filesLocation = "/home/InteractiveCulture/StoryScopeMedia/";
#endif
#if UNITY_STANDALONE_OSX
    string filesLocation = "";
#endif

    public string folder;
    [SerializeField]
    string path;
    public List<Texture2D> images = new List<Texture2D>();
    readonly List<string> videoURL = new List<string>();
    public bool finishedLoading = false;
    VideoPlayer vp;
    Renderer jzrenderer = null;
    public int currentItem = 0;

    void Start()
    {
        if (UnityEngine.Application.isEditor)
            filesLocation = "Assets/Bob popcorn/";
        path = filesLocation + folder;
        GetComponentInChildren<FiducialController>().MarkerID = int.Parse(gameObject.name.Substring(gameObject.name.Length - 2));
        //Check if video or Texture
        if (GetComponent<VideoPlayer>()) sourceType = sourceElementType.Video;
        else sourceType = sourceElementType.Texture;

        if (sourceType == sourceElementType.Video)
        {
            vp = GetComponent<VideoPlayer>();
            //extension = "mp4";

        }
        if (sourceType == sourceElementType.Texture)
        {
            jzrenderer = GetComponent<Renderer>();
            //extension = "png";
        }
       //StartCoroutine("LoadAll", Directory.GetFiles(filesLocation, "*." + extension, SearchOption.AllDirectories));
        StartCoroutine(LoadAll(Directory.GetFiles(path)));
    }

    private void Update()
    {
        //TODO:if (Input.GetKey(KeyCode.Space) && Input.GetKeyDown(KeyCode.B)) StartCoroutine(LoadAll(Directory.GetFiles(path)));

        if (!UnityEngine.Application.isEditor)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Next(false);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Next(true);
    }
    public IEnumerator LoadAll(string[] filePaths) //Load all video's and textures
    {
        foreach (string filePath in filePaths)
        {
            if (sourceType == sourceElementType.Texture)
            {
                UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file:///" + filePath);

                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success) Debug.Log(uwr.error);
                else images.Add(DownloadHandlerTexture.GetContent(uwr));
            }

            UnityWebRequest load = new UnityWebRequest("file:///" + filePath);

            if (sourceType == sourceElementType.Video)
            {
                yield return load;

                if (!string.IsNullOrEmpty(load.error)) Debug.LogWarning(filePath + " error");
                else videoURL.Add(load.url);
            }
        }
        finishedLoading = true;
    }
    public void Next(bool next)// show Next or previous item (texture or video)
    {
        if (next) currentItem++;
        if (!next) currentItem--;
        if (sourceType == sourceElementType.Video)
        {
            if (currentItem < 0) currentItem = videoURL.Count - 1;
            currentItem %= videoURL.Count;
            vp.url = videoURL[currentItem];
        }
        if (sourceType == sourceElementType.Texture)
        {
            if (currentItem < 0) currentItem = images.Count - 1;
            currentItem %= images.Count;
            jzrenderer.material.mainTexture = images[currentItem];
        }

    }

    public void OnApplicationFocus(bool focus)
    {
      //TODO:  images.Clear();
       //TODO: StartCoroutine(LoadAll(Directory.GetFiles(path)));
    }
}
