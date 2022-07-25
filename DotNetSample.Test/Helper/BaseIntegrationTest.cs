using DotNetSample.Data;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DotNetSample.Test.Helper
{
    public static class ExtensionMethods
    {
        public static async Task<V?> GetAsync<V>(this HttpClient client, string requestUri)
        {
            var result = await client.GetAsync(requestUri);
            return result.IsSuccessStatusCode ? await result.Content.ReadAsAsync<V>() : default;
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, string requestUri, T payload)
        {
            return await client.PostAsync(requestUri, JsonContent.Create(payload));
        }

        public static async Task<V?> PostAsyncAndReturn<T, V>(this HttpClient client, string requestUri, T payload)
        {
            var result = await client.PostAsync(requestUri, JsonContent.Create(payload));
            return result.IsSuccessStatusCode ? await result.Content.ReadAsAsync<V>() : default;
        }
    }

    public abstract class BaseIntegrationTest : IDisposable
    {
        TestApp App;
        public HttpClient TestClient;

        public BaseIntegrationTest(string env, Func<AppDbContext, bool> seed)
        {
            App = new TestApp(env, seed);
            TestClient = App.CreateClient();
        }

        public void Dispose()
        {
            TestClient.Dispose();
            App.Dispose();
        }
    }
}
