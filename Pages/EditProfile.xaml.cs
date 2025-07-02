using Microsoft.Maui.Controls;

namespace Time_Managmeent_System.Pages;

public partial class EditProfile : ContentPage
{
    // Simulated user profile (replace with your actual data source/service)
    private UserProfile _userProfile = new UserProfile
    {
        First = "John",
        Last = "Doe",
        Position = "Admin"
    };

    public EditProfile()
    {
        InitializeComponent();
        LoadProfile();
    }

    private void LoadProfile()
    {
        // Populate fields with current user data
        FirstNameEntry.Text = _userProfile.First;
        LastNameEntry.Text = _userProfile.Last;
        PositionEntry.Text = _userProfile.Position;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Save changes (replace with your actual save logic)
        _userProfile.First = FirstNameEntry.Text;
        _userProfile.Last = LastNameEntry.Text;
        _userProfile.Position = PositionEntry.Text;

        // Simulate save and notify user
        await DisplayAlert("Profile Updated", "Your profile has been updated.", "OK");
        await Navigation.PopAsync();
    }

    // Simple user profile class for demonstration
    private class UserProfile
    {
        public string First { get; set; }
        public string Last { get; set; }
        public string Position { get; set; }
    }
}