namespace Time_Managmeent_System.Pages.LoginType;

public partial class Guest : ContentPage
{
	public Guest()
	{
		InitializeComponent();
	}

	private async void OnLoginClicked(object sender, EventArgs e)
	{
        // Implement your Register logic here
        // For example, validate credentials and navigate to the Employee, Admin, or Manager dashboard
        await DisplayAlert("Login", "Register  successful!", "OK");
        await Navigation.PushAsync(new LoginPage()); // Navigate to the main page after login
    }
    }