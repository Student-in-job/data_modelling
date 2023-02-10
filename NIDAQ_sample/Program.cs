using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DAQmx = NationalInstruments.DAQmx;

namespace NIDAQ_sample
{
    class Program
    {
        private DAQmx::Task myTask;
        private string channelName = "Dev2/ao1";
        private bool started = false;
        int frequency = 60;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting generate a signal");
            Program p = new Program();
            p.StartThread();
            Console.WriteLine("Press any key to stop");
            ConsoleKeyInfo key = Console.ReadKey();
            p.started = false;
            Console.WriteLine("{0} key have been pressed", key.KeyChar);
            p.StopSignal();
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
                while (started)
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
            started = true;
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
