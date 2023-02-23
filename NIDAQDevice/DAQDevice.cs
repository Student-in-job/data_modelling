using System;
using NationalInstruments.DAQmx;

namespace NIDAQDevice
{
    public class DAQDevice
    {
        public static Task myTask;
        public static AnalogSingleChannelWriter writer = null;

        public static void CreateChannel(string channelName,out string error , int min = -10, int max = 10)
        {
            try
            {
                myTask = new Task();
                myTask.AOChannels.CreateVoltageChannel(channelName, "", min, max, AOVoltageUnits.Volts);
                myTask.Control(TaskAction.Verify);
                writer = new AnalogSingleChannelWriter(myTask.Stream);
                error = string.Empty;
            }
            catch (Exception e)
            {
                error = e.Message;
            }
        }

        public static void WriteData(double[] data, out string error)
        {
            try
            {
                writer.WriteMultiSample(true, data);
                error = string.Empty;
            }
            catch(Exception e)
            {
                error = e.Message;
            }
        }

        public static void StartTask(out string error)
        {
            try
            {
                myTask.Start();
                error = string.Empty;
            }
            catch (Exception e)
            {
                error = e.Message;
            }
        }

        public static void StopTask()
        {
            myTask.Stop();
        }

        public static void Dispose()
        {
            writer = null;
            myTask.Dispose();
        }
    }
}