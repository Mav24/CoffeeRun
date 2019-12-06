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

            // Gets Table from Current order
            _currentOrder = new ObservableCollection<CurrentOrder>((await _connection.Table<CurrentOrder>().ToListAsync()));
            
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
            if (selectedList.Count > 0)
            {
                CreateNewOrder.IsEnabled = true;
            }
            else
            {
                CreateNewOrder.IsEnabled = false;
            }

            #region Something I'm working on
            //var checkBox = (CheckBox)sender;
            //Customer customer = checkBox.BindingContext as Customer;

            //if (customer.AddToOrderChecked)
            //{
            //    selectedList.Add(customer);
            //}
            //else if (customer.AddToOrderChecked == false)
            //{
            //    selectedList.Remove(customer);
            //}
            //if (selectedList.Count > 0)
            //{
            //    CreateNewOrder.IsEnabled = true;
            //}
            //else
            //{
            //    CreateNewOrder.IsEnabled = false;
            //}
            #endregion
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

        async void EditCustomer(object sender, EventArgs e)
        {
            bool update = true;
            var customer = (sender as MenuItem).CommandParameter as Customer;
            await Shell.Current.Navigation.PushModalAsync(new AddPersonPage(customer, update));
            
        }

        
        async void CreateNewOrder_Clicked(object sender, EventArgs e)
        { 
            
            if (_currentOrder.Count > 0)
            {
                if (await DisplayAlert("Warning!", "Do you want to add to the current order or create new order?", "New", "Add"))
                {
                    await _connection.DeleteAllAsync<CurrentOrder>();
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
                        CreateNewOrder.IsEnabled = false;

                    }
                    selectedList.Clear();
                    await Shell.Current.GoToAsync("//CurrentOrderPage");
                }
                else
                {
                    await AddToOrder();
                    CreateNewOrder.IsEnabled = false;
                }
            }
            else
            {
                await AddToOrder();
                CreateNewOrder.IsEnabled = false;
            }
            
        }

        private async Task AddToOrder()
        {
            // Create a list for current order names
            List<string> currentOrderNames = new List<string>();

            // Create a list for duplicate names
            List<string> duplicateNames = new List<string>();

            foreach (var name in _currentOrder)
            {
                currentOrderNames.Add(name.Name);
            }
            foreach (var item in selectedList)
            {
                
                if (!currentOrderNames.Contains(item.Name))
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
                else
                {
                    duplicateNames.Add(item.Name);
                }
                

            }
            // Creating a message with the duplicate names
            var message = string.Empty;
            foreach (var item in duplicateNames)
            {
                message += item + ", ";
            }

            // Clears selected customers
            selectedList.Clear();

            // Check if there is duplicate names
            if (duplicateNames.Count > 0)
            {
                await DisplayAlert("Name Exist!", $"{message} Already exist in current order", "Ok");
            }
            await Shell.Current.GoToAsync("//CurrentOrderPage");
        }

        private void Customerslist_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            customerslist.SelectedItem = null;
        }
    }
}