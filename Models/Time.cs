using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
namespace Time_Managmeent_System.Models;


[Table("time_log")]
public class Time: BaseModel
{
    [PrimaryKey("tid", false)]
    public string Tid { get; set; } // Ensure this matches your database schema
    [Column("id")]
    public string User_ID { get; set; }
    
    [Column("clock_in")]
    public DateTimeOffset Clocked_in { get; set; } // Consider using  if you need timezone support
    [Column("clock_out")]
    public DateTimeOffset Clocked_out { get; set; } // Nullable if the notification can be unread
    [Column("hours")]
    public double Hours { get; set; } // Total hours worked in this shift
    [Column("status")]
    public bool Status { get; set; } // True for completed, false for pending
    [Column("shift_date")]
    public DateTime Shift_date { get; set; } // Date of the shift, used for calendar display
    [Column("shift_type")]
    public string Shift_type { get; set; } // Type of shift (e.g., First Shift, Second Shift, Third Shift)

}
