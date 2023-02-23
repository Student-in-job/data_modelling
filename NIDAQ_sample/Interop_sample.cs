using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace NIDAQ_sample
{
    class Interop_sample
    {
        public string DeviceChannel { get; set; }
        public bool Running { get; set; }
        private double[] signalData;
        private Thread myThread;
        
        [DllImport("NIDAQDevice.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void CreateChannel(string channelName, out string error, int min = -10, int max = 10);

        [DllImport("NIDAQDevice.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void StartTask(out string error);

        [DllImport("NIDAQDevice.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void WriteData(double[] data, out string error);

        [DllImport("NIDAQdevice.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void StopTask();

        [DllImport("NIDAQDevice.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void Dispose();

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

        public void RunWork()
        {
            string errorMessage;
            while (this.Running)
            {
                WriteData(this.signalData, out errorMessage);
                if (hasError(errorMessage)) break;
            }
        }

        public void WriteLoop(double [] data)
        {
            this.Running = true;
            this.signalData = data;
            myThread = new Thread(RunWork);
            myThread.Start();
        }

        public void StopLoop()
        {
            this.Running = false;
            myThread.Join();
        }

        public static bool hasError(string error)
        {
            if (error != string.Empty)
            {
                Console.WriteLine(error);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
