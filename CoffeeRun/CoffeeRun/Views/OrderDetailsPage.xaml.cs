using CoffeeRun.Models;
using MarcTron.Plugin;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoffeeRun.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderDetailsPage : ContentPage
    {
        private readonly SQLiteAsyncConnection _connection;
        private ObservableCollection<CurrentOrder> _currentOrder;
        private ObservableCollection<OrderDetails> _orderDetails;



        public OrderDetailsPage()
        {
            InitializeComponent();
            _connection = MtSql.Current.GetConnectionAsync("coffeerun.db3");
            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _currentOrder = new ObservableCollection<CurrentOrder>((await _connection.Table<CurrentOrder>().ToListAsync()));
            CrossMTAdmob.Current.LoadInterstitial(GetAdIds.GetInterstitialIds());
            DisplayCurrentOrder();
            ShowAdMob();
        }

        private void ShowAdMob()
        {
            CrossMTAdmob.Current.ShowInterstitial();
        }

        private void DisplayCurrentOrder()
        {
            _orderDetails = new ObservableCollection<OrderDetails>();
            OrderDetails orderDetails = new OrderDetails();
            foreach (var item in _currentOrder)
            {
                switch (item.CoffeeSize)
                {
                    case "Small":
                        if (item.CoffeeType == "Black")
                            orderDetails.SmallBlack += 1;
                        if (item.CoffeeType == "2 Cream")
                            orderDetails.Small2Cream += 1;
                        if (item.CoffeeType == "Double Double")
                            orderDetails.SmallDD += 1;
                        if (item.CoffeeType == "Regular")
                            orderDetails.SmallRegular += 1;
                        break;
                    case "Medium":
                        if (item.CoffeeType == "Black")
                            orderDetails.MediumBlack += 1;
                        if (item.CoffeeType == "2 Cream")
                            orderDetails.Medium2Cream += 1;
                        if (item.CoffeeType == "Double Double")
                            orderDetails.MediumDD += 1;
                        if (item.CoffeeType == "Regular")
                            orderDetails.MediumRegular += 1;
                        break;
                    case "Large":
                        if (item.CoffeeType == "Black")
                            orderDetails.LargeBlack += 1;
                        if (item.CoffeeType == "2 Cream")
                            orderDetails.Large2Cream += 1;
                        if (item.CoffeeType == "Double Double")
                            orderDetails.LargeDD += 1;
                        if (item.CoffeeType == "Regular")
                            orderDetails.LargeRegular += 1;
                        break;
                    case "X-Large":
                        if (item.CoffeeType == "Black")
                            orderDetails.XLargeBlack += 1;
                        if (item.CoffeeType == "2 Cream")
                            orderDetails.XLarge2Cream += 1;
                        if (item.CoffeeType == "Double Double")
                            orderDetails.XLargeDD += 1;
                        if (item.CoffeeType == "Regular")
                            orderDetails.XLargeRegular += 1;
                        break;
                    default:
                        break;
                }
            }
            _orderDetails.Add(orderDetails);
            OrderDetailsCollectionView.ItemsSource = _orderDetails;
        }

        async void CompleteOrder_Click(object sender, EventArgs e)
        {
            //await DisplayAlert("Coming Soon!", "Not yet working", "Ok");
            if (_currentOrder.Count > 0)
            {
                if (await DisplayAlert("Finish order!", "Are you sure you want to finish current order?", "Yes", "No"))
                {
                    await _connection.DeleteAllAsync<CurrentOrder>();
                    await Shell.Current.GoToAsync("//CurrentOrderPage");
                }
            }
            else
            {
                await DisplayAlert("No Order!", "There is no current order to finish", "Ok");
            }
            
        }
    }
}