using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time_Managmeent_System.Models;

[Table("audit_log")]
public class Audit : BaseModel
{
    [PrimaryKey("a_id", false)]
    public string Id { get; set; } // Ensure this matches your database schema

    [Column("change")]
    public string change { get; set; }
    
}
