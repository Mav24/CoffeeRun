using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoffeeRun.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PastOrderPage : ContentPage
    {
        public PastOrderPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var location = await Geolocation.GetLastKnownLocationAsync();
            var options = new MapLaunchOptions { Name = "Coffee Shops", NavigationMode = NavigationMode.Default };
            await Map.OpenAsync(location, options);

        }
    }
}