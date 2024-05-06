using UnityEngine;
using System.Collections;

// Basic code borrowed from: http://www.ikriz.nl/2011/12/23/unity-video-remake
public class WebCamera : MonoBehaviour
{
    private WebCamTexture webcamTexture;

    IEnumerator Start()
    {
        /* First we'll try to go through the list of cameras (if
         * we can) and find a suitable one (not front-facing). */
        bool foundCamera = false;
        string desiredCameraName = "";
        if (WebCamTexture.devices != null)
        {
            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                // We'd like a camera which is not front-facing.
                if (!WebCamTexture.devices[i].isFrontFacing)
                    desiredCameraName = WebCamTexture.devices[i].name;
            }
        }

        /* If we've got one, we try to get access and then play it. */
        if (foundCamera)
        {
            webcamTexture = new WebCamTexture(desiredCameraName);
            GetComponent<Renderer>().material.mainTexture = webcamTexture;

            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                webcamTexture.Play();
            }
        }
        else
        {
            // No camera. So sad. Time to end it.
            print("No");
        }
    }
}