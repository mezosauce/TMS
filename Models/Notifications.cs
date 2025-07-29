using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time_Managmeent_System.Models;


[Table("notifications")]
public class Notifications : BaseModel
{
    [PrimaryKey("n_id", false)]
    public string Notification_ID { get; set; } // Ensure this matches your database schema
    [Column("s_id")]
    public string Sender_ID { get; set; }
    [Column("r_id")]
    public string Receiver_ID { get; set; }
    
    [Column("message")]
    public string message { get; set; } // This should be a string or text type in your database
    
    [Column("s_time")]
    public DateTime Send_Time { get; set; } // Consider using  if you need timezone support
    [Column("r_time")]
    public DateTime Receive_Time { get; set; } // Nullable if the notification can be unread
}

