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
                    await _dataService.SupabaseClient
                        .From<Time>()
                        .Where(t => t.User_ID == shift.User_ID)
                        .Delete();

                    await DisplayAlert("Success", "Shift canceled successfully.", "OK");
                    LoadEvents(); // Refresh list
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to cancel shift: {ex.Message}", "OK");
                }
            }
        }

        private async void OnTradeShiftClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Time shift)
            {
                // TODO: implement trade logic or open a trade request modal
                await DisplayAlert("Trade Shift", $"Request to trade shift {shift.Shift_type} on {shift.Shift_date:d}.", "OK");
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

