using CoffeeRun.Models;
using CoffeeRun.ViewModels;
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
    public partial class CustomerListPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Customer> _customers;
        private ObservableCollection<Customer> selectedList;
        private ObservableCollection<CurrentOrder> _currentOrder;

        public CustomerListPage()
        {
            InitializeComponent();
            _connection = MtSql.Current.GetConnectionAsync("coffeerun.db3");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _connection.CreateTableAsync<Customer>();
            _customers = new ObservableCollection<Customer>((await _connection.Table<Customer>().ToListAsync()));
            customerslist.ItemsSource = _customers;
        }

        private void AddCustomer_Clicked(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PushModalAsync(new AddPersonPage());
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            selectedList = new ObservableCollection<Customer>();

            for (int i = 0; i < _customers.Count; i++)
            {
                Customer item = _customers[i];
                if (item.AddToOrderChecked)
                {
                    selectedList.Add(item);
                }
            }
        }

        async void DeleteCustomer(object sender, EventArgs e)
        {
            var delCustomer = (sender as MenuItem).CommandParameter as Customer;
            if (await DisplayAlert("Warning", $"Are you sure you want to delete {delCustomer.Name} from Customer List?", "yes", "No"))
            {
                await _connection.DeleteAsync(delCustomer);
                _customers.Remove(delCustomer);
            }
        }

        private void EditCustomer(object sender, EventArgs e)
        {
            //TODO: Edit customer profile.
        }

        async void CreateNewOrder_Clicked(object sender, EventArgs e)
        {
            _currentOrder = new ObservableCollection<CurrentOrder>();
            foreach (var item in selectedList)
            {
                CurrentOrder order = new CurrentOrder()
                {
                    Name = item.Name,
                    CoffeeSize = item.CoffeeSize,
                    CoffeeType = item.CoffeeType,
                    Paid = false
                };
                await _connection.InsertAsync(order);
                _currentOrder.Add(order);

            }
            await Shell.Current.GoToAsync("//CurrentOrderPage");
        }
    }
}