using Time_Managmeent_System.Services;
using Time_Managmeent_System.ViewModels;

namespace Time_Managmeent_System.Pages.LoginType;

public partial class Guest : ContentPage
{
    private readonly EmployeesListingViewModel _employeesListingViewModel;

    public Guest(EmployeesListingViewModel employeesListingViewModel)
    {
        InitializeComponent();
        _employeesListingViewModel = employeesListingViewModel;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Implement your Register logic here
        // For example, validate credentials and navigate to the Employee, Admin, or Manager dashboard
        await DisplayAlert("Login", "Register successful!", "OK");
        await Navigation.PushAsync(new LoginPage(_employeesListingViewModel)); // Pass the required parameter
    }
}