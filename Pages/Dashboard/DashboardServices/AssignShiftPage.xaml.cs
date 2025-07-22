using Microsoft.EntityFrameworkCore.Metadata;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using Time_Management_System.Services; // For EventStorage
using Time_Management_System.Models; // Ensure this namespace contains CalendarEvent

namespace Time_Managmeent_System.Pages.Dashboard.DashboardServices;

public partial class AssignShiftPage : ContentPage
{
    private readonly DataService _dataService;
    public ObservableCollection<Shift> ShiftOptions { get; set; }
    public ObservableCollection<string> EmployeeOptions { get; set; }

    public AssignShiftPage(DataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        ShiftOptions = new ObservableCollection<Shift>
        {
            new Shift { Name = "First Shift", Start = new TimeSpan(9, 0, 0), End = new TimeSpan(17, 0, 0) },
            new Shift { Name = "Second Shift", Start = new TimeSpan(17, 0, 0), End = new TimeSpan(22, 0, 0) },
            new Shift { Name = "Third Shift", Start = new TimeSpan(22, 0, 0), End = new TimeSpan(3, 0, 0) }
        };
        EmployeeOptions = new ObservableCollection<string>();
        BindingContext = this;
        _ = LoadEmployeesAsync();
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
        var selectedEmployee = EmployeePicker.SelectedItem as string;
        var selectedShift = ShiftPicker.SelectedItem as Shift;
        if (selectedEmployee == null || selectedShift == null)
        {
            await DisplayAlert("Error", "Please select both employee and shift.", "OK");
            return;
        }

        // Find the next Monday (or today if today is Monday)
        var today = DateTime.Today;
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        var monday = today.AddDays(daysUntilMonday);

        var events = new List<CalendarEvent>();
        for (int i = 0; i < 5; i++)
        {
            var date = monday.AddDays(i);
            var calendarEvent = new CalendarEvent
            {
                Date = date,
                Title = $"{selectedShift.Name} - {selectedEmployee}",
                Description = $"{selectedEmployee} assigned to {selectedShift.Name} ({selectedShift.Start:hh\\:mm} - {selectedShift.End:hh\\:mm})",
                Time = selectedShift.Start
            };
            events.Add(calendarEvent);
        }

        foreach (var evt in events)
        {
            EventStorage.AddEvent(evt);
        }

        await DisplayAlert("Assigned", $"{selectedEmployee} assigned to {selectedShift.Name} for Monday to Friday.", "OK");
    }
}

public class Shift
{
    public string Name { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public override string ToString() => Name;
}