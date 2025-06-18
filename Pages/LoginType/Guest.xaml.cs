using Supabase.Postgrest.Models;
using Time_Managmeent_System.Services;
using Supabase.Postgrest.Attributes;
using Time_Managmeent_System.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

namespace Time_Managmeent_System.Pages.LoginType;

public partial class Guest : ContentPage
{
    private readonly DataService _dataservice;
    public Guest(DataService dataService)
    {
        InitializeComponent();
        _dataservice = dataService;
    }

    // Define a UserProfile class to match the structure of your User Data table
    [Table("User Data")]
    public class UserProfile : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; } // Ensure this matches your database schema
        [Column("First")]
        public string First { get; set; }
        [Column("Last")]
        public string Last { get; set; }
        [Column("Position")]
        public string Position { get; set; }
        [Column("avatar_url")]
        public string AvatarUrl { get; set; }
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text; // Assuming you have an entry for email
        string password = PasswordEntry.Text; // Assuming you have an entry for password
        string firstName = FirstEntry.Text; // Assuming you have an entry for first name
        string lastName = LastEntry.Text; // Assuming you have an entry for last name
        string position = RoleEntry.SelectedItem?.ToString(); // Corrected to use SelectedItem for Picker

        // Input validation
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(position))
        {
            await DisplayAlert("Input Error", "Please fill in all fields.", "OK");
            return;
        }

        try
        {
            // Step 1: Create a new user in the authentication table
            var authResponse = await _dataservice.SupabaseClient.Auth.SignUp(email, password);

            
            if (authResponse.User.Id != null)
            {
               var UserID = authResponse.User.Id; // Use the user ID from the authentication response
               

                //The code is showing the ID fine but as soon as it tries to insert, it inserts null
                // Step 2: Insert user profile into User Data table

                var userProfile = new UserProfile
                {

                    Id = UserID, // Use the user ID from the authentication response
                    First = firstName,
                    Last = lastName,
                    Position = position,
                };
                await DisplayAlert("User ID", userProfile.Id, "OK");

                var insertResponse = await _dataservice.SupabaseClient
                    .From<UserProfile>() // Use underscores instead of space
                    .Insert([userProfile]); // Insert the new user profile
                    
                    
                // Fix: Check the HTTP response status instead of a non-existent 'Error' property
                if (insertResponse != null)
                {
                    await DisplayAlert("Success", "User registered successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "ErrorMESSAGE", "OK");
                }
            }
            else
            {
                await DisplayAlert("Signup Failed", "Error creating user. Please check your input or try again later.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Signup Error", ex.Message, "OK");
        }
    }
}