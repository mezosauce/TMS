using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time_Managmeent_System.Models;


[Table("time_log")]
public class Time: BaseModel
{
    [PrimaryKey("tid", false)]
    public string Time_ID { get; set; } // Ensure this matches your database schema
    [Column("id")]
    public string User_ID { get; set; }
    [Column("shift")]
    public DateTimeOffset Shift { get; set; } // Day to assign in calendar
    [Column("clock-in")]
    public DateTimeOffset Clocked_in { get; set; } // Consider using DateTimeOffset if you need timezone support
    [Column("clock-out")]
    public DateTimeOffset Clocked_out { get; set; } // Nullable if the notification can be unread
    [Column("status")]
    public bool Status { get; set; } // True for completed, false for pending

}
