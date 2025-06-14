using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Time_Management_System.Control
{
    public class CalendarView : ContentView
    {
        public CalendarView()
        {
            Content = new Label
            {
                Text = "Calendar Placeholder",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }
    }
}

