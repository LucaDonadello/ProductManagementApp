using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using SQLMaui.Data;
using SQLMaui.Models;

namespace SQLMaui.ViewModels
{
    /* Partial splits the functionalities of the class in multiple files
     * ObservableObject is It provides a series of SetProperty methods that can be used to easily set property values from types inheriting from ObservableObject
     * and to automatically raise the appropriate events.
     */
    public partial class ProductViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public ProductViewModel(DatabaseContext context) => _context = context;

        [ObservableProperty]
        private ObservableCollection<Product>? _products = new();    // Simply a list of Objects that provides notifications when items get added, removed, or when the whole list is refreshed

        [ObservableProperty]
        private Product? _operatingProduct = new();

        [ObservableProperty]
        private bool _isBusy;   // Operation is in progress

        [ObservableProperty]
        private bool _isRefreshing; // Operation is in progress

        [ObservableProperty]
        private string? _busyText;   // Text to display when the operation is in progress

        //[RelayCommand]
        //private void SetOperatingProduct(Product? product) => OperatingProduct = product ?? new();

        // This method is used to load the products asynchronously -- Fetch the item to display in the UI
        public async Task LoadProductsAsync()
        {
            await ExecuteAsync(async () =>
            {
                var products = await _context.GetAllAsync<Product>();
                // You can add other operations here such as fetching other data from the database
                // example:
                // var otherData = await _context.GetAllync<OtherData>();
                if (products is not null && products.Any())
                {
                    Products ??= new ObservableCollection<Product>();   // made a new collection of products

                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                }
            }, "Loading Products...");
        }

        [RelayCommand]
        public void SetOperatingProduct(Product? product) => OperatingProduct = product ?? new();

        [RelayCommand]
        private async Task SaveProductAsync()
        {
            if (OperatingProduct is null) { return; }   // If the product is null, return nothing

            var (isValid, ErrorMessage) = OperatingProduct.Validate();

            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation Error", ErrorMessage, "Ok");
                return;
            }


            var BusyText = OperatingProduct.Id == 0 ? "Creating Product..." : "Updating Product...";

            await ExecuteAsync(async () =>
            {

                if (OperatingProduct.Id == 0)
                {
                    // Creating the product
                    await _context.AddItemAsync<Product>(OperatingProduct);
                    // Add produt to the list
                    Products.Add(OperatingProduct);
                }
                else
                {
                    // Updating the product and check if product is updatable
                    if (await _context.UpdateItemAsync<Product>(OperatingProduct))
                    {
                        // update the product in the list
                        var productCopy = OperatingProduct.Clone();
                        var index = Products.IndexOf(OperatingProduct);
                        Products.RemoveAt(index);
                        Products.Insert(index, productCopy);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Product Updation Error", "Ok");
                        return;
                    }
                }
                // Need to reset the operating product
                OperatingProduct = new();
            }, BusyText);
            
        }
        [RelayCommand]
        private async Task DeleteProductAsync(int id)
        {
            // avoid repetition
            //IsBusy = true;
            //BusyText = "Deleting product";
            //try
            //{
            await ExecuteAsync(async () => {
                if (await _context.DeleteItemByKeyAsync<Product>(id))
                {
                    var product = Products.FirstOrDefault(p => p.Id == id); // returns the first element that meets the requirement
                    Products.Remove(product);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Unable to delete the product", "Ok");
                }
            }, "Deleting Product..");
            
            //}
            //finally
            //{
            //    IsBusy = false;
            //    BusyText = "Processing...";
            //}
        }

        private async Task ExecuteAsync(Func<Task>? operation, string? busyText = null)
        {
            IsBusy = true;
            busyText = busyText ?? "Processing...";
            try
            {
                if (operation != null)
                {
                    await operation.Invoke();    // Perform the operation  
                }
            }
            finally
            {
                IsBusy = false;
                busyText = "Processing...";
            }
        }
    }
}
