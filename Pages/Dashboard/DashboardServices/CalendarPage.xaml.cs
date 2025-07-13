using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Time_Management_System.Control;
using Microsoft.Maui.Controls.Compatibility;
using Time_Managmeent_System.Services;

using Layout = Microsoft.Maui.Controls.Layout; // Explicitly alias the Layout type

namespace Time_Management_System.Pages;


public partial class CalendarPage : ContentPage
{


    private readonly DataService _dataService;
    public CalendarPage(DataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Content is Layout layout)
        {
            var calendarView = FindDescendant<CalendarView>(layout);
            calendarView?.Rebuild();
        }
    }

    private T FindDescendant<T>(Layout layout) where T : View
    {
        foreach (var child in layout.Children)
        {
            if (child is T match)
                return match;

            if (child is Layout childLayout)
            {
                var descendant = FindDescendant<T>(childLayout);
                if (descendant != null)
                    return descendant;
            }
        }
        return null;
    }
}

