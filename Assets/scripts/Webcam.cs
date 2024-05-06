using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Webcam : MonoBehaviour
{
    public CameraSnap camSensetivity;
    public GameObject sliders;
    public Slider contrastSlider, saturationSlider, posterizeSlider, sensetivitySlider;
    public Material _material;
    private WebCamTexture webcamTexture;
    public GameObject warning;
    public Vector2 camResolution, contrast, saturation, posterize, sensetivity;
    WebCamDevice[] devices;

    private void Start()
    {
        WebCamDevice[] devicesPRINT = WebCamTexture.devices;
        GetComponent<Renderer>().material = _material;
        Startup();
    }

    private void Startup()
    {
        devices = WebCamTexture.devices;
        webcamTexture = new WebCamTexture();
        StartCoroutine(TryCamera());
        if (PlayerPrefs.HasKey("contrast")) _material.SetFloat("_Contrast", GetFloatFromPlayerPrefs("contrast"));
        if (PlayerPrefs.HasKey("saturation")) _material.SetFloat("_Saturation", GetFloatFromPlayerPrefs("saturation"));
        if (PlayerPrefs.HasKey("posterize")) _material.SetFloat("_Posterize", GetFloatFromPlayerPrefs("posterize"));
        if (PlayerPrefs.HasKey("sensitivity")) camSensetivity.tolerance = GetFloatFromPlayerPrefs("sensitivity");
        contrastSlider.value = GetFloatFromPlayerPrefs("contrast");
        saturationSlider.value = GetFloatFromPlayerPrefs("saturation");
        posterizeSlider.value = GetFloatFromPlayerPrefs("posterize");
        sensetivitySlider.value = GetFloatFromPlayerPrefs("sensitivity");
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad < 15f) return;
        if (Input.GetKey(KeyCode.Alpha1)) UpdateTextureSettings(-.01f, "_Contrast", contrast, contrastSlider, "contrast");
        if (Input.GetKey(KeyCode.Alpha1)) UpdateTextureSettings(-.01f, "_Contrast", contrast, contrastSlider, "contrast");
        if (Input.GetKey(KeyCode.Alpha2)) UpdateTextureSettings(.01f, "_Contrast", contrast, contrastSlider, "contrast");
        if (Input.GetKey(KeyCode.Alpha3)) UpdateTextureSettings(-.05f, "_Saturation", saturation, saturationSlider, "saturation");
        if (Input.GetKey(KeyCode.Alpha4)) UpdateTextureSettings(.05f, "_Saturation", saturation, saturationSlider, "saturation");
        if (Input.GetKeyDown(KeyCode.Alpha5)) UpdateTextureSettings(-1f, "_Posterize", posterize, posterizeSlider, "posterize");
        if (Input.GetKeyDown(KeyCode.Alpha6)) UpdateTextureSettings(1f, "_Posterize", posterize, posterizeSlider, "posterize");
        if (Input.GetKeyDown(KeyCode.Alpha7)) UpdateTextureSettings(-1f, sensetivity, sensetivitySlider, "sensitivity");
        if (Input.GetKeyDown(KeyCode.Alpha8)) UpdateTextureSettings(1f,  sensetivity, sensetivitySlider, "sensitivity");
        if (Input.GetKeyDown(KeyCode.H)) sliders.SetActive(!sliders.activeInHierarchy);
        if (!webcamTexture.isPlaying) warning.SetActive(true);
        else warning.SetActive(false);
    }

    private void OnDisable()
    {
        sliders.SetActive(false);
    }

    private void UpdateTextureSettings(float value, Vector2 clamp, Slider slider, string playerPrefsKey)
    {
        float c = Mathf.Clamp(camSensetivity.tolerance + value, clamp.x, clamp.y);
        camSensetivity.tolerance = c;
        slider.value = c;
        SetFloatToPlayerPrefs(playerPrefsKey, camSensetivity.tolerance);
        PlayerPrefs.Save();
    }
    private void UpdateTextureSettings(float value, string setting, Vector2 clamp, Slider slider, string playerPrefsKey)
    {
        float c = Mathf.Clamp(_material.GetFloat(setting) + value, clamp.x, clamp.y); //1 7
        _material.SetFloat(setting, c);
        slider.value = c;
        SetFloatToPlayerPrefs(playerPrefsKey, _material.GetFloat(setting));
        PlayerPrefs.Save();
    }
    private void SetFloatToPlayerPrefs(string contrast, float value)
    {
        PlayerPrefs.SetFloat(contrast, value);
    }
    private float GetFloatFromPlayerPrefs(string contrast)
    {
        return PlayerPrefs.GetFloat(contrast);
    }

    private void RestartCam()
    {
        if (!webcamTexture.isPlaying)
        {
            for (int i = 0; i < devices.Length; i++)
            {
#if UNITY_EDITOR
                webcamTexture = new WebCamTexture(devices[0].name, (int)camResolution.x, (int)camResolution.y, 30);
#endif
#if UNITY_STANDALONE_LINUX
                webcamTexture = new WebCamTexture(devices[2].name, (int)camResolution.x, (int)camResolution.y, 30);
#endif
                if (webcamTexture.isReadable) webcamTexture.Play();
                _material.SetTexture("_WebcamTex", webcamTexture);
                if (webcamTexture.isPlaying) break;
            }
        }
    }
    IEnumerator TryCamera()
    {
        while (true)
        {
            if (webcamTexture.isPlaying) break;
            RestartCam();
            yield return new WaitForSeconds(10);
        }
    }

    public void OnApplicationQuit()
    {
        TurnOffWebcam();
    }

    public void TurnOffWebcam()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            Debug.Log("Camera is still playing");
            webcamTexture.Stop();

            while (webcamTexture.isPlaying)
            {
                continue;
            }
            Debug.Log("Camera stopped playing");
        }
        webcamTexture = null;
      //  SceneManager.LoadScene(0);
    }

}
