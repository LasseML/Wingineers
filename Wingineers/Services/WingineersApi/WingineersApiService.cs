using Wingineers.Models;
using Wingineers.Services.WingineersApi.Models;

namespace Wingineers.Services.WingineersApi
{
    public class WingineersApiService
    {
        private readonly HttpClient _client;
        private readonly string Version = "1";

        public WingineersApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<SalesPersonDto>?> GetSalesPersonsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _client.GetAsync(
                    $"api/v{Version}/SalesPersons",
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadFromJsonAsync<List<SalesPersonDto>>(cancellationToken);

                return content;
            }
            catch (Exception ex)
            {
                //Logger
                return null;
            }
        }

        public async Task<List<OrderLineDto>?> GetOrderLinesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _client.GetAsync(
                    $"api/v{Version}/OrderLines",
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadFromJsonAsync<List<OrderLineDto>>(cancellationToken);

                return content;
            }
            catch (Exception ex)
            {
                //Logger
                return [];
            }
        }
    }
}
