using System;
using System.Net;
using System.Net.Sockets;

namespace listen
{
    class Program
    {
        static void usage()
        {
            Console.WriteLine(new string('=', 42));
            Console.WriteLine("listen v1.0, Kliment Andreev 2016");
            Console.WriteLine("Usage: listen -p <port>");
            Console.WriteLine("Opens a TCP socket on a given port");
            Console.WriteLine("CTRL + C to exit or type q from the client");
            Console.WriteLine(new string('=', 42));
        }

        static void Main(string[] args)
        {
            if ((args.Length == 0) || (args.Length != 2))
            {
                usage();
                return;
            }

            if (args[0]!="-p")
            {
                Console.WriteLine("Unknown switch {0}", args[0]);
                usage();
                return;
            }

            int intTemp;
            bool bisNumeric = int.TryParse(args[1], out intTemp);
            if (!bisNumeric)
            {
                usage();
                return;
            }
           
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port intTemp.
                Int32 intPort = intTemp;
                IPAddress localAddr = IPAddress.Parse("0.0.0.0");
                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, intPort);
                // Start listening for client requests.
                server.Start();
                // Buffer for reading data
                Byte[] bytes = new Byte[4096];
                
                String data = null;
                // Enter the listening loop.
                while (true)
                {
                    usage();
                    Console.WriteLine("I am listening on {0}... ", intPort);
                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("=====> Connected to {0}", client.Client.RemoteEndPoint);
                    data = null;
                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();
                    byte[] banner = System.Text.Encoding.ASCII.GetBytes(
                        "listen v1.0, Kliment Andreev 2016\r\nType q to exit\r\n" 
                        + "Connected to " + client.Client.LocalEndPoint + "\r\n");
                    stream.Write(banner, 0, banner.Length);
                    int intLoop;
                    // Loop to receive all the data sent by the client.
                    while ((intLoop = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, intLoop);
                        Console.WriteLine("Received: {0} from {1}", data, client.Client.RemoteEndPoint);
                        // Process the data sent by the client.
                        data = data.ToUpper();
                        if (data == "Q") return;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                        // Send back a response.                        
                        stream.Write(msg, 0, msg.Length);                        
                        Console.WriteLine("Sent: {0} to {1}", data, client.Client.RemoteEndPoint);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                //server.Stop();
            }
        }
    }
}
