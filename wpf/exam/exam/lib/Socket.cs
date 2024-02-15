using SpotifyWPF.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace exam.lib
{
    class Client
    {
        private static string BASE_URL = "http://localhost:4300";
        private static string END = "--END--";
        private const int BufferSize = 1024;
        public ActionsHandler actions;


        public Client()
        {

        }

        public void connect(string ip, int port)
        {
            try
            {
                TcpClient client = new TcpClient(ip, port);

                NetworkStream networkStream = client.GetStream();
                this.actions = new ActionsHandler(networkStream);

                Thread receiveThread = new Thread(() => ReceiveData(networkStream));
                receiveThread.Start();

                Console.ReadLine(); // Keep the console application running
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void ReceiveData(NetworkStream networkStream)
        {
            try
            {
                StringBuilder receivedData = new StringBuilder();
                byte[] buffer = new byte[BufferSize];
                int bytesRead;

                while ((bytesRead = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    receivedData.Append(receivedMessage);
                    int index = receivedMessage.IndexOf(END);

                    if (index != -1)
                    {
                        string json = receivedData.ToString().Substring(0, index);
                        actions.handle(json);
                        receivedData.Clear();
                    };
                };


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data: {ex.Message}");
            }
        }

        public static async Task<string> makeGet(string path)
        {

            string url = BASE_URL + path;

            using (HttpClient client = new HttpClient())
            {

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                    return content;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }

            return "[]";
        }

    }
}
