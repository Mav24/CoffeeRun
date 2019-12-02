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