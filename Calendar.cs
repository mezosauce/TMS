using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;

namespace Time_Management_System.Control;


public class CalendarView : ContentView
{
    private readonly Grid _calendarGrid;
    private readonly Label _monthLabel;
    private DateTime _currentDate;

    private DataService _dataService;

    private List<Time> _monthShifts = new();



   
    public CalendarView()
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
                .Where(t => t.Shift_date >= firstDay)
                .Get();

            System.Diagnostics.Debug.WriteLine($"Supabase response: {response?.Models?.Count ?? -1} items");

            _monthShifts = response.Models.ToList();

            System.Diagnostics.Debug.WriteLine($"_monthShifts assigned: {_monthShifts.Count} items");
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
                FontSize = 12,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                TextColor = isCurrentMonth ? Colors.Black : Colors.LightGray,
                Margin = new Thickness(0, 0, 2, 0)
            };

            var shifts = _monthShifts.Where(s => s.Shift.Date == cellDate.Date).OrderBy(s => s.Shift).ToList();

            var eventStack = new StackLayout
            {
                Spacing = 1,
                Padding = new Thickness(0),
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
            };

            foreach (var shift in shifts)
            {
                var label = new Label
                {
                    Text = $"Shift: {shift.Clocked_in:HH:mm}-{shift.Clocked_out:HH:mm} ({shift.Hours}h)",
                    FontSize = 10,
                    TextColor = Colors.DarkGreen,
                    LineBreakMode = LineBreakMode.TailTruncation
                };
                eventStack.Children.Add(label);
            }



            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) =>
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(new Pages.EventModalPage(cellDate));
            };

            var dayContent = new Grid();
            dayContent.Children.Add(dayLabel);
            if (eventStack.Children.Any())
            {
                dayContent.Children.Add(eventStack);
            }

            dayContent.GestureRecognizers.Add(tapGesture);

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

    public void Rebuild()
    {
        BuildCalendar();
    }
}
