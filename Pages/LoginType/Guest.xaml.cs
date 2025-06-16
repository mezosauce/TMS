using Time_Managmeent_System.Services;
using Supabase;
using Time_Managmeent_System.ViewModels;

namespace Time_Managmeent_System.Pages.LoginType;

public partial class Guest : ContentPage
{
    private readonly DataService _dataservice;
    public Guest(DataService dataService)
    {
        InitializeComponent();
        _dataservice = dataService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Implement your Register logic here
        // For example, validate credentials and navigate to the Employee, Admin, or Manager dashboard
        await DisplayAlert("Login", "Register successful!", "OK");
        await Navigation.PushAsync(new LoginPage(_dataservice)); // Pass the required parameter
    }
}