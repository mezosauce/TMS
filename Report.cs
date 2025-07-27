using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;
using System.Collections.Immutable;


namespace Time_Management_System.Control;

public class ReportView : ContentView
{
    private readonly Grid _calendarGrid;
    private readonly Label _monthLabel;
    private DateTime _currentDate;

    private DataService _dataService;

    private List<Time> _monthShifts = new();

    private Dictionary<string, string> _userNames = new();
    private Dictionary<string, string> _userPositions = new();


    public ReportView()
    {
        _currentDate = DateTime.Today;


        _monthLabel = new Label
        {
            Text = _currentDate.ToString("MMMM yyyy"),
            FontSize = 20,
            HorizontalOptions = LayoutOptions.Center
        };



        var prevButton = new Button { Text = "<" };
        prevButton.Clicked += (s, e) => { _currentDate = _currentDate.AddMonths(-1); BuildCalendar(); };

        var nextButton = new Button { Text = ">" };
        nextButton.Clicked += (s, e) => { _currentDate = _currentDate.AddMonths(1); BuildCalendar(); };

        var header = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Children = { prevButton, _monthLabel, nextButton }
        };

        _calendarGrid = new Grid
        {
            RowSpacing = 0,
            ColumnSpacing = 0,
            Padding = 0
        };

        for (int i = 0; i < 7; i++)
            _calendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

        var calendarContainer = new Grid
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            RowDefinitions = { new RowDefinition { Height = GridLength.Star } },
            Children = { _calendarGrid }
        };

        var layout = new Grid();
        layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

        layout.Children.Add(header);
        Grid.SetRow(header, 0);

        layout.Children.Add(calendarContainer);
        Grid.SetRow(calendarContainer, 1);

        Content = layout;


    }

    public DataService DataService
    {
        get => _dataService;
        set
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Info",
                    "CalendarView.DataService setter called",
                    "OK");
            });
            _dataService = value;
            if (_dataService != null)
                _ = LoadAndBuildCalendarAsync();
        }
    }

    private async Task LoadAndBuildCalendarAsync()
    {
        await LoadMonthShiftsAsync();
        BuildCalendar();
    }



    private async Task LoadMonthShiftsAsync()
    {
        try
        {
            var firstDay = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            var response = await _dataService.SupabaseClient
                .From<Time>()
                .Where(t => t.Shift_date < firstDay)
                .Get();



            System.Diagnostics.Debug.WriteLine($"Supabase response: {response?.Models?.Count ?? -1} items");

            _monthShifts = response.Models.ToList();

            System.Diagnostics.Debug.WriteLine($"_monthShifts assigned: {_monthShifts.Count} items");

            var userIds = _monthShifts
                .Where(s => !string.IsNullOrEmpty(s.User_ID))
                .Select(s => s.User_ID)
                .Distinct()
                .ToList();

            // Load corresponding user profiles from Supabase
            if (userIds.Any())
            {
                var userProfilesResponse = await _dataService.SupabaseClient
                    .From<UserProfile>()
                    .Get();




                // userProfilesResponse.Models is likely IEnumerable<UserProfile>
                var userProfiles = userProfilesResponse.Models
                    .Where(u => userIds.Contains(u.Id))
                    .ToList();

                _userNames = userProfiles.ToDictionary(
                    u => u.Id,
                    u => $"{u.First} {u.Last}" // adjust field names based on your actual schema
                );
                _userPositions = userProfiles.ToDictionary(
                    u => u.Id,
                    u => u.Position ?? "Employee" // default to "Employee" if null
                );

                System.Diagnostics.Debug.WriteLine($"Loaded {_userNames.Count} user names");
            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception in LoadMonthShiftsAsync: {ex}");
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load shifts: {ex.Message}", "OK");
            });
        }
    }

    private void BuildCalendar()
    {
        _calendarGrid.Children.Clear();
        _calendarGrid.RowDefinitions.Clear();

        _monthLabel.Text = _currentDate.ToString("MMMM yyyy");

        // Day headers  
        string[] days = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
        _calendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (int i = 0; i < 7; i++)
        {
            var label = new Label
            {
                Text = days[i],
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            _calendarGrid.Children.Add(label);
            Grid.SetColumn(label, i);
            Grid.SetRow(label, 0);
        }

        // Calendar grid
        DateTime firstOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
        int startDay = (int)firstOfMonth.DayOfWeek;
        int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
        DateTime startDate = firstOfMonth.AddDays(-startDay);
        int totalCells = 42; // 6 weeks * 7 days

        for (int row = 1; row <= 6; row++)
            _calendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

        for (int i = 0; i < totalCells; i++)
        {
            int row = (i / 7) + 1;
            int col = i % 7;
            DateTime cellDate = startDate.AddDays(i);
            bool isCurrentMonth = cellDate.Month == _currentDate.Month;

            var dayLabel = new Label
            {
                Text = cellDate.Day.ToString(),
                FontSize = 6,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                TextColor = isCurrentMonth ? Colors.Black : Colors.LightGray,
                Margin = new Thickness(0, 0, 2, 0)
            };

            var shifts = _monthShifts.Where(s => s.Shift_date.Date == cellDate.Date).OrderBy(s => s.Shift_date).ToList();

            var eventStack = new StackLayout
            {
                Spacing = 1,
                Padding = new Thickness(0),
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
            };

            foreach (var shift in shifts)
            {
                _userNames.TryGetValue(shift.User_ID, out string employeeName);
                _userPositions.TryGetValue(shift.User_ID, out string position);

                var startTime = GetShiftStartTime(shift.Shift_type, shift.Shift_date);
                var endTime = GetShiftEndTime(shift.Shift_type, shift.Shift_date);




                var backgroundColor = position == "Manager" ? Colors.LightCoral : Colors.LightGreen;

                var label = new Label
                {
                    Text = $"{employeeName}: {startTime:hh:mm tt} - {endTime:hh:mm tt}",
                    FontSize = 6,
                    TextColor = Colors.DarkGreen,
                    LineBreakMode = LineBreakMode.TailTruncation,
                    BackgroundColor = backgroundColor,
                };
                eventStack.Children.Add(label);
            }





            var dayContent = new Grid();
            dayContent.Children.Add(dayLabel);
            if (eventStack.Children.Any())
            {
                dayContent.Children.Add(eventStack);
            }



            var dayFrame = new Frame
            {
                BorderColor = Colors.Gray,
                BackgroundColor = Colors.White,
                CornerRadius = 0,
                Padding = new Thickness(4),
                HasShadow = false,
                Content = dayContent
            };

            _calendarGrid.Children.Add(dayFrame);
            _calendarGrid.SetColumn((IView)dayFrame, col);
            _calendarGrid.SetRow((IView)dayFrame, row);
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
            "First Shift" => shiftDate.Date.AddHours(17),           // 5:00 PM
            "Second Shift" => shiftDate.Date.AddHours(22),          // 10:00 PM
            "Third Shift" => shiftDate.Date.AddDays(1).AddHours(3), // 3:00 AM next day
            _ => throw new ArgumentException("Invalid shift type")
        };
    }

    public void Rebuild()
    {
        BuildCalendar();
    }
}