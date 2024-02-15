using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;
using System.IO;

namespace Scrabble
{
    class AccionsHandler
    {
        private static string BASE_URL = "http://localhost:4300";

        public AccionsHandler() { 
        }

        public async Task<string> handle(string json, TcpClient conn)
        {
            SampleRequest req = parseRequest(json);
            Console.WriteLine(json + " " + req.msg);

            switch (req.msg) {
                case "Hello":
                default:
                    return handleDefault();
            }
        
        }

        private string handleDefault() {
            var data = new SampleResponseData()
            {
                ok = false,
                error = "Action not found!"
            };
            var res = new SampleRequest() { 
                msg = "Error",
                data = JsonConvert.SerializeObject(data)
            };

            return JsonConvert.SerializeObject(res);
        }
       

        private string generateResponse<T>(string msg, T data) {
            var res = new SampleResponse()
            {
                msg = msg,
                data = JsonConvert.SerializeObject(data)
            };

            return JsonConvert.SerializeObject(res);
        }

        private string generateSampleResponse(string msg, bool ok, string error = null) {
            var data = new SampleResponseData() { 
                ok = ok,
                error = (error == null) ? "" : error
            };

            var res = new SampleRequest()
            {
                msg = msg,
                data = JsonConvert.SerializeObject(data)
            };

            return JsonConvert.SerializeObject(res);
        }

        

        private SampleRequest parseRequest(string json) {
            return JsonConvert.DeserializeObject<SampleRequest>(json);
        }

        private SampleRquestAuth getAuth(SampleRequest req) { 
            return JsonConvert.DeserializeObject<SampleRquestAuth>(req.auth);
        }

        private T getData<T>(SampleRequest req)
        {
            return JsonConvert.DeserializeObject<T>(req.data);
        }

        private async void writeConn(TcpClient conn, string str) {
            try
            {
                var stream = conn.GetStream();
                byte[] reply = Encoding.UTF8.GetBytes(str + Server.END);
                await stream.WriteAsync(reply, 0, reply.Length);
                Console.WriteLine("Send");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to stream: {ex.Message}");
            }
        }

        private static async Task<string> makeGet(string path)
        {

            string url = BASE_URL + path;

            using (HttpClient client = new HttpClient())
            {

                HttpResponseMessage response = await client.GetAsync(url);

                // Check if the request was successful (status code 200)
                if (response.IsSuccessStatusCode)
                {
                    // Read and output the response content
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

        private static async Task<string> makePost(string path, string json)
        {
            using (HttpClient client = new HttpClient())
            {
                // The URL to which you want to make the POST request
                string url = BASE_URL + path;

                // Convert the data to a StringContent
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // Make the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string result = await response.Content.ReadAsStringAsync();

                    // Process the result as needed
                    Console.WriteLine($"Response: {result}");
                    return result;
                }
                else
                {
                    // Handle unsuccessful request
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }

            return "{}";
        }

    }

    // Server Request DTOs

    class SampleRequest {
        public string msg { get; set; }
        public string auth { get; set; } // json
        public string? data { get; set; } // json
    }

    class SampleRquestAuth
    {
        public string nom { get; set; }
        public long data { get; set; }
    }

    // Server Response DTOs

    class SampleResponse {
        public string msg { get; set; }
        public string data { get; set; } // json
    }

    class SampleResponseData {
        public bool ok { get; set; }
        public string error { get; set; }
    }

    // Custom Data DTOs

    

}
