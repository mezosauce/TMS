namespace Time_Managmeent_System.Pages.LoginType;

public partial class Employee : ContentPage
{
	public Employee()
	{
		InitializeComponent();
	}

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Implement your login logic here
        // For example, validate credentials and navigate to the admin dashboard
        await DisplayAlert("Login", "Employee login successful!", "OK");
        await Navigation.PushAsync(new Dashboard.EmployeeDash()); // Navigate to the main page after login
    }
}