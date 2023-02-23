using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NIDAQDevice;
using System.Threading;
using ni = NIDAQDevice;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DAQDevice : MonoBehaviour
{
    [Header("Configuration Attributes")]
    public string DeviceChannel = "Dev2/ao1";
    public int Frequency;
    public bool Running;
    private double[] signalData;
    private Thread myThread;
    private bool vibrationOn;

    void Start()
    {
        string errorMessage;
        signalData = DAQDevice.GetSineWave(this.Frequency);
        this.CreateChannel(out errorMessage);
    }

    // Update is called once per frame
    void Update()
    {
        string errorMessage;
        if (Input.GetKeyDown("v"))
        {
            vibrationOn = !vibrationOn;

            //If we've just turned it ON
            if (vibrationOn)
            {
                StartTask(out errorMessage);
                if (DAQDevice.hasError(errorMessage))
                    vibrationOn = false;
                WriteLoop();
            }
            else
                StopLoop();
        }
    }

    private void CreateChannel(out string errorMessage, int min = -10, int max = 10)
    {
        ni.DAQDevice.CreateChannel(DeviceChannel, out errorMessage, min, max);
    }

    private void StartTask(out string error)
    {
        ni.DAQDevice.StartTask(out error);
        if (DAQDevice.hasError(error))
            Running = false;
        else
            Running = true;
    }
    private void StopLoop()
    {
        if (!this.Running)
            return;
        myThread.Join();
        this.Running = false;
    }

    private void RunWork()
    {
        string errorMessage;
        while (this.Running)
        {
            ni.DAQDevice.WriteData(this.signalData, out errorMessage);
            if (DAQDevice.hasError(errorMessage)) break;
        }
    }

    private void WriteLoop()
    {
        myThread = new Thread(RunWork);
        myThread.Start();
        this.Running = true;
    }

    public static double[] GetSineWave(int frequency)
    {
        int amplitude = 9;
        int len = 1000;
        double step = 2 * Math.PI / len;
        double[] res = new double[len];
        for (int i = 0; i < len; i++)
        {
            res[i] = amplitude * Math.Sin(step * i * frequency);
        }
        return res;
    }

    public static bool hasError(string error)
    {
        if (error != string.Empty)
        {
            Debug.LogError(error);
            return true;
        }
        else
        {
            return false;
        }
    }
}