using CoffeeRun.Models;
using SQLite;
using System.Collections.ObjectModel;

namespace CoffeeRun.Views;

public partial class OrderDetailsPage : ContentPage
{
    private readonly SQLiteAsyncConnection _connection;
    private ObservableCollection<CurrentOrder> _currentOrder = new();
    private ObservableCollection<OrderDetails> _orderDetails = new();

    public OrderDetailsPage()
    {
        InitializeComponent();
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "coffeerun.db3");
        _connection = new SQLiteAsyncConnection(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _currentOrder = new ObservableCollection<CurrentOrder>(await _connection.Table<CurrentOrder>().ToListAsync());
        DisplayCurrentOrder();
    }

    private void DisplayCurrentOrder()
    {
        _orderDetails = new ObservableCollection<OrderDetails>();
        OrderDetails orderDetails = new OrderDetails();
        foreach (var item in _currentOrder)
        {
            switch (item.CoffeeSize)
            {
                case "Small":
                    if (item.CoffeeType == "Black")
                        orderDetails.SmallBlack += 1;
                    if (item.CoffeeType == "Regular")
                        orderDetails.SmallRegular += 1;
                    if (item.CoffeeType == "Dbl Dbl")
                        orderDetails.SmallDD += 1;
                    if (item.CoffeeType == "Trpl Trpl")
                        orderDetails.SmallTrplTrpl += 1;
                    if (item.Custom)
                        orderDetails.Custom += $"{item.CoffeeSize} : {item.CoffeeType}" + Environment.NewLine;
                    break;
                case "Medium":
                    if (item.CoffeeType == "Black")
                        orderDetails.MediumBlack += 1;
                    if (item.CoffeeType == "Regular")
                        orderDetails.MediumRegular += 1;
                    if (item.CoffeeType == "Dbl Dbl")
                        orderDetails.MediumDD += 1;
                    if (item.CoffeeType == "Trpl Trpl")
                        orderDetails.MediumTrplTrpl += 1;
                    if (item.Custom)
                        orderDetails.Custom += $"{item.CoffeeSize} : {item.CoffeeType}" + Environment.NewLine;
                    break;
                case "Large":
                    if (item.CoffeeType == "Black")
                        orderDetails.LargeBlack += 1;
                    if (item.CoffeeType == "Regular")
                        orderDetails.LargeRegular += 1;
                    if (item.CoffeeType == "Dbl Dbl")
                        orderDetails.LargeDD += 1;
                    if (item.CoffeeType == "Trpl Trpl")
                        orderDetails.LargeTrplTrpl += 1;
                    if (item.Custom)
                        orderDetails.Custom += $"{item.CoffeeSize} : {item.CoffeeType}" + Environment.NewLine;
                    break;
                case "X-Large":
                    if (item.CoffeeType == "Black")
                        orderDetails.XLargeBlack += 1;
                    if (item.CoffeeType == "Regular")
                        orderDetails.XLargeRegular += 1;
                    if (item.CoffeeType == "Dbl Dbl")
                        orderDetails.XLargeDD += 1;
                    if (item.CoffeeType == "Trpl Trpl")
                        orderDetails.XLargeTrplTrpl += 1;
                    if (item.Custom)
                        orderDetails.Custom += $"{item.CoffeeSize} : {item.CoffeeType}" + Environment.NewLine;
                    break;
                default:
                    break;
            }
        }
        _orderDetails.Add(orderDetails);
        OrderDetailsCollectionView.ItemsSource = _orderDetails;
    }

    async void CompleteOrder_Click(object? sender, EventArgs e)
    {
        if (_currentOrder.Count > 0)
        {
            if (await DisplayAlert("Finish order!", "Are you sure you want to finish current order?", "Yes", "No"))
            {
                await _connection.DeleteAllAsync<CurrentOrder>();
                await Shell.Current.GoToAsync("//CurrentOrderPage");
            }
        }
        else
        {
            await DisplayAlert("No Order!", "There is no current order to finish", "Ok");
        }
    }
}