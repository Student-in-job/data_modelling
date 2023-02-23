using System;

namespace NIDAQ_sample
{
    class Program
    {
        static void Main(string[] args)
        {
            string channelName = "Dev2/ao1";
            int frequency = 80;

            //TryNI(channelName, frequency);
            TryLib(channelName, frequency);
        }

        public static void TryNI(string channelName, int frequency)
        {
            Console.WriteLine("Starting generate a signal");
            NIDAQ daqDevice = new NIDAQ(channelName, frequency);
            daqDevice.StartThread();
            Console.WriteLine("Press any key to stop");
            ConsoleKeyInfo key = Console.ReadKey();
            daqDevice.Started = false;
            Console.WriteLine("{0} key have been pressed", key.KeyChar);
            daqDevice.StopSignal();
        }

        public static void TryInterop(string channelName, int frequency)
        {
            
            Console.WriteLine("Starting generate a signal");
            Interop_sample daqDevice = new Interop_sample { DeviceChannel = channelName};
            //daqDevice.DeviceChannel = channelName;
            string errorMessage = string.Empty;
            
            Interop_sample.CreateChannel(daqDevice.DeviceChannel, out errorMessage, -10, 10);
            if (Interop_sample.hasError(errorMessage)) return;
            
            Interop_sample.StartTask(out errorMessage);
            if (Interop_sample.hasError(errorMessage)) return;

            double[] values = Interop_sample.GetSineWave(frequency);
            daqDevice.WriteLoop(values);

            Console.WriteLine("Press any key to stop");
            ConsoleKeyInfo key = Console.ReadKey();
            daqDevice.StopLoop();
            Console.WriteLine("{0} key have been pressed", key.KeyChar);
        }

        public static void TryLib(string channelName, int frequency)
        {
            Console.WriteLine("Starting generate a signal");
            DAQDevice daqDevice = new DAQDevice { DeviceChannel = channelName };
            string errorMessage = string.Empty;

            daqDevice.CreateChannel(out errorMessage);
            if (DAQDevice.hasError(errorMessage)) return;
            daqDevice.StartTask(out errorMessage);
            if (DAQDevice.hasError(errorMessage)) return;

            double[] values = DAQDevice.GetSineWave(frequency);
            daqDevice.WriteLoop(values);
            Console.WriteLine("Press any key to stop");
            ConsoleKeyInfo key = Console.ReadKey();
            daqDevice.StopLoop();
            Console.WriteLine("{0} key have been pressed", key.KeyChar);
        }
    }
}
