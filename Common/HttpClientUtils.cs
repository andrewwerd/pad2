using System.Text;

namespace Common
{
    public class HttpClientUtils
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static HttpResponseMessage SendJson(string json, string url, string method)
        {
            var httpMethod = new HttpMethod(method.ToUpper());
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(httpMethod, url)
            {
                Content = content
            };

            var task = _httpClient.SendAsync(request);
            task.Wait();

            return task.Result;
        }
    }
}
