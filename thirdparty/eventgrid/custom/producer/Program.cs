using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace linuxacademy.az203.thidparty.eventgrid
{
    class Program
    {
        static void Main(string[] args)
        {
            var topicEndpoint = "https://laaz203egcustomtopic.westus-1.eventgrid.azure.net/api/events";
            var sasKey = "ic+d5gbA4osJyfkLC0E51p9bg5q84k1rqDsio2MrJD0=";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("aeg-sas-key", sasKey);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("democlient");

            var events = new List<GridEvent<LoginData>>()
            {
                new GridEvent<LoginData>
                {
                    Subject = "Login Event",
                    EventType = "allEvents",
                    EventTime = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString(),
                    Data = new LoginData
                    {
                        Username = "Mike1",
                    }
                },
                new GridEvent<LoginData>
                {
                    Subject = "Logout Event",
                    EventType = "allEvents",
                    EventTime = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString(),
                    Data = new LoginData
                    {
                        Username = "Mike2",
                    }
                }
                
            };

            var json = JsonConvert.SerializeObject(events);
            var request = new HttpRequestMessage(HttpMethod.Post, topicEndpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = client.SendAsync(request).Result;
            System.Console.WriteLine(response);

            events = new List<GridEvent<LoginData>>()
            {
                new GridEvent<LoginData>
                {
                    Subject = "Logout Event",
                    EventType = "allEvents",
                    EventTime = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString(),
                    Data = new LoginData
                    {
                        Username = "Mike",
                    }
                }
            };

            //response = client.SendAsync(request).Result;
            //System.Console.WriteLine(response);
        }
    }
}
