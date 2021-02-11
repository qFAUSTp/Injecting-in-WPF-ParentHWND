using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryParentHWND.Methods
{
    class PipeServer
    {
        private static string temp = "";
        private static StreamWriter streamWriter;

        public static string GetMessage()
        {
            return temp;
        }

        public static void SendMessage(string message)
        {
            streamWriter.WriteLine(message);
        }

        public static void SartDuplexPipe(string name)
        {
            StartServer(name + "_to_wpf");
            StartClient(name + "_to_unity");
        }

        private static void StartClient(string pipeName)
        {
            Console.WriteLine("Creating client pipe");
            NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
            Console.WriteLine("Connecting to server...");
            clientPipe.Connect();
            Console.WriteLine("Connection succeeded!");
            streamWriter = new StreamWriter(clientPipe);
            streamWriter.AutoFlush = true;      
        }
        private static void StartServer(string pipeName)
        {
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Creating server pipe");
                NamedPipeServerStream serverPipe = new NamedPipeServerStream(pipeName, PipeDirection.In);
                Console.WriteLine("Waiting for connection...");
                serverPipe.WaitForConnection();
                Console.WriteLine("Connection successful.");
                using (StreamReader streamReader = new StreamReader(serverPipe))
                {
                    while (true)
                    {
                        while ((temp = streamReader.ReadLine()) != null)
                        {
                            Console.WriteLine("Received from server: {0}", temp);
                        }
                    }
                }
            });
        }
    }
}
