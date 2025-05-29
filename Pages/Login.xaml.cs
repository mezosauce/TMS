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
    }
}