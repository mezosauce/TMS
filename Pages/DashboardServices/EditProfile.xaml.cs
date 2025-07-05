using Microsoft.Maui.Controls;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Threading.Tasks;

namespace Time_Managmeent_System.Pages;

public partial class EditProfile : ContentPage
{
    private UserProfile _userProfile;
    public EditProfile()
    {
        InitializeComponent();
        LoadProfileAsync();
    }
    private async void LoadProfileAsync()
    {
        // Example: Fetch user profile from Supabase (replace with your actual fetch logic)
        var userId = "current_user_id"; // Replace with actual user ID
        //_userProfile = await GetUserProfileAsync(userId);

        // Map all attributes to variables or UI fields
        if (_userProfile != null)
        {
            FirstNameEntry.Text = _userProfile.First;
            LastNameEntry.Text = _userProfile.Last;
            PositionEntry.Text = _userProfile.Position;
            // Add more fields as needed, e.g.:
            // AvatarUrlEntry.Text = _userProfile.AvatarUrl;
        }
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