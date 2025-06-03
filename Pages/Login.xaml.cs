using Microsoft.Maui.Controls;

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
            // Navigate to Employee login or main page
            await Navigation.PushAsync(new LoginType.Employee());
        }

        private async void OnManagerLoginClicked(object sender, EventArgs e)
        {
            // Navigate to Manager login or main page
            await Navigation.PushAsync(new LoginType.Manager());
        }

        private async void OnAdminLoginClicked(object sender, EventArgs e)
        {
            // Navigate to Admin login or main page
            await Navigation.PushAsync(new LoginType.Admin());
        }
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Navigate to Guest login or main page
            await Navigation.PushAsync(new LoginType.Guest());
        }
        private void OnToggleDarkModeClicked(object sender, EventArgs e)
        {
            if (App.Current.UserAppTheme == AppTheme.Dark)
            {
                App.Current.UserAppTheme = AppTheme.Light;
                if (sender is Button btn)
                {
                    btn.Text = "\uf185"; // Example: Sun icon (FontAwesome unicode)
                    btn.TextColor = Colors.Black;
                }
            }
            else
            {
                App.Current.UserAppTheme = AppTheme.Dark;
                if (sender is Button btn)
                {
                    btn.Text = "\uf186"; // Example: Moon icon (FontAwesome unicode)
                    btn.TextColor = Colors.White;
                }
            }
        }
    }
}