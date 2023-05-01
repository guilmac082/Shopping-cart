using Microsoft.AspNetCore.Http.HttpResults;
using ShoppingCart.ShoppingCart;
using System;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShoppingCart.ProductCatalogClient
{
    /// <summary>
    /// იდეაში ამას თავისი ბაზა უნდა ჰქონდეს სადაც მოვა პროდუქტ აიდი და თავის კატალოგის ბაზაში მონახავს ამ
    /// პროდუქტს დეტალურად; ანუ კატალოგია და თავისი დეტალური ბაზა აქვს ამ კატალოგის;
    /// </summary>
    public interface IProductCatalogClient
    {
        Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productIds);
    }

    public class ProductCatalogClient : IProductCatalogClient
    {
        private readonly HttpClient client;
        private static string productCatalogBaseUrl = @"https://git.io/JeHiE";
        private static string getProductPathTemplate = "?productIds=[{0}]";

        public ProductCatalogClient(HttpClient client)
        {
            client.BaseAddress = new Uri(productCatalogBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.client = client;
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds)
        {
            using var response = await RequestProductFromProductCatalog(productCatalogIds);
            return await ConvertToShoppingCartItems(response);
        }

        private async Task<HttpResponseMessage> RequestProductFromProductCatalog(int[] productCatalogIds)
        {
            var productsResource = string.Format(getProductPathTemplate, string.Join(",", productCatalogIds));
            return await client.GetAsync(productsResource);
        }

        private static async Task<IEnumerable<ShoppingCartItem>> ConvertToShoppingCartItems(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var products =
                await JsonSerializer.DeserializeAsync<List<ProductCatalogProduct>>(
                    await response.Content.ReadAsStreamAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            return products.Select(p => new ShoppingCartItem(p.ProductId, p.ProductName, p.ProductDescription, p.Price));
        }
        private record ProductCatalogProduct(int ProductId, string ProductName, string ProductDescription, Money Price);
    }

}
