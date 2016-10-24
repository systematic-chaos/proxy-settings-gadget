using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace NetworkSettings
{
    sealed class Connectivity
    {
        private static TextWriter _outputStream = null;

        public static void Ping(string who, TextWriter outputStream)
        {
            if (string.IsNullOrEmpty(who))
                throw new ArgumentException("Ping needs a host or IP Address.");

            AutoResetEvent waiter = new AutoResetEvent(false);

            Ping pingSender = new Ping();

            _outputStream = outputStream;

            // When the PingCompleted event is raised,
            // the PingCompletedCallback methods is called.
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Wait 12 seconds for a reply.
            int timeout = 12000;

            // Set options for transmission:
            // The data can go through 64 gateways or routers
            // before it is destroyed, and the data packet
            // cannot be fragmented.
            PingOptions options = new PingOptions(64, true);

            _outputStream.WriteLine("Time to live: {0}", options.Ttl);
            _outputStream.WriteLine("Don't fragment: {0}", options.DontFragment);

            // Send the ping asynchronously.
            // Use the waiter as the user token.
            // When the callback completes, it can wake up this thread.
            pingSender.SendAsync(who, timeout, buffer, options, waiter);

            // Prevent this example application from ending.
            // A real application should do somethind useful
            // when possible.
            waiter.WaitOne();
            _outputStream.WriteLine("Ping example completed");
        }

        private static void PingCompletedCallback(object sender,
            PingCompletedEventArgs e)
        {
            // If the operation was cancelled, display a message to the user.
            if (e.Cancelled)
            {
                _outputStream.WriteLine("Ping cancelled.");

                // Let the main thread resume.
                // UserToken is the AutoResetEvent object that the main thread
                // is waiting for.
                ((AutoResetEvent)e.UserState).Set();
            }

            // If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                _outputStream.WriteLine("Ping failed:");
                _outputStream.WriteLine(e.Error.ToString());

                // Let the main thread resume.
                ((AutoResetEvent)e.UserState).Set();
            }

            PingReply reply = e.Reply;

            DisplayReply(reply);

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
        }

        private static void DisplayReply(PingReply reply)
        {
            if (reply == null)
                return;

            _outputStream.WriteLine("ping status: {0}", reply.Status);
            if (reply.Status == IPStatus.Success)
            {
                _outputStream.WriteLine("Address: {0}", reply.Address.ToString());
                _outputStream.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                _outputStream.WriteLine("Time to live: {0}", reply.Options.Ttl);
                _outputStream.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                _outputStream.WriteLine("Buffer size: {0}", reply.Buffer.Length);
            }
        }
    }
}
