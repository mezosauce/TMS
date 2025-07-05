using Microsoft.Maui.Controls;
using Supabase.Interfaces;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Threading.Tasks;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;
namespace Time_Managmeent_System.Pages;

public partial class EditProfile : ContentPage
{
    private readonly DataService _dataService;

    private UserProfile _userProfile;

    public EditProfile(DataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        LoadProfileAsync();
        
    }

    private async void OnUploadAvatarClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select an image",
            FileTypes = FilePickerFileType.Images
        });

        if (result != null)
        {
            using var stream = await result.OpenReadAsync();
            string url = "https://your-storage-service.com/path/" + result.FileName;

            AvatarUrl.Text = url;
            AvatarPreview.Source = url;
        }
    }

    private async void LoadProfileAsync()
    {
        // Get the current authenticated user from Supabase Auth

        var user = _dataService.SupabaseClient.Auth.CurrentUser;
        if (user == null)
        {
            await DisplayAlert("Error", "No authenticated user found.", "OK");
            return;
        }

        // Assuming the UID is stored as a string in the Username column of Employee
        var response = await _dataService.SupabaseClient
            .From<UserProfile>()
            .Where(e => e.Id == user.Id)
            .Get();


        _userProfile = response.Models.FirstOrDefault(); // <-- Assign the result here

        if (_userProfile != null)
        {
            FirstNameEntry.Text = _userProfile.First;
            LastNameEntry.Text = _userProfile.Last;
            AvatarUrl.Text = _userProfile.AvatarUrl;
            // Add more fields as needed
        }
        else
        {
            await DisplayAlert("Error", "Employee not found.", "OK");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_userProfile != null)
        {
            _userProfile.First = FirstNameEntry.Text;
            _userProfile.Last = LastNameEntry.Text;
            _userProfile.AvatarUrl = AvatarUrl.Text;
            // Update other fields as needed

            await UpdateUserProfile(_userProfile);

            await DisplayAlert("Profile Updated", "Your profile has been updated.", "OK");
            await Navigation.PopAsync();
        }
    }

    public async Task UpdateUserProfile(UserProfile userProfile)
    {
        await _dataService.SupabaseClient
            .From<UserProfile>()
            .Where(u => u.Id == userProfile.Id)
            .Set(u => u.First, userProfile.First)
            .Set(u => u.Last, userProfile.Last)
            .Set(u => u.AvatarUrl, userProfile.AvatarUrl)
            // Add more fields as needed
            .Update();
    }
}
