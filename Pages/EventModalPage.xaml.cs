using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;

namespace Time_Management_System.Pages
{
    public partial class EventModalPage : ContentPage
    {
        private DateTime _selectedDate;
        private Label _dateLabel; // Renamed to avoid ambiguity

        private readonly DataService _dataService;

        public EventModalPage(DateTime selectedDate, DataService dataService)
        {
            InitializeComponent();
            _selectedDate = selectedDate;
            _dataService = dataService;

            DateLabel.Text = selectedDate.ToString("D");

            LoadEvents();
        }
        public class ShiftDisplay
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string UserId { get; set; }
            public Time ShiftModel { get; set; }

            public bool IsOwnedByCurrentUser { get; set; }  // New property
        }

        private async void LoadEvents()
        {
            try
            {
                var user = _dataService.SupabaseClient.Auth.CurrentUser;

                // Pull all shifts on the selected date
                var response = await _dataService.SupabaseClient
                    .From<Time>()
                    .Where(t => t.Shift_date == _selectedDate.Date)
                    .Get();

                var shifts = response.Models;

                // Optional: load user profiles (names)
                var userIds = shifts.Select(s => s.User_ID).Distinct().ToList();
                var usersResponse = await _dataService.SupabaseClient
                    .From<UserProfile>()
                    .Get();

                var userProfiles = usersResponse.Models
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionary(u => u.Id, u => $"{u.First} {u.Last}");

                var shiftDisplays = shifts.Select(s =>
                {
                    var start = GetShiftStartTime(s.Shift_type, s.Shift_date);
                    var end = GetShiftEndTime(s.Shift_type, s.Shift_date);
                    userProfiles.TryGetValue(s.User_ID, out var name);

                    return new ShiftDisplay
                    {
                        Title = $"{name ?? "Unknown"} - {s.Shift_type}",
                        Description = $"{start:hh:mm tt} to {end:hh:mm tt}",
                        UserId = s.User_ID,
                        ShiftModel = s,
                        IsOwnedByCurrentUser = s.User_ID == user.Id  // << 👈 here
                    };
                }).ToList();


                EventList.ItemsSource = shiftDisplays;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load shifts: {ex.Message}", "OK");
            }
        }

        private async void OnCancelShiftClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Time shift)
            {
                try
                {
                    var user = _dataService.SupabaseClient.Auth.CurrentUser;

                    if (user == null)
                    {
                        await DisplayAlert("Error", "User not logged in.", "OK");
                        return;
                    }

                    // Get the user profile of the logged-in user
                    var userProfileResponse = await _dataService.SupabaseClient
                        .From<UserProfile>()
                        .Where(x => x.Id == user.Id)
                        .Get();

                    var userProfile = userProfileResponse.Models.FirstOrDefault();
                    if (userProfile == null)
                    {
                        await DisplayAlert("Error", "Could not find current user profile.", "OK");
                        return;
                    }

                    var adminResponse = await _dataService.SupabaseClient
                         .From<UserProfile>()
                         .Where(x => x.Position == "Admin")
                         .Get();

                    var admin = adminResponse.Models
                        .FirstOrDefault(up => up.Position != null && up.Position.Contains("Admin", StringComparison.OrdinalIgnoreCase));

                    if (admin == null)
                    {
                        await DisplayAlert("Error", "No admin found to notify.", "OK");
                        return;
                    }

                    // Delete the shift
                    await _dataService.SupabaseClient
                        .From<Time>()
                        .Where(t => t.User_ID == shift.User_ID && t.Shift_date == shift.Shift_date)
                        .Delete();

                    await DisplayAlert("Success", "Shift canceled successfully.", "OK");

                    // Create notification
                    var notification = new Notifications
                    {
                        Notification_ID = Guid.NewGuid().ToString(),
                        Sender_ID = user.Id,
                        Receiver_ID = admin.Id,
                        message = $"CANCELED EVENT: {userProfile.First} {userProfile.Last} canceled their shift on {shift.Shift_date:MMMM dd, yyyy} ({shift.Shift_type}).",
                        Send_Time = DateTime.UtcNow,
                        Receive_Time = DateTime.UtcNow
                    };

                    // Insert the notification
                    await _dataService.SupabaseClient
                        .From<Notifications>()
                        .Insert(notification);

                    LoadEvents(); // Refresh UI
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to cancel shift: {ex.Message}", "OK");
                }
            }
        }


        private async void OnTradeShiftClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Time currentShift)
            {
                try
                {
                    var currentUser = _dataService.SupabaseClient.Auth.CurrentUser;

                    if (currentUser == null)
                    {
                        await DisplayAlert("Error", "User not logged in.", "OK");
                        return;
                    }

                    // Get current user profile
                    var currentProfileResponse = await _dataService.SupabaseClient
                        .From<UserProfile>()
                        .Where(x => x.Id == currentUser.Id)
                        .Get();

                    var currentProfile = currentProfileResponse.Models.FirstOrDefault();
                    if (currentProfile == null)
                    {
                        await DisplayAlert("Error", "Your profile could not be found.", "OK");
                        return;
                    }

                    // Get all future shifts from other users
                    var futureShiftResponse = await _dataService.SupabaseClient
                        .From<Time>()
                        .Where(t => t.Shift_date > DateTime.UtcNow && t.User_ID != currentUser.Id)
                        .Get();

                    var allFutureShifts = futureShiftResponse.Models;

                    var userIds = allFutureShifts.Select(t => t.User_ID).Distinct().ToList();

                    var userProfilesResponse = await _dataService.SupabaseClient
                        .From<UserProfile>()
                        .Get();

                    var allProfiles = userProfilesResponse.Models
                        .Where(p => userIds.Contains(p.Id))
                        .ToList();

                    if (!allProfiles.Any())
                    {
                        await DisplayAlert("No Users", "No users available for trade.", "OK");
                        return;
                    }

                    // First dropdown: pick a user
                    var userNames = allProfiles.Select(p => $"{p.First} {p.Last}").ToArray();
                    var selectedUserName = await DisplayActionSheet("Select user to trade with:", "Cancel", null, userNames);

                    if (selectedUserName == "Cancel" || string.IsNullOrEmpty(selectedUserName))
                        return;

                    var selectedProfile = allProfiles.FirstOrDefault(p => $"{p.First} {p.Last}" == selectedUserName);
                    if (selectedProfile == null)
                    {
                        await DisplayAlert("Error", "Selected user not found.", "OK");
                        return;
                    }

                    // Get their future shifts
                    var selectedUserShifts = allFutureShifts
                        .Where(t => t.User_ID == selectedProfile.Id)
                        .OrderBy(t => t.Shift_date)
                        .ToList();

                    if (!selectedUserShifts.Any())
                    {
                        await DisplayAlert("No Shifts", $"{selectedUserName} has no future shifts available.", "OK");
                        return;
                    }

                    // Second dropdown: pick one of their shifts
                    var shiftOptions = selectedUserShifts
                        .Select(t => $"{t.Shift_type} on {t.Shift_date:MMM dd, yyyy}")
                        .ToArray();

                    var selectedShiftText = await DisplayActionSheet("Select a shift to trade with:", "Cancel", null, shiftOptions);

                    if (selectedShiftText == "Cancel" || string.IsNullOrEmpty(selectedShiftText))
                        return;

                    var selectedTradeShift = selectedUserShifts
                        .FirstOrDefault(t => $"{t.Shift_type} on {t.Shift_date:MMM dd, yyyy}" == selectedShiftText);

                    if (selectedTradeShift == null)
                    {
                        await DisplayAlert("Error", "Selected shift not found.", "OK");
                        return;
                    }

                    // Confirm trade request
                    var confirm = await DisplayAlert("Confirm Trade Request",
                        $"Trade your {currentShift.Shift_type} shift on {currentShift.Shift_date:MMM dd} with {selectedUserName}'s {selectedTradeShift.Shift_type} on {selectedTradeShift.Shift_date:MMM dd}?",
                        "Yes", "No");

                    if (!confirm) return;

                    // Send trade request as a notification
                    var notification = new Notifications
                    {
                      //  Notification_ID = Guid.NewGuid().ToString(),
                        Sender_ID = currentUser.Id,
                        Receiver_ID = selectedProfile.Id,
                        message = $"SHIFT TRADE REQUEST: {currentProfile.First} {currentProfile.Last} wants to trade their {currentShift.Shift_type} shift on {currentShift.Shift_date:MMM dd} " +
                                  $"with your {selectedTradeShift.Shift_type} shift on {selectedTradeShift.Shift_date:MMM dd}.",
                        Send_Time = DateTime.UtcNow,
                        Receive_Time = DateTime.UtcNow
                    };

                    await _dataService.SupabaseClient
                        .From<Notifications>()
                        .Insert(notification);

                    await DisplayAlert("Request Sent", "Your shift trade request has been sent.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Trade request failed: {ex.Message}", "OK");
                }
            }
        }




        private DateTime GetShiftStartTime(string shiftType, DateTime shiftDate)
        {
            return shiftType switch
            {
                "First Shift" => shiftDate.Date.AddHours(9),
                "Second Shift" => shiftDate.Date.AddHours(17),
                "Third Shift" => shiftDate.Date.AddHours(22),
                _ => throw new ArgumentException("Invalid shift type")
            };
        }

        private DateTime GetShiftEndTime(string shiftType, DateTime shiftDate)
        {
            return shiftType switch
            {
                "First Shift" => shiftDate.Date.AddHours(17),
                "Second Shift" => shiftDate.Date.AddHours(22),
                "Third Shift" => shiftDate.Date.AddDays(1).AddHours(3),
                _ => throw new ArgumentException("Invalid shift type")
            };
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(); // Close the modal and return to the calendar view
        }

        private void OnDeleteEventClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is CalendarEvent calendarEvent)
            {
                EventStorage.DeleteEvent(calendarEvent);
                LoadEvents(); // Refresh the list
            }
        }

    }
}

