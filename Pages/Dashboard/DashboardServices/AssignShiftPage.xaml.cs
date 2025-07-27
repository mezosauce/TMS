using Microsoft.EntityFrameworkCore.Metadata;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;
using System.Collections.ObjectModel;
using System.Globalization;



namespace Time_Managmeent_System.Pages.Dashboard.DashboardServices;

public partial class AssignShiftPage : ContentPage
{
    private readonly DataService _dataService;
    public ObservableCollection<string> WeekdayDates { get; set; } = new();

    private readonly List<(string Name, TimeSpan Start, TimeSpan End)> ShiftOptions = new()
    {
        ("First Shift", new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
        ("Second Shift", new TimeSpan(17, 0, 0), new TimeSpan(22, 0, 0)),
        ("Third Shift", new TimeSpan(22, 0, 0), new TimeSpan(3, 0, 0)) // Crosses midnight, handle carefully
    };
    public ObservableCollection<string> EmployeeOptions { get; set; }

    public AssignShiftPage(DataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;

        ShiftPicker.ItemsSource = ShiftOptions.Select(s => s.Name).ToList();

        EmployeeOptions = new ObservableCollection<string>();
        BindingContext = this;
        GenerateWeekdayDates();
        _ = LoadEmployeesAsync();
    }

    private void GenerateWeekdayDates()
    {
        var today = DateTime.Today;
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        DateTime startDate = today.AddDays(daysUntilMonday); // Start from next Monday

        WeekdayDates.Clear();

        for (int week = 0; week < 4; week++) // Generate 4 weeks
        {
            var weekStart = startDate.AddDays(week * 7);
            for (int i = 0; i < 5; i++) // Monday to Friday
            {
                var date = weekStart.AddDays(i);
                WeekdayDates.Add(date.ToString("dddd, MMM dd yyyy")); // Example: "Monday, Jul 29 2025"
            }
        }
    }
    private async Task LoadEmployeesAsync()
    {
        try
        {
            // Query your "UserProfile" table for users with a specific position stored in the "Position" property
            var response = await _dataService.SupabaseClient
                .From<UserProfile>()
                .Where(x => x.Position == "Employee")
                .Get();

            EmployeeOptions.Clear();
            foreach (var profile in response.Models)
            {
                EmployeeOptions.Add($"{profile.First} {profile.Last}");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load employees: {ex.Message}", "OK");
        }
    }

    public async void OnAssignShiftClicked(object sender, EventArgs e)
    {
        var selectedEmployeeName = EmployeePicker.SelectedItem as string;
        var selectedShiftName = ShiftPicker.SelectedItem as string;

        if (string.IsNullOrEmpty(selectedEmployeeName) || string.IsNullOrEmpty(selectedShiftName))
        {
            await DisplayAlert("Error", "Please select both employee and shift.", "OK");
            return;
        }

        var fullName = selectedEmployeeName?.Split(' ');
        if (fullName == null || fullName.Length < 2)
        {
            await DisplayAlert("Error", "Could not parse selected employee name.", "OK");
            return;
        }
        var first = fullName[0];
        var last = fullName[1];

        var employeeResponse = await _dataService.SupabaseClient
            .From<UserProfile>()
            .Where(x => x.First == first && x.Last == last)
            .Single();

        if (employeeResponse == null)
        {
            await DisplayAlert("Error", "Selected employee not found.", "OK");
            return;
        }

        var userId = employeeResponse.Id; // Adjust property to your actual user ID field

        var selectedShift = ShiftOptions.First(s => s.Name == selectedShiftName);

        // Find the next Monday (or today if Monday)
        var today = DateTime.Today;
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        var monday = today.AddDays(daysUntilMonday);

        var timeEntries = new List<Time>();

        for (int i = 0; i < 5; i++) // Monday to Friday
        {
            var shiftDate = monday.AddDays(i);

            // Calculate clock-in and clock-out DateTimeOffset using shift times
            var clockIn = shiftDate.Add(selectedShift.Start); 
            var clockInUtc = DateTime.SpecifyKind(clockIn, DateTimeKind.Utc);
            var clockInOffset = new DateTimeOffset(clockInUtc);




            DateTimeOffset clockOutOffset;

            if (selectedShift.End > selectedShift.Start)
            {
                var clockOut = shiftDate.Add(selectedShift.End);
                clockOutOffset = new DateTimeOffset(clockOut); // Correct: uses system timezone
            }
            else
            {
                var clockOut = shiftDate.AddDays(1).Add(selectedShift.End);
                clockOutOffset = new DateTimeOffset(clockOut); // Correct: handles next-day end time
            }

            double hours = (clockOutOffset - clockInOffset).TotalHours;

            var timeEntry = new Time
            {
                Tid = Guid.NewGuid().ToString(), // Generate unique ID
                User_ID = userId,
                Clocked_in = clockInOffset,
                Clocked_out = clockOutOffset,
                Hours = hours,
                Status = false, // Pending by default
                Shift_date = shiftDate,
                Shift_type = selectedShift.Name
            };

            timeEntries.Add(timeEntry);
        }

        try
        {
            foreach (var timeEntry in timeEntries)
            {
                await _dataService.SupabaseClient
                    .From<Time>()
                    .Insert(timeEntry);
            }

            await DisplayAlert("Success", $"{selectedEmployeeName} assigned to {selectedShiftName} for Monday to Friday.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to assign shifts: {ex.Message}", "OK");
        }
    }
}

