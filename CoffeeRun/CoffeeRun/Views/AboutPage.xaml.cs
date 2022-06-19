using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using MarcTron.Plugin;

namespace CoffeeRun.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void ReviewApp_Clicked(object sender, EventArgs e)
        {
            try
            {
                Launcher.OpenAsync(new Uri($"market://details?id={AppInfo.PackageName}"));
            }
            catch (Exception)
            {
                DisplayAlert("Error!", "Something went wrong. Please try again!", "Ok");
            }
        }
    }
}