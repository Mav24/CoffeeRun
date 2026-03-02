namespace CoffeeRun.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
    }

    private async void ReviewApp_Clicked(object? sender, EventArgs e)
    {
        try
        {
            await Launcher.OpenAsync(new Uri($"market://details?id={AppInfo.PackageName}"));
        }
        catch (Exception)
        {
            await DisplayAlert("Error!", "Something went wrong. Please try again!", "Ok");
        }
    }
}