using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using WpfApplicationPropertyChanged;
using System.Collections.ObjectModel;
using System.Globalization;
using MathNet.Numerics.Statistics;

namespace WPFTCPIPServer
{

    [ImplementPropertyChanged]
    public class ViewModel
    {
        int x = 0;
        bool DisableStartServerButton = false;
        bool DisableStopServerButton = true;
        bool StopListeiningWhileLoop = false;

        public string Temperature { get; set; }
        public string ServerStatus { get; set; }

        private List<double> TemperatureList = new List<double>();

        public Collection<CollectionDataValue> Data { get; set; }

        public class CollectionDataValue
        {
            public double xData { get; set; }
            public double yData { get; set; }
        }

        public ViewModel()
        {
            ServerStatus = "Press Start server button to start the TCP/IP server";
            Data = new Collection<CollectionDataValue>();
        }

        private void StartServer()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("192.168.2.101");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop. 
                while (true)
                {
                    if (StopListeiningWhileLoop == true)
                    {
                        break;
                    }
                    else
                    {
                        ServerStatus = "Waiting for a connection... ";

                        // Perform a blocking call to accept requests. 
                        // You could also user server.AcceptSocket() here.
                        TcpClient client = server.AcceptTcpClient();
                        ServerStatus = "Connected!";

                        data = null;

                        // Get a stream object for reading and writing
                        NetworkStream stream = client.GetStream();

                        int i;

                        // Loop to receive all the data sent by the client. 
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            string TemperatureString;
                            double TemperatureDouble;
                            DescriptiveStatistics descrStat = new DescriptiveStatistics(TemperatureList);
                            double dKurtosis = descrStat.Kurtosis;
                            double dSkewness = descrStat.Skewness;

                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            TemperatureString = data;
                            TemperatureDouble = double.Parse(TemperatureString, NumberStyles.Float, CultureInfo.InvariantCulture);



                            Data.Add(new CollectionDataValue { xData = x, yData = TemperatureDouble });
                            TemperatureList.Add(TemperatureDouble);

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            //Send back a response.
                            stream.Write(msg, 0, msg.Length);

                            ServerStatus = "Receiving temperature data and sending response to the Client.";

                            #region TemperatureStatistics
                            Temperature = "Current temperature: " + String.Format("{0:0.0000}", TemperatureDouble) + " [°C]" + Environment.NewLine +
                            "Mean: " + String.Format("{0:0.0000}", TemperatureList.Mean()) + " [°C]" + Environment.NewLine +
                            "Median: " + String.Format("{0:0.0000}", TemperatureList.Median()) + " [°C]" + Environment.NewLine +
                            "Standard deviation: " + String.Format("{0:0.0000}", TemperatureList.StandardDeviation()) + " [°C]" + Environment.NewLine +
                            "3x Standard deviation: " + String.Format("{0:0.0000}", 3 * TemperatureList.StandardDeviation()) + " [°C]" + Environment.NewLine +
                            "Max: " + String.Format("{0:0.0000}", TemperatureList.Max()) + " [°C]" + Environment.NewLine +
                            "Min: " + String.Format("{0:0.0000}", TemperatureList.Min()) + " [°C]" + Environment.NewLine +
                            "Kurtosis: " + String.Format("{0:0.0000}", descrStat.Kurtosis) + "\r\n" +
                            "Skewness: " + String.Format("{0:0.0000}", descrStat.Skewness) + "\r\n" +
                            "Sample number: " + Convert.ToString(x);
                            #endregion

                            x++;
                        }

                        // Shutdown and end connection
                        client.Close();
                    }
                }
                
                ServerStatus = "You may now close the program";
            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        #region Commands

        /// <summary>
        /// Start Server button
        /// </summary>
        void UpdateControlExecute()
        {
            Thread ServerThread = new Thread(new ThreadStart(StartServer));
            ServerThread.Start();
            DisableStartServerButton = true;
            DisableStopServerButton = false;
        }

        bool CanUpdateControlExecute()
        {
            if (DisableStartServerButton == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public ICommand StartServerButton
        {
            get { return new RelayCommand(UpdateControlExecute, CanUpdateControlExecute); }
        }


        /// <summary>
        /// Stop Server button
        /// </summary>
        void UpdateStopServerButtonExecute()
        {
            Thread ServerThread = new Thread(new ThreadStart(StartServer));
            ServerThread.Start();
            DisableStartServerButton = true;
            StopListeiningWhileLoop = true;
        }

        bool CanUpdateStopServerButtonExecute()
        {
            if (DisableStopServerButton == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand StopServerButton
        {
            get { return new RelayCommand(UpdateStopServerButtonExecute, CanUpdateStopServerButtonExecute); }
        } 
        
        #endregion
    }
}
