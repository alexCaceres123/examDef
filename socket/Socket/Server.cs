using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Scrabble
{

    class Server
    {
        private string ip;
        private int port;
        private static AccionsHandler accions;
        public static string END = "--END--";

        public Server(string ip, int port) {
            this.ip = ip;
            this.port = port;
            accions = new AccionsHandler();
        }

        public async Task start()
        {
            TcpListener server = null;

            try
            {

                IPAddress ipAddr = IPAddress.Parse(this.ip);
                server = new TcpListener(ipAddr, this.port);
                server.Start();
                Console.WriteLine($"Server listening on {ipAddr}:{this.port}");

                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected!");
                    _ = Task.Run(() => HandleClientAsync(client));
                }

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            } finally
            {
                server?.Stop();
            }
        }

        async Task HandleClientAsync(TcpClient client)
        {

            NetworkStream stream = client.GetStream();

            try
            {

                byte[] buffer = new byte[1024];
                StringBuilder receivedData = new StringBuilder();

                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    receivedData.Append(data);
                    string rd = receivedData.ToString();
                    int index = rd.IndexOf(END);

                    if (index != -1)
                    {
                        string tmp = ""; 
                        if(index + END.Length < data.Length) data.Substring(index + END.Length, data.Length - index - END.Length);
                        receivedData.Clear();
                        if(tmp.Length > 0) receivedData.Append(tmp);

                        string json = rd.Substring(0, index);
                        string response = await accions.handle(json, client);

                        if (response != null)
                        {
                            byte[] reply = Encoding.UTF8.GetBytes(response + END);
                            await stream.WriteAsync(reply, 0, reply.Length);
                        }

                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                stream.Close();
                client.Close();
            }

        }

    }

}