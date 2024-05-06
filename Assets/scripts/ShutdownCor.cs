using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShutdownCor : MonoBehaviour
{
    public JZShutdown_111 jz;
    public Text text;
    readonly int shutdownTimer = 10;
    int shutdownValue = 10;
    void Start()
    {
        text.enabled = false;
        shutdownValue = shutdownTimer;

#if UNITY_STANDALONE_WIN
        Cursor.visible = false;
#endif
#if UNITY_STANDALONE_LINUX
        Cursor.visible = false;
#endif
#if UNITY_EDITOR
        Cursor.visible = true;
#endif
    }

    public IEnumerator Shutdown()
    {
        shutdownValue = shutdownTimer;
        while (shutdownValue > 0)
        {
            text.text = shutdownValue.ToString();
            yield return new WaitForSeconds(1);
            shutdownValue--;
        }
        jz.SetPPPower("power", "off");
        System.Diagnostics.Process.Start("/home/shutdown.sh");
    }

    public void StartShutdownTimer()
    {
        StartCoroutine(Shutdown());
    }
    public void StopShutdownTimer()
    {
        StopAllCoroutines();
        text.enabled = false;
    }
}
