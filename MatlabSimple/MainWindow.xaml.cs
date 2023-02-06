using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
using MathWorks.MATLAB.Engine;
using MathWorks.MATLAB.Types;

namespace MatlabSimple
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        dynamic engine;
        double[] rawArray;
        bool dataRetrieved = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task StartMatlabAsync()
        {
            engine = await MATLABEngine.StartMATLABAsync();
        }

        private async void btnStartMatlab_Click(object sender, RoutedEventArgs e)
        {
            txtOutput.Text = "Starting Matlab...";
            await StartMatlabAsync();
            txtOutput.Text = "Matlab have started asynchronously successfully!";
            btnCalcData.IsEnabled = true;
        }

        private async Task<double[]> CalculateFunc()
        {
            string dir = System.IO.Directory.GetCurrentDirectory();
            RunOptions opts = new RunOptions(async: true);
            string command = @"'cd(''" + dir + "'')'";
            engine.eval(command);
            double[] retVal = await engine.dft321(opts);
            return retVal;
        }

        private void PlotData()
        {
            double[] xArray = new double[rawArray.Length];
            for (int i = 0; i < rawArray.Length; i++)
            {
                xArray[i] = i;
            }
            Plot pl = new Plot(cvPlot);
            pl.Clean();
            pl.PlotData(xArray, rawArray);
            txtOutput.Text = string.Format("Number of returned values {0}", rawArray.Length.ToString());
            txtOutput.Text = string.Format("Canvas size - x:{0}, y:{1}", pl.Width.ToString(), pl.Height.ToString());
        }

        private async void btnCalcData_Click(object sender, RoutedEventArgs e)
        {
            rawArray = await CalculateFunc();
            dataRetrieved = true;
            PlotData();
        }

        public void Dispose()
        {
            engine.Dispose();
            MATLABEngine.TerminateEngineClient();
        }

        private void cvPlot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (dataRetrieved)
            {
                PlotData();
            }
        }
    }
}
