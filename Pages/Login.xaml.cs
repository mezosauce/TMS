using Time_Managmeent_System.Services;
using Time_Managmeent_System.ViewModels;


namespace Time_Managmeent_System.Pages;


public partial class LoginPage : ContentPage
{

   private readonly DataService _dataservice;

    public LoginPage(EmployeesListingViewModel employeesListingViewModel)
    {
        InitializeComponent();
        BindingContext= employeesListingViewModel;
    }


    private async void OnEmployeeLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginType.Employee(_dataservice));
    }

    private async void OnManagerLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginType.Manager());
    }

    private async void OnAdminLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginType.Admin());
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (BindingContext is EmployeesListingViewModel employeesListingViewModel)
        {
            await Navigation.PushAsync(new LoginType.Guest(employeesListingViewModel, _dataservice));
        }
        else
        {
            await DisplayAlert("Error", "Unable to navigate. Invalid context.", "OK");
        }
    }

    private void OnToggleDarkModeClicked(object sender, EventArgs e)
    {
        if (App.Current.UserAppTheme == AppTheme.Dark)
        {
            App.Current.UserAppTheme = AppTheme.Light;
            if (sender is Button btn)
            {
                btn.Text = "\uf185";
                btn.TextColor = Colors.Black;
            }
        }
        else
        {
            App.Current.UserAppTheme = AppTheme.Dark;
            if (sender is Button btn)
            {
                btn.Text = "\uf186";
                btn.TextColor = Colors.White;
            }
        }
    }
}