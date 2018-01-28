using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TracertFetcher
{
    public enum DataType
    {
        Ping,
        Tracert,
        Premade
    }

    public static AsyncOperation GetData(DataType dataType, string ip, out string output)
    {
        var async = GetTracert(dataType, ip, out output);

        return async;
    }

    private static AsyncOperation GetTracert(DataType dataType, string ip, out string output)
    {
        var commandToRun = "ping";

        switch (dataType)
        {
            case DataType.Ping:
                commandToRun = "ping";
                break;
            case DataType.Tracert:
                commandToRun = "tracert";
                break;
            default:
                throw new ArgumentOutOfRangeException("dataType", dataType, null);
        }

        string maxWait = "50";

        bool resolveHostNames = false;

        Debug.Log("STARTING TRACERT: " + ip);

        Process pProcess = new Process();

        pProcess.StartInfo.FileName = commandToRun;
        pProcess.StartInfo.Arguments = (resolveHostNames ? "" : "-d") +
                                       " " +
                                       "-w " + maxWait +
                                       " " + ip;

        Debug.Log("Started command: " + pProcess.StartInfo.Arguments);

        pProcess.StartInfo.UseShellExecute = false;
        pProcess.StartInfo.CreateNoWindow = true;
        pProcess.StartInfo.RedirectStandardOutput = true;
        pProcess.StartInfo.WorkingDirectory = Application.dataPath.Replace("/Assets", "");
        pProcess.Start();

        string strOutput = pProcess.StandardOutput.ReadToEnd();

        pProcess.WaitForExit();

        output = strOutput;

        return null;
    }
}