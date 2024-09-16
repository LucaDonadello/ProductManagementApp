using SQLMaui.ViewModels;

namespace SQLMaui
{
    public partial class MainPage : ContentPage
    {
        private readonly ProductViewModel _viewModel;   // Add viewModel
        public MainPage(ProductViewModel viewModel)   // Add viewModel
        {
            InitializeComponent();
            BindingContext = viewModel;   // Set the BindingContext to the viewModel
            _viewModel = viewModel;
        }
        // Populate products when the screen comes into view
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadProductsAsync();
        }
    }

}
