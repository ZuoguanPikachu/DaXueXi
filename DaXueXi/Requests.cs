using System.Collections.Generic;
using System.Net.Http;

namespace DaXueXi
{
    internal class Requests
    {
        private HttpClient client;

        public Requests()
        {
            HttpClientHandler handler = new HttpClientHandler();
            client = new HttpClient(handler);
        }
        public HttpContent get(string url, Dictionary<string, string> headers)
        {
            client.DefaultRequestHeaders.Clear();
            foreach (KeyValuePair<string, string> item in headers)
            {
                client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }

            HttpResponseMessage response = client.GetAsync(url).Result;
            HttpContent content = response.Content;

            return content;
        }
    }
}
