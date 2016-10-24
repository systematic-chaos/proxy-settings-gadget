using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkSettings
{
    class NetworkInformation
    {
        public static void DisplayNetworkInterfaces(TextWriter outputStream)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface n in adapters)
            {
                outputStream.WriteLine("      {0} is {1}", n.Name, n.OperationalStatus);
            }
        }

        public static void DisplayTcpStatistics(NetworkInterfaceComponent version, TextWriter outputStream)
        {
            IPGlobalProperties properties =
                IPGlobalProperties.GetIPGlobalProperties();
            TcpStatistics tcpstat = null;
            outputStream.WriteLine("");
            switch (version)
            {
                case NetworkInterfaceComponent.IPv4:
                    tcpstat = properties.GetTcpIPv4Statistics();
                    outputStream.WriteLine("TCP/IPv4 Statistics:");
                    break;
                case NetworkInterfaceComponent.IPv6:
                    tcpstat = properties.GetTcpIPv6Statistics();
                    outputStream.WriteLine("TCP/IPv6 Statistics:");
                    break;
                default:
                    throw new ArgumentException("version");
            }
            outputStream.WriteLine("  Minimum Transmission Timeout. : {0}",
                tcpstat.MinimumTransmissionTimeout);
            outputStream.WriteLine("  Maximum Transmission Timeout : {0}",
                tcpstat.MaximumTransmissionTimeout);

            outputStream.WriteLine(" Connection Data:");
            outputStream.WriteLine("      Current : {0}",
                tcpstat.CurrentConnections);
            outputStream.WriteLine("      Cumulative : {0}",
                tcpstat.CumulativeConnections);
            outputStream.WriteLine("      Initiated : {0}",
                tcpstat.ConnectionsInitiated);
            outputStream.WriteLine("      Accepted : {0}",
                tcpstat.ConnectionsAccepted);
            outputStream.WriteLine("      Failed Attempts : {0}",
                tcpstat.FailedConnectionAttempts);
            outputStream.WriteLine("      Reset : {0}",
                tcpstat.ResetConnections);

            outputStream.WriteLine("");
            outputStream.WriteLine("  Segment Data:");
            outputStream.WriteLine("      Received  ................... : {0}",
                tcpstat.SegmentsReceived);
            outputStream.WriteLine("      Sent : {0}",
                tcpstat.SegmentsSent);
            outputStream.WriteLine("      Retransmitted : {0}",
                tcpstat.SegmentsResent);

            outputStream.WriteLine("");
        }
    }
}
