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
    public int gain = 5;
    public float shrink = 2.5f;

    private void Plot()
    {
        if (!dataRetrieved)
            return;
        double[] xArray = new double[rawArray.Length];
        for (int i = 0; i < rawArray.Length; i++)
        {
            xArray[i] = i;
        }

        Transform MatlabObject = GameObject.Find("Matlab").GetComponent<Transform>();
        int newpoints = rawArray.Length - MatlabObject.childCount;
        int j = 0;
        Vector3 position;
        Transform pointPrefab = GameObject.Find("point").GetComponent<Transform>();
        for (int i = 0; i < MatlabObject.childCount; i++)
        {
            Transform point = MatlabObject.GetChild(i);
            position.x = (float)xArray[j]/shrink - 100;
            position.y = gain * (float)rawArray[i];
            position.z = 100;
            point.localPosition = position;
            j++;
        }        
        for (int i = 0; i < newpoints; i++)
        {
            Transform point = Instantiate(pointPrefab); // a sphere
            position.x = (float)xArray[j] / shrink - 100;
            position.y = gain * (float)rawArray[j];
            position.z = 100;
            point.localPosition = position;
            point.SetParent(transform, false);
            j++;
        }
    }

    private void CleanPlot()
    {
        Vector3 position = GameObject.Find("point").transform.position;
        Transform parentObject = GameObject.Find("Matlab").GetComponent<Transform>();
        for (int i = 0; i < parentObject.childCount; i++)
        {
            Transform transform = parentObject.GetChild(i);
            transform.position = position;
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
                //TerminateMatlab();
            }
            dataRetrieved = true;
            Plot();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CleanPlot();
        }
    }

    private async Task StartMatlabAsync()
    {
        try
        {
            engine = await MATLABEngine.StartMATLABAsync();
        }
        catch (MathWorks.MATLAB.Exceptions.MATLABNotAvailableException exp)
        {
            Debug.Log(exp.Message);
        }
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
}
