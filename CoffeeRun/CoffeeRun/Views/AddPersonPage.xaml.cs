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
    public partial class AddPersonPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Customer> _customers;
        
        public AddPersonPage()
        {
            InitializeComponent();
            _connection = MtSql.Current.GetConnectionAsync("coffeerun.db3");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _connection.CreateTableAsync<Customer>();
            _customers = new ObservableCollection<Customer>((await _connection.Table<Customer>().ToListAsync()));
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            try
            {
                Customer customer = new Customer()
                {
                    Name = name.Text,
                    CoffeeSize = coffeesize.SelectedItem.ToString(),
                    CoffeeType = coffeetype.SelectedItem.ToString(),
                    AddToOrderChecked = false
                };
                if (string.IsNullOrWhiteSpace(name.Text) || coffeesize.SelectedIndex == -1 || coffeetype.SelectedIndex == -1)
                {
                    await DisplayAlert("Error", "All fields need to be filled out", "OK");
                }
                else
                {
                    await _connection.InsertAsync(customer);
                    _customers.Add(customer);
                    await DisplayAlert("Customer Added", "new customer has been added", "OK");
                    await Shell.Current.Navigation.PopModalAsync();
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Error", $"Customer {name.Text} already exist in customer list", "OK");
                
            }
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PopModalAsync();
        }
    }
}