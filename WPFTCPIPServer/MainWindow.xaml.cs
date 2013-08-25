using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFTCPIPServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
        private long lastUpdateMilliSeconds;

        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            stopwatch.Start();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //if (stopwatch.ElapsedMilliseconds > lastUpdateMilliSeconds + 1000)
            //{
            //    plot1.RefreshPlot(true);
            //    lastUpdateMilliSeconds = stopwatch.ElapsedMilliseconds;
            //    //Without this 1000 ms delay after some time the program throws exception: 
            //    //"Collection was modified; enumeration operation may not execute"
            //    //It is probably due to too fast plot1 refresh rate.
            //    //In general this delay should be correlated with the client data sending rate.
            //}
        }
    }
}
