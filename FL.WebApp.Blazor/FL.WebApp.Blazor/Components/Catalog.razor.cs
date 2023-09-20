using EasyNetQ;
using FL.Common.Models;
using FL.WebApp.Blazor.Models.RPC;
using FL.WebApp.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FL.WebApp.Blazor.Components
{
    public partial class Catalog : ComponentBase
    {
        [Inject]
        private ICatalogService CatalogApi { get; set; }

        [Inject]
        private IBus Bus { get; set; }

        private bool _displayUnavailableProductError = false;
        private Product _unavailableProduct = new Product();
        private List<Product> _catalog = new List<Product>();
        private List<Product> _cart = new List<Product>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // do api call here to initialize catalog list
            _catalog = await CatalogApi.GetAllProductsAsync();
        }

        private async Task PurchaseItem(Product product)
        {
            var response = await Bus.Rpc.RequestAsync<Product, InventoryResponse>(product);

            await CatalogApi.CheckProductAvailabilityAsync(product);

            InventoryResponse inventory = response;

            if (inventory.Stock > 0)
            {
                // this part was supposed to be a call to another service, but let's just settle on displaying it to UI for now
                await InvokeAsync(() =>
                {
                    _cart.Add(product);
                    _displayUnavailableProductError = false;

                    StateHasChanged();
                });
            }
            else
            {
                _unavailableProduct = product;
                _displayUnavailableProductError = true;

                StateHasChanged();
            }
        }
    }
}
