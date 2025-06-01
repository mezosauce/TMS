namespace Time_Managmeent_System.Pages.Dashboard;

public partial class EmployeeDash : ContentPage
{
	public EmployeeDash()
	{
		InitializeComponent();
	}

	private async void OnClockClicked(object sender, EventArgs e)
	{

        //CLOCK IN AND OUT LOGIC
        if (ClockButton.Text == "Clock Out")
		{
			await DisplayAlert("Clock", "Successfully Clocked-Out", "OK");
			ClockButton.Text = "Clock In"; // Change button text to "Clock In"
			return; // Exit the method if already clocked out
        }
        await DisplayAlert("Clock", "Clock-in successful!", "OK");
		ClockButton.Text = "Clock Out"; // Change button text to "Clock Out"
    }
}