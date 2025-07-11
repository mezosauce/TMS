using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Supabase.Postgrest;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Linq;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;


namespace Time_Managmeent_System.Pages.LoginType;

public partial class Guest : ContentPage
{
    private readonly DataService _dataservice;
    public Guest(DataService dataService)
    {
        InitializeComponent();
        _dataservice = dataService;
    }

    [Table("user_data")]
    public class UserProfile : BaseModel
    {
        [PrimaryKey("id", false)]
        [Column("id")]
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
                // Step 2: Insert user profile into user_data table

                var userProfile = new UserProfile
                {

                    Id = UserID, // Use the user ID from the authentication response
                    First = firstName,
                    Last = lastName,
                    Position = position,
                };
                await DisplayAlert("User ID", userProfile.Id, "OK");

                var insertResponse = await _dataservice.SupabaseClient
                .From<UserProfile>()
                .Insert(userProfile);


                // Fix: Check the HTTP response status instead of a non-existent 'Error' property
                if (insertResponse != null)
                {
                    // Corrected audit insertion code based on your actual Audit model
                await DisplayAlert("Success", "User registered successfully! \n Make sure to verify your email before Signing In", "OK");

                // Create the audit message
                var changeMessage = "User Made \n Id: " + UserID + "\n Name: " + firstName + " " + lastName + "\n Role: " + position;

                // Create a proper Audit object matching your model structure
                var auditEntry = new Audit
                {
                    Id = Guid.NewGuid().ToString(),  // Generate a unique audit ID
                    change = changeMessage  // Your change message
                };

                // Insert the audit entry
                await _dataservice.SupabaseClient
                    .From<Audit>()
                    .Insert(auditEntry);
                    await Navigation.PushAsync(new SignIn(_dataservice));

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