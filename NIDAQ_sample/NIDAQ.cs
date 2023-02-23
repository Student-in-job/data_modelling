using System;
using DAQmx = NationalInstruments.DAQmx;
using System.Threading;

namespace NIDAQ_sample
{
    class NIDAQ
    {
        private DAQmx::Task myTask;
        int frequency = 60;
        private string channelName;
        public bool Started { get; set; }

        public NIDAQ(string channel)
        {
            this.channelName = channel;
            this.Started = false;
        }

        public NIDAQ(string channel, int freq)
        {
            this.channelName = channel;
            this.frequency = freq;
            this.Started = false;
        }

        public void CreateSignal()
        {
            try
            {
                myTask = new DAQmx.Task();
                myTask.AOChannels.CreateVoltageChannel(channelName, "", -10, 10, DAQmx.AOVoltageUnits.Volts);
                myTask.Control(DAQmx.TaskAction.Verify);
                DAQmx.AnalogSingleChannelWriter writer = new DAQmx.AnalogSingleChannelWriter(myTask.Stream);
                //writer.WriteMultiSample(true, GetSineWave());
                myTask.Start();
                while (this.Started)
                {
                    writer.WriteMultiSample(true, GetSineWave());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                myTask.Dispose();
            }
            finally
            { }
        }

        public void StartThread()
        {
            this.Started = true;
            Thread mythread = new Thread(CreateSignal);
            mythread.Start();
        }

        public double[] GetSineWave()
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

        public void StopSignal()
        {
            try
            {
                myTask.Stop();
                myTask.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
