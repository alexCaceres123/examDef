using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace SpotifyWPF.lib
{

    internal class ActionsHandler
    {

        private NetworkStream stream;

        public ActionsHandler(NetworkStream networkStream)
        {
            this.stream = networkStream;
        }

        public void handle(string json)
        {

            var res = this.parseResponse(json);

            switch (res.msg)
            {
                case "HelloAdmin":
                    handleHelloAdmin(res);
                    break;
            }

        }

        private void handleHelloAdmin(Response res)
        {
               
        }

      
        private void sendData(NetworkStream networkStream, string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                networkStream.Write(data, 0, data.Length);
                Console.WriteLine($"Sent: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data: {ex.Message}");
            }
        }

        

        private string generateRequest<T>(string msg, T data)
        {
            var res = new Request()
            {
                msg = msg,
                data = JsonConvert.SerializeObject(data)
            };

            return JsonConvert.SerializeObject(res);
        }

        private Response parseResponse(string res)
        {
            return JsonConvert.DeserializeObject<Response>(res);
        }

        private T parseData<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

    }

    class Response
    {
        public string msg { get; set; }
        public string data { get; set; } 
    }

    class Request
    {
        public string msg { get; set; }
        public string data { get; set; } 
    }
}

