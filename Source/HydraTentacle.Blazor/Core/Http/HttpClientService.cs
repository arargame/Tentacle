using System.Net.Http.Json;

namespace HydraTentacle.Blazor.Core.Http
{
    public class HttpClientService
    {
        protected readonly HttpClient _client;

        public HttpClientService(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            return await _client.GetFromJsonAsync<T>(url)
                   ?? throw new Exception("Null response");
        }

        public async Task PostAsync<T>(string url, T data)
        {
            await _client.PostAsJsonAsync(url, data);
        }
    }
}
