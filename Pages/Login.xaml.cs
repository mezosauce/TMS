using Time_Managmeent_System.ViewModels;

namespace Time_Managmeent_System.Pages
{
    public partial class LoginPage : ContentPage
    {

        public LoginPage()
        {
            InitializeComponent();
        }



    private async void OnEmployeeLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginType.Employee());
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
        // Pass the required 'employeesListingViewModel' parameter to the Guest constructor
        if (BindingContext is EmployeesListingViewModel employeesListingViewModel)
        {
            await Navigation.PushAsync(new LoginType.Guest(employeesListingViewModel));
        }
        else
        {
            // Handle the case where BindingContext is not of the expected type
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