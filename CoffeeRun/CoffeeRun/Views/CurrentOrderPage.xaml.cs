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
        private int paidCount;
        private int unPaidCount;
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
            var currentOrderCount = _currentOrder.Count;
            CheckCount();
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkbox = (CheckBox)sender;

            CurrentOrder customer = checkbox.BindingContext as CurrentOrder;

            if (customer != null)
            {
                AddOrUpdateTheResult(customer, checkbox);
            }
        }

        async void AddOrUpdateTheResult(CurrentOrder customer, CheckBox checkbox)
        {
            if (checkbox.IsChecked)
            {
                CheckCount();
                customer.Paid = true;
                await _connection.UpdateAsync(customer);
            }
            else
            {
                CheckCount();
                customer.Paid = false;
                await _connection.UpdateAsync(customer);
            }
        }

        async void AddOrCreateOrder(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomerListPage");
        }

        private void CurrentOrderList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            currentOrderList.SelectedItem = null;
        }

        async void RemoveFromOrder_Clicked(object sender, EventArgs e)
        {
            var removeCustomer = (sender as MenuItem).CommandParameter as CurrentOrder;
            if (await DisplayAlert("Warning!", $"Are you sure you want to remove {removeCustomer.Name} from current order?", "Yes", "No"))
            {
                await _connection.DeleteAsync(removeCustomer);
                _currentOrder.Remove(removeCustomer);
                CheckCount();
            }
        }

        private void CheckCount()
        {
            paidChecked.Text = _currentOrder.Count(x => x.Paid).ToString();
            numberOfCoffees.Text = _currentOrder.Count().ToString();
        }

        async void EditCustomer_Clicked(object sender, EventArgs e)
        {
            bool update = true;
            var currentOrderCustomer = (sender as MenuItem).CommandParameter as CurrentOrder;
            await Shell.Current.Navigation.PushModalAsync(new AddPersonPage(currentOrderCustomer, update));
        }
    }
}