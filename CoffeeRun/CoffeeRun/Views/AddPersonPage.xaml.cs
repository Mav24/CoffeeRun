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
    [QueryProperty("Customer", "customer")]
    public partial class AddPersonPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Customer> _customers;
        private Customer customer;
        private bool update = false;

        public AddPersonPage()
        {
            InitializeComponent();
            GetConnection();
        }

        public AddPersonPage(Customer customer, bool update)
        {
            InitializeComponent();
            GetConnection();
            this.update = update;
            this.customer = new Customer();
            this.customer = customer;

            // Fill fields with info
            name.Text = customer.Name;
            coffeetype.SelectedItem = customer.CoffeeType;
            coffeesize.SelectedItem = customer.CoffeeSize;


        }

        private void GetConnection()
        {
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
            if (update)
            {
                try
                {
                    Customer updateCustomer = new Customer()
                    {
                        Id = customer.Id,
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
                        await _connection.UpdateAsync(updateCustomer);
                        await DisplayAlert("Customer Updated", $"Customer {updateCustomer.Name} updated", "OK");
                        await Shell.Current.Navigation.PopModalAsync();
                    }
                }
                catch (Exception)
                {
                    await DisplayAlert("Can't Update!", $"Sorry {name.Text} already exist in customer list", "OK");
                }
                
            }
            else
                try
                {
                    Customer newCustomer = new Customer()
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
                        await _connection.InsertAsync(newCustomer);
                        _customers.Add(newCustomer);
                        await DisplayAlert("Customer Added", $"Customer {newCustomer.Name} has been added", "OK");
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