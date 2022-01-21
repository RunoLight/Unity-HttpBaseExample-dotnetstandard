using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Light.Unity.AspNetCore.HTTP.Extensions
{
    public class HttpClientPool
    {
        private const byte MaxParallelRequestCount = byte.MaxValue;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);
        private readonly string baseUrl;
        private readonly SemaphoreSlim @lock = new SemaphoreSlim(MaxParallelRequestCount);
        private readonly TimeSpan timeout;

        private readonly ConcurrentDictionary<string, string> headerMap = new ConcurrentDictionary<string, string>();

        public HttpClientPool(string baseUrl, TimeSpan? requestTimeout = null)
        {
            this.baseUrl = baseUrl;
            timeout = requestTimeout ?? DefaultTimeout;
        }

        public async Task<HttpClient> GetClient(int timeout = 200, string baseUrl = null,
            TimeSpan? requestTimeout = null, HttpClientHandler customHandler = null)
        {
            if (!await @lock.WaitAsync(timeout)) return default;

            var client = new HttpClient(customHandler ?? new HttpClientHandler { AllowAutoRedirect = false })
            {
                BaseAddress = new Uri(baseUrl ?? this.baseUrl),
                Timeout = requestTimeout ?? this.timeout
            };
            foreach (var item in headerMap) client.DefaultRequestHeaders.Add(item.Key, item.Value);
            return client;
        }

        public void FreeClient(HttpClient client, HttpResponseMessage response = null)
        {
            response?.Dispose();
            client.Dispose();
            @lock.Release();
        }

        public void SetDefaultHeader(string key, string value)
        {
            if (value == null)
            {
                headerMap.TryRemove(key, out var d);
                return;
            }

            if (headerMap.TryAdd(key, value) == false) headerMap[key] = value;
        }
    }
}