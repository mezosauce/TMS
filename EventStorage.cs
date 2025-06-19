using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Time_Management_System.Models;

namespace Time_Management_System.Services
{
    public static class EventStorage
    {
        private static readonly List<CalendarEvent> _events = new();

        public static List<CalendarEvent> GetEvents(DateTime date) =>
            _events.Where(e => e.Date.Date == date.Date).ToList();

        public static void AddEvent(CalendarEvent calendarEvent) =>
            _events.Add(calendarEvent);
    }
}
