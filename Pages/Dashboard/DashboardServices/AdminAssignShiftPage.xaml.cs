using System.Collections.ObjectModel;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;
using Supabase.Postgrest.Attributes;
namespace Time_Managmeent_System.Pages.Dashboard.DashboardServices;

public partial class AdminAssignShiftPage : ContentPage
{
    private readonly DataService _dataService;

    public ObservableCollection<string> RoleOptions { get; set; } = new() { "Manager", "Employee" };
    public ObservableCollection<string> EmployeeOptions { get; set; } = new();
    public ObservableCollection<string> WeekdayDates { get; set; } = new();

    private readonly List<(string Name, TimeSpan Start, TimeSpan End)> ShiftOptionsList = new()
    {
        ("First Shift", new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
        ("Second Shift", new TimeSpan(17, 0, 0), new TimeSpan(22, 0, 0)),
        ("Third Shift", new TimeSpan(22, 0, 0), new TimeSpan(3, 0, 0)) // overnight
    };

    public List<string> ShiftOptions => ShiftOptionsList.Select(s => s.Name).ToList();

    public AdminAssignShiftPage(DataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        BindingContext = this;

        GenerateWeekdayDates();
        RolePicker.SelectedIndexChanged += RolePicker_SelectedIndexChanged;
    }

    private void GenerateWeekdayDates()
    {
        var today = DateTime.Today;
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        var startDate = today.AddDays(daysUntilMonday);

        WeekdayDates.Clear();

        for (int week = 0; week < 4; week++)
        {
            var weekStart = startDate.AddDays(week * 7);
            for (int i = 0; i < 5; i++)
            {
                var date = weekStart.AddDays(i);
                WeekdayDates.Add(date.ToString("dddd, MMM dd yyyy"));
            }
        }
    }

    private async void RolePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedRole = RolePicker.SelectedItem as string;
        if (string.IsNullOrEmpty(selectedRole))
            return;

        await LoadEmployeesByRoleAsync(selectedRole);
    }

    private async Task LoadEmployeesByRoleAsync(string role)
    {
        try
        {
            var response = await _dataService.SupabaseClient
                .From<UserProfile>()
                .Where(x => x.Position == role)
                .Get();

            EmployeeOptions.Clear();
            foreach (var user in response.Models)
                EmployeeOptions.Add($"{user.First} {user.Last}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load users: {ex.Message}", "OK");
        }
    }

    public async void OnAssignShiftClicked(object sender, EventArgs e)
    {
        var selectedEmployeeName = EmployeePicker.SelectedItem as string;
        var selectedShiftName = ShiftPicker.SelectedItem as string;
        var selectedRole = RolePicker.SelectedItem as string;

        if (string.IsNullOrEmpty(selectedEmployeeName) ||
            string.IsNullOrEmpty(selectedShiftName) ||
            string.IsNullOrEmpty(selectedRole))
        {
            await DisplayAlert("Error", "Please select role, shift, and employee.", "OK");
            return;
        }

        var nameParts = selectedEmployeeName.Split(' ');
        if (nameParts.Length < 2)
        {
            await DisplayAlert("Error", "Invalid employee name format.", "OK");
            return;
        }

        var first = nameParts[0];
        var last = nameParts[1];


        var employee = await _dataService.SupabaseClient
            .From<UserProfile>()
            .Where(x => x.First == first && x.Last == last)
            .Single();

        
        if (employee == null)
        {
            await DisplayAlert("Error", "Employee not found.", "OK");
            return;
        }

        var selectedShift = ShiftOptionsList.First(s => s.Name == selectedShiftName);
        var today = DateTime.Today;
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        var monday = today.AddDays(daysUntilMonday);

        var timeEntries = new List<Time>();

        for (int i = 0; i < 5; i++)
        {
            var date = monday.AddDays(i);
            var clockIn = date.Add(selectedShift.Start);
            var clockOut = selectedShift.End > selectedShift.Start
                ? date.Add(selectedShift.End)
                : date.AddDays(1).Add(selectedShift.End);

            var timeEntry = new Time
            {
                Tid = Guid.NewGuid().ToString(),
                User_ID = employee.Id,
                Clocked_in = new DateTimeOffset(clockIn),
                Clocked_out = new DateTimeOffset(clockOut),
                Hours = (clockOut - clockIn).TotalHours,
                Status = false,
                Shift_date = date,
                Shift_type = selectedShift.Name
            };

            timeEntries.Add(timeEntry);
        }

        try
        {
            foreach (var entry in timeEntries)
            {
                await _dataService.SupabaseClient.From<Time>().Insert(entry);
            }

            await DisplayAlert("Success", $"Assigned {selectedShiftName} to {selectedEmployeeName} ({selectedRole}).", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to assign shift: {ex.Message}", "OK");
        }
    }
}
