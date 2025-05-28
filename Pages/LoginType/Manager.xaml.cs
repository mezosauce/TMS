namespace Time_Managmeent_System.Pages.LoginType;

public partial class Manager : ContentPage
{
	public Manager()
	{
		InitializeComponent();
	}

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Implement your login logic here
        // For example, validate credentials and navigate to the admin dashboard
        await DisplayAlert("Login", "Manager login successful!", "OK");
        await Navigation.PushAsync(new MainPage()); // Navigate to the main page after login
    }
}