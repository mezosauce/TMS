using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Time_Management_System.Models;
using Time_Management_System.Services;

namespace Time_Management_System.Pages
{
    public partial class EventModalPage : ContentPage
    {
        private DateTime _selectedDate;
        private Label _dateLabel; // Renamed to avoid ambiguity

        public EventModalPage(DateTime selectedDate)
        {
            InitializeComponent(); // Ensure this method is generated in the corresponding XAML file
            _selectedDate = selectedDate;

            // Ensure _dateLabel is defined in the XAML file and properly linked
            _dateLabel = new Label(); // Temporary fix if DateLabel is missing
            _dateLabel.Text = selectedDate.ToString("D");
            LoadEvents();
        }

        private void LoadEvents()
        {
            EventList.ItemsSource = EventStorage.GetEvents(_selectedDate);
        }

        private void OnAddEventClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                EventStorage.AddEvent(new CalendarEvent
                {
                    Date = _selectedDate,
                    Title = TitleEntry.Text,
                    Description = DescriptionEditor.Text,
                    Time = TimePicker.Time
                });

                TitleEntry.Text = string.Empty;
                DescriptionEditor.Text = string.Empty;
                LoadEvents();
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(); // Close the modal and return to the calendar view
        }
    }
}

