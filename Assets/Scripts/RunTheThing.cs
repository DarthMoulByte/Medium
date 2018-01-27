using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RunTheThing : MonoBehaviour
{
    private void OnEnable()
    {
        //StartCoroutine(GetTracert("www.nrk.no"));
    }

    public IEnumerator GetTracert(string ip)
    {
        Debug.Log("STARTING TRACERT: " + ip);

        Process pProcess = new Process();

        pProcess.StartInfo.FileName = "ping";
        pProcess.StartInfo.Arguments = ip;

        pProcess.StartInfo.UseShellExecute = false;
        pProcess.StartInfo.CreateNoWindow = true;
        pProcess.StartInfo.RedirectStandardOutput = true;
        pProcess.StartInfo.WorkingDirectory = Application.dataPath.Replace("/Assets", "");
        pProcess.Start();

        string strOutput = pProcess.StandardOutput.ReadToEnd();

//        pProcess.WaitForExit();

//        strOutput = strOutput.Replace("\n", "");
        Debug.Log(strOutput);

        yield return null;

    }
}