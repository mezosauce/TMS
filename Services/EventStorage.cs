// File: Services/EventStorage.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Time_Managmeent_System.Models; 

namespace Time_Managmeent_System.Services
{
    public static class EventStorage
    {
        private static readonly List<CalendarEvent> _events = new();

        public static void AddEvent(CalendarEvent calendarEvent)
        {
            _events.Add(calendarEvent);
        }

        public static void DeleteEvent(CalendarEvent calendarEvent)
        {
            _events.Remove(calendarEvent);
        }

        public static List<CalendarEvent> GetEvents(DateTime date)
        {
            return _events.Where(e => e.Date.Date == date.Date).ToList();
        }
    }
}
