using CoffeeRun.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CoffeeRun.ViewModels
{
    public class CustomerListVM
    {
        private SQLiteAsyncConnection _connection;
        public ObservableCollection<Customer> Customers { get; private set; } = new ObservableCollection<Customer>();
    }
}
