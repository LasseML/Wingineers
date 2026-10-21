using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Wingineers.Services.WingineersApi
{
    public static class WingineersServiceCollection
    {
        public static IServiceCollection AddWingineersClient(this IServiceCollection services)
        {
            services.AddOptions<WingineersOptions>()
                .BindConfiguration(WingineersOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHttpClient<WingineersApiService>((serviceProvider, client) =>
            {
                var settings = serviceProvider
                    .GetRequiredService<IOptions<WingineersOptions>>().Value;

                client.DefaultRequestHeaders.Add("ApiKey", settings.ApiKey);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                    mediaType: "application/json", quality: 1.0));

                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            return services;
        }
    }
}
