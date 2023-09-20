using FL.Common.Models;
using Newtonsoft.Json;

namespace FL.WebApp.Blazor.Services
{
    public interface ICatalogService
    {
        Task<List<Product>> GetAllProductsAsync();

        Task CheckProductAvailabilityAsync(Product product);
    }

    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;

        public CatalogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Product>>($"catalog/all");

            if (response != null)
                return response;
            else
            {
                Console.WriteLine($"Response is null");

                return new List<Product>();
            }
        }

        public async Task CheckProductAvailabilityAsync(Product product)
        {
            await _httpClient.PostAsJsonAsync($"catalog/check", product);
        }
    }
}
