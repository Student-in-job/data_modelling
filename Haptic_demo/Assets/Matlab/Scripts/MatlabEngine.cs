using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MathWorks.MATLAB.Engine;
using MathWorks.MATLAB.Types;
using System.IO;
using UnityEngine.UI;
using System;

public class MatlabEngine : MonoBehaviour
{
    dynamic engine;
    double[] rawArray;
    bool dataRetrieved = false;
    public GameObject pointPrefab;
    public GameObject text;

    private void plot()
    {
        if (!dataRetrieved)
            return;
        double[] xArray = new double[rawArray.Length];
        for (int i = 0; i < rawArray.Length; i++)
        {
            xArray[i] = i;
        }

        Transform pointPrefab = GameObject.Find("point").GetComponent<Transform>();
        Vector3 position;
        for (int i = 0; i < xArray.Length; i++)
        {
            Transform point = Instantiate(pointPrefab); // a sphere
            position.x = (float)xArray[i] - 100;
            position.y = (float)rawArray[i];
            position.z = 100;
            point.localPosition = position;
            point.SetParent(transform, false);
        }
    }

    // Start is called before the first frame update
    async void Start()
    {
        TextMesh txtMesh = text.GetComponent<TextMesh>();
        txtMesh.text = "Starting matlab";
        Debug.Log("Starting Matlab...");
        await StartMatlabAsync();
        txtMesh.text = "Matlab have started asynchronously successfully!";
    }

    // Update is called once per frame
    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            try
            {
                rawArray = await CalculateFunc();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                TerminateMatlab();
            }
            dataRetrieved = true;
            plot();
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }

    private void TerminateMatlab()
    {
        engine.Dispose();
        MATLABEngine.TerminateEngineClient();
    }

    private async Task StartMatlabAsync()
    {
        engine = await MATLABEngine.StartMATLABAsync();
    }

    private async Task<double[]> CalculateFunc()
    {
        string dir = System.IO.Directory.GetCurrentDirectory();
        Debug.Log(dir);
        RunOptions opts = new RunOptions(async: true);
        string command = @"'cd(''" + dir + "'')'";
        engine.eval(command);
        string mes = engine.eval("pwd");
        Debug.Log(mes);
        double[] retVal = await engine.dft321(opts);
        
        return retVal;
    }

    private void OnApplicationQuit()
    {
        
        TerminateMatlab();
    }
}
