using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeRun.Models;
using MarcTron.Plugin;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoffeeRun.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentOrderPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<CurrentOrder> _currentOrder;
        private int smallCoffee;
        private int mediumCoffee;
        private int largeCoffee;
        private int xLargeCoffee;

        public CurrentOrderPage()
        {
            InitializeComponent();
            _connection = MtSql.Current.GetConnectionAsync("coffeerun.db3");
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _connection.CreateTableAsync<CurrentOrder>();
            _currentOrder = new ObservableCollection<CurrentOrder>((await _connection.Table<CurrentOrder>().ToListAsync()));
            foreach (var item in _currentOrder)
            {
                switch (item.CoffeeSize)
                {
                    case "Small":
                        smallCoffee += 1;
                        break;
                    case "Medium":
                        mediumCoffee += 1;
                        break;
                    case "Large":
                        largeCoffee += 1;
                        break;
                    case "X-Large":
                        xLargeCoffee += 1;
                        break;
                    default:
                        break;
                }
            }
            //small.Text = $"S:{smallCoffee}";
            //medium.Text = $"M:{mediumCoffee}";
            //large.Text = $"L:{largeCoffee}";
            //xlarge.Text = $"Xl:{xLargeCoffee}";
            currentOrderList.ItemsSource = _currentOrder;
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

        }

        async void AddOrCreateOrder(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomerListPage");
        }
    }
}