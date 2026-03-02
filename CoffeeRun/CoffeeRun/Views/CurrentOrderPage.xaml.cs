using System.Collections.ObjectModel;
using CoffeeRun.Models;
using SQLite;

namespace CoffeeRun.Views;

public partial class CurrentOrderPage : ContentPage
{
    private readonly SQLiteAsyncConnection _connection;
    private ObservableCollection<CurrentOrder> _currentOrder = new();

    public CurrentOrderPage()
    {
        InitializeComponent();
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "coffeerun.db3");
        _connection = new SQLiteAsyncConnection(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _connection.CreateTableAsync<CurrentOrder>();
        _currentOrder = new ObservableCollection<CurrentOrder>(await _connection.Table<CurrentOrder>().ToListAsync());
        currentOrderList.ItemsSource = _currentOrder;
        CheckCount();
    }

    private void CheckBox_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        var checkbox = (CheckBox?)sender;
        if (checkbox == null) return;

        CurrentOrder? customer = checkbox.BindingContext as CurrentOrder;

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

    async void AddOrCreateOrder(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//CustomerListPage");
    }

    private void CurrentOrderList_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        currentOrderList.SelectedItem = null;
    }

    async void RemoveFromOrder_Clicked(object? sender, EventArgs e)
    {
        var removeCustomer = (sender as MenuItem)?.CommandParameter as CurrentOrder;
        if (removeCustomer != null && await DisplayAlert("Warning!", $"Are you sure you want to remove {removeCustomer.Name} from current order?", "Yes", "No"))
        {
            await _connection.DeleteAsync(removeCustomer);
            _currentOrder.Remove(removeCustomer);
            CheckCount();
        }
    }

    private void CheckCount()
    {
        paidChecked.Text = _currentOrder.Count(x => x.Paid).ToString();
        numberOfCoffees.Text = _currentOrder.Count.ToString();
    }

    async void EditCustomer_Clicked(object? sender, EventArgs e)
    {
        bool update = true;
        var currentOrderCustomer = (sender as MenuItem)?.CommandParameter as CurrentOrder;
        if (currentOrderCustomer != null)
        {
            await Shell.Current.Navigation.PushModalAsync(new AddPersonPage(currentOrderCustomer, update));
        }
    }
}