using Supabase;
using Microsoft.Maui.ApplicationModel.Communication;
using Supabase.Gotrue;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;


namespace Time_Managmeent_System.Pages;

public partial class EditAccount : ContentPage
{
    private List<UserProfile> currentUsers = new();
    private readonly DataService _dataService;
    public EditAccount(DataService dataService)

    {
        InitializeComponent();
        _dataService = dataService;
    }


    private async void OnRoleSelected(object sender, EventArgs e)
    {
        // Get selected role from Picker
        var selectedRole = RoleEntry.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedRole))
            return;

        try
        {
            // Query Supabase for all users with this position
            var results = await _dataService.SupabaseClient
                .From<UserProfile>()
                .Where(x => x.Position == selectedRole)
                .Get();

            // Get first names list
            var firstNames = results.Models
                .Select(u => u.First)
                .Distinct()
                .ToList();

            // Bind to second Picker
            FirstNamePicker.ItemsSource = firstNames;
            FirstNamePicker.IsVisible = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load names: {ex.Message}", "OK");
        }
    }

    private async void RoleEntry_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedRole = RoleEntry.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedRole))
            return;

        try
        {
            var results = await _dataService.SupabaseClient
                .From<UserProfile>()
                .Where(x => x.Position == selectedRole)
                .Get();

            // Save the list for later
            currentUsers = results.Models.Cast<UserProfile>().ToList();

            // Populate first names in Picker
            var firstNames = currentUsers.Select(u => u.First).Distinct().ToList();
            FirstNamePicker.ItemsSource = firstNames;
            FirstNamePicker.IsVisible = true;

            // Reset other UI
            
            DeleteButton.IsVisible = false;

            // Add event handler for FirstNamePicker
            FirstNamePicker.SelectedIndexChanged += FirstNamePicker_SelectedIndexChanged;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load names: {ex.Message}", "OK");
        }
    }
    private void FirstNamePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedFirstName = FirstNamePicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedFirstName))
            return;

        // Pick first match for simplicity
        var user = currentUsers.FirstOrDefault(u => u.First == selectedFirstName);
        if (user == null)
            return;

        
        DeleteButton.IsVisible = true;


        // Show and pre-fill edit fields
        EditProfileLabel.IsVisible = true;
        FirstNameEntry.Text = user.First;
        LastNameEntry.Text = user.Last;

        FirstNameEntry.IsVisible = true;
        LastNameEntry.IsVisible = true;

        SaveButton.IsVisible = true;


    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var selectedFirstName = FirstNamePicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedFirstName))
            return;

        var user = currentUsers.FirstOrDefault(u => u.First == selectedFirstName);
        if (user == null)
            return;

        var confirm = await DisplayAlert("Confirm Delete", $"Delete {user.First} {user.Last}?", "Yes", "No");
        if (!confirm)
            return;

        try
        {
            // Execute the delete operation without assigning to a variable
            await _dataService.SupabaseClient
                .From<UserProfile>()
                .Where(x => x.Id == user.Id)
                .Delete();


            // Delete the user using the Admin API

            var edgeResult = await _dataService.CallDeleteUserEdgeFunctionAsync(user.Id);
            if (edgeResult)
            {
                await DisplayAlert("Success", "Employee deleted successfully via edge function.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to delete employee via edge function.", "OK");
            }

            var thisuser = _dataService.SupabaseClient.Auth.CurrentUser;
            var changeMessage = "User: " + user.First + " " + user.Last + " was deleted by ID:: " + thisuser.Id ;

            // Create a proper Audit object matching your model structure
            var auditEntry = new Audit
            {
                Id = Guid.NewGuid().ToString(), 
                change = changeMessage  
            };

            // Insert the audit entry
            await _dataService.SupabaseClient
                .From<Audit>()
                .Insert(auditEntry);



            // Refresh the list by triggering the role selection again
            if (RoleEntry.SelectedItem != null)
            {
                RoleEntry_SelectedIndexChanged(null, null);
            }

            // Reset UI elements
            
            DeleteButton.IsVisible = false;
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during the delete operation
            await DisplayAlert("Error", $"An error occurred while deleting the employee: {ex.Message}", "OK");

            // Log the exception for debugging
            Console.WriteLine($"Delete operation failed: {ex}");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var selectedFirstName = FirstNamePicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedFirstName))
            return;

        var user = currentUsers.FirstOrDefault(u => u.First == selectedFirstName);
        if (user == null)
            return;

        // Update fields
        user.First = FirstNameEntry.Text;
        user.Last = LastNameEntry.Text;

        try
        {
            UpdateUserProfile(user);

            await DisplayAlert("Success", "Profile updated!", "OK");

            var thisuser = _dataService.SupabaseClient.Auth.CurrentUser;
            var changeMessage = "User: " + user.First + " " + user.Last + " was deleted by ID:: " + thisuser.Id;

            // Create a proper Audit object matching your model structure
            var auditEntry = new Audit
            {
                Id = Guid.NewGuid().ToString(),
                change = changeMessage
            };

            // Insert the audit entry
            await _dataService.SupabaseClient
                .From<Audit>()
                .Insert(auditEntry);

            // route back
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to update: {ex.Message}", "OK");
        }
    }

    public async Task UpdateUserProfile(UserProfile userProfile)
    {
        await _dataService.SupabaseClient
            .From<UserProfile>()
            .Where(u => u.Id == userProfile.Id)
            .Set(u => u.First, userProfile.First)
            .Set(u => u.Last, userProfile.Last)
            // Add more fields as needed
            .Update();
    }
}

