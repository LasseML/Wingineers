using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Wingineers.Models;
using Wingineers.Services.WingineersApi.Models;
using static System.Net.WebRequestMethods;

namespace Wingineers.Services.WingineersApi
{
    public class WingineersApiService(
        HttpClient client,
        IMemoryCache cache,
        IOptionsMonitor<WingineersOptions> options,
        ILogger<WingineersApiService> logger)
    {
        private readonly string Version = "1";

        public async Task<List<SalesPersonDto>?> GetSalesPersonsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCachedAsync<List<SalesPersonDto>>($"api/v{Version}/SalesPersons", cancellationToken);
        }

        public async Task<List<OrderLineDto>?> GetOrderLinesAsync(CancellationToken cancellationToken = default)
        {
          return await GetCachedAsync<List<OrderLineDto>>($"api/v{Version}/OrderLines", cancellationToken);
        }

        private async Task<T?> GetCachedAsync<T>(string url, CancellationToken cancellationToken) where T : class
        {
            string key = $"wingineers:{url}";

            if (cache.TryGetValue(key, out T? cached))
            {
                return cached;
            }

            try
            {
                using HttpResponseMessage response = await client.GetAsync(url, cancellationToken);

                response.EnsureSuccessStatusCode();

                T? content = await response.Content.ReadFromJsonAsync<T>(cancellationToken);

                if (content is not null)
                {
                    cache.Set(key, content, options.CurrentValue.CacheDuration);
                }

                return content;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Wingineers API call to {Url} failed", url);
                return null;
            }
        }
    }
}
