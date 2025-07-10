using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time_Managmeent_System.Models;

[Table("user_data")]
public class Audit : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; } // Ensure this matches your database schema
    [Column("a_id")]
    public string auditID { get; set; }
    [Column("change")]
    public string change { get; set; }
    
}
