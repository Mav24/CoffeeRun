﻿using CoffeeRun.Models;
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
    //[QueryProperty("Customer", "customer")]
    public partial class AddPersonPage : ContentPage
    {
        //public ObservableCollection<TypesOfCoffee> TypesOfCoffee { get; set; }

        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Customer> _customers;
        private ObservableCollection<CurrentOrder> _currentOrder;
        private readonly Customer customer;
        private readonly bool update = false;
        private readonly CurrentOrder currentOrder;

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
            this.customer = customer;

            // Fill fields with info
            if (customer.Custom)
            {
                name.Text = customer.Name;
                coffeetype.SelectedItem = "Custom";
                customCoffeeType.Text = customer.CoffeeType.ToString();
                coffeesize.SelectedItem = customer.CoffeeSize;
            }
            else
            {
                name.Text = customer.Name;
                coffeetype.SelectedItem = customer.CoffeeType;
                coffeesize.SelectedItem = customer.CoffeeSize;
            }
        }

        public AddPersonPage(CurrentOrder currentOrderCustomer, bool update)
        {
            InitializeComponent();
            GetConnection();
            customer = new Customer()
            {
                Id = currentOrderCustomer.Id,
                Name = currentOrderCustomer.Name,
                CoffeeSize = currentOrderCustomer.CoffeeSize,
                CoffeeType = currentOrderCustomer.CoffeeType
            };

            this.update = update;
            currentOrder = currentOrderCustomer;
            

            // Fill fields with info
            name.Text = currentOrderCustomer.Name;
            coffeetype.SelectedItem = currentOrderCustomer.CoffeeType;
            coffeesize.SelectedItem = currentOrderCustomer.CoffeeSize;
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
            _currentOrder = new ObservableCollection<CurrentOrder>(await _connection.Table<CurrentOrder>().ToListAsync());
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            if (update)
            {
                await UpdateCustomer();

            }
            else
            {
                await CreateNewCustomer();
            }
        }

        private async Task CreateNewCustomer()
        {
            try
            {

                //CoffeeTypes coffeeTypes = new CoffeeTypes() { WhatDoYouTakeInYourCoffee = customCoffeeType.Text };
                if (string.IsNullOrWhiteSpace(name.Text) || coffeesize.SelectedIndex == -1 || coffeetype.SelectedIndex == -1)
                {
                    await DisplayAlert("Error", "All fields need to be filled out", "OK");
                }
                else
                {

                    Customer newCustomer = new Customer()
                    {
                        Name = name.Text,
                        CoffeeSize = coffeesize.SelectedItem.ToString(),
                        CoffeeType = coffeetype.SelectedItem.ToString(),
                        Custom = false,
                        AddToOrderChecked = false
                    };
                    if(coffeetype.SelectedItem.ToString() == "Custom")
                    {
                        newCustomer.CoffeeType = customCoffeeType.Text;
                        newCustomer.Custom = true;
                    }
                    //await _connection.InsertAsync(coffeeTypes);
                    //_coffeeTypes.Add(coffeeTypes);

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

        private async Task UpdateCustomer()
        {
            try
            {
                Customer updateCustomer = new Customer();

                if (customer.Custom)
                {
                    updateCustomer = new Customer()
                    {
                        Id = customer.Id,
                        Name = name.Text,
                        CoffeeSize = coffeesize.SelectedItem.ToString(),
                        CoffeeType = customCoffeeType.Text,
                        Custom = customer.Custom,
                        AddToOrderChecked = false
                    };
                }
                else
                {
                    updateCustomer = new Customer() 
                    {
                        Id = customer.Id,
                        Name = name.Text,
                        CoffeeSize = coffeesize.SelectedItem.ToString(),
                        CoffeeType = coffeetype.SelectedItem.ToString(),
                        Custom = customer.Custom,
                        AddToOrderChecked = false
                    };
                }
                
                if (string.IsNullOrWhiteSpace(name.Text) || coffeesize.SelectedIndex == -1 || coffeetype.SelectedIndex == -1)
                {
                    await DisplayAlert("Error", "All fields need to be filled out", "OK");
                }

                else
                {
                    // This is so it will up the current order page also
                    if (_currentOrder.Count > 0)
                    {
                        CurrentOrder updateCurrentOrder = new CurrentOrder()
                        {
                            Id = customer.Id,
                            Name = updateCustomer.Name,
                            CoffeeSize = updateCustomer.CoffeeSize,
                            CoffeeType = updateCustomer.CoffeeType,
                            Custom = updateCustomer.Custom,
                            //Paid = currentOrder.Paid
                        };
                        foreach (var item in _currentOrder)
                        {
                            if (item.Name == updateCustomer.Name)
                                await _connection.UpdateAsync(updateCurrentOrder);


                        }
                    }
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

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PopModalAsync();
        }

        private void coffeetype_SelectedIndexChanged(object sender, EventArgs e)
        {
            string isCustom = coffeetype.SelectedItem.ToString();
            if(isCustom == "Custom")
            {
                customCoffeeType.IsVisible = true;
                if(customer != null)
                {
                    customer.Custom = true;
                }
            }
            else
            {
                customCoffeeType.IsVisible = false;
                if(customer != null)
                {
                    customer.Custom = false;
                }
            }
        }
    }
}