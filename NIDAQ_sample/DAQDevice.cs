using System;
using System.Threading;
using ni = NIDAQDevice;

namespace NIDAQ_sample
{
    class DAQDevice:IDisposable
    {
        public string DeviceChannel { get; set; }
        public bool Running { get; set; }
        private double[] signalData;
        private Thread myThread;

        public void CreateChannel(out string errorMessage, int min = -10, int max = 10)
        {
            ni.DAQDevice.CreateChannel(DeviceChannel, out errorMessage, min, max);
        }

        public void StartTask(out string error)
        {
            ni.DAQDevice.StartTask(out error);
        }

        public void Dispose()
        {
            ni.DAQDevice.Dispose();
            myThread = null;
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

        public void RunWork()
        {
            string errorMessage;
            while (this.Running)
            {
                ni.DAQDevice.WriteData(this.signalData, out errorMessage);
                if (hasError(errorMessage)) break;
            }
        }

        public void WriteLoop(double[] data)
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
