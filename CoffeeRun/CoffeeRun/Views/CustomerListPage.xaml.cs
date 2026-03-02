using CoffeeRun.Models;
using SQLite;
using System.Collections.ObjectModel;

namespace CoffeeRun.Views;

public partial class CustomerListPage : ContentPage
{
    private SQLiteAsyncConnection _connection;
    private ObservableCollection<Customer> _customers = new();
    private ObservableCollection<Customer> selectedList = new();
    private ObservableCollection<CurrentOrder> _currentOrder = new();

    public CustomerListPage()
    {
        InitializeComponent();
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "coffeerun.db3");
        _connection = new SQLiteAsyncConnection(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _connection.CreateTableAsync<Customer>();
        _customers = new ObservableCollection<Customer>(await _connection.Table<Customer>().ToListAsync());
        customerslist.ItemsSource = _customers;

        // Gets Table from Current order
        _currentOrder = new ObservableCollection<CurrentOrder>(await _connection.Table<CurrentOrder>().ToListAsync());
        selectedList = new ObservableCollection<Customer>();
    }

    private void AddCustomer_Clicked(object? sender, EventArgs e)
    {
        Shell.Current.Navigation.PushModalAsync(new AddPersonPage());
    }

    private void CheckBox_CheckedChanged(object? sender, CheckedChangedEventArgs e)
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
    }

    async void DeleteCustomer(object? sender, EventArgs e)
    {
        var delCustomer = (sender as MenuItem)?.CommandParameter as Customer;
        if (delCustomer != null && await DisplayAlert("Warning", $"Are you sure you want to delete {delCustomer.Name} from Customer List?", "yes", "No"))
        {
            await _connection.DeleteAsync(delCustomer);
            _customers.Remove(delCustomer);
        }
    }

    async void EditCustomer(object? sender, EventArgs e)
    {
        bool update = true;
        var customer = (sender as MenuItem)?.CommandParameter as Customer;
        if (customer != null)
        {
            await Shell.Current.Navigation.PushModalAsync(new AddPersonPage(customer, update));
        }
    }

    
    async void CreateNewOrder_Clicked(object? sender, EventArgs e)
    {
        
        if (selectedList.Count == 0)
        {
            await DisplayAlert("Nothing Selected", "You need to select a customer to create or add to an order!", "Ok");
        }
        else
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
                            Id = item.Id,
                            Name = item.Name,
                            CoffeeSize = item.CoffeeSize,
                            CoffeeType = item.CoffeeType,
                            Custom = item.Custom,
                            Paid = false
                        };
                        await _connection.InsertAsync(order);
                        _currentOrder.Add(order);
                    }
                    selectedList.Clear();
                    await Shell.Current.GoToAsync("//CurrentOrderPage");
                }
                else
                {
                    await AddToOrder();
                }
            }
            else
            {
                await AddToOrder();
            }
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
            currentOrderNames.Add(name.Name ?? string.Empty);
        }
        foreach (var item in selectedList)
        {
            
            if (!currentOrderNames.Contains(item.Name ?? string.Empty))
            {
                CurrentOrder order = new CurrentOrder()
                {
                    Id = item.Id,
                    Name = item.Name,
                    CoffeeSize = item.CoffeeSize,
                    CoffeeType = item.CoffeeType,
                    Custom = item.Custom,
                    Paid = false
                };
                await _connection.InsertAsync(order);
                _currentOrder.Add(order);
            }
            else
            {
                duplicateNames.Add(item.Name ?? string.Empty);
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

    private void Customerslist_ItemTapped(object? sender, ItemTappedEventArgs e)
    {
        customerslist.SelectedItem = null;
    }
}