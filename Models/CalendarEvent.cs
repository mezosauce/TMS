﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time_Managmeent_System.Models
{
    public class CalendarEvent
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TimeSpan? Time { get; set; }
    }
}
