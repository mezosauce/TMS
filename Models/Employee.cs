using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time_Managmeent_System.Models;


[Table("Employee")]
public class Employee : BaseModel
{

    [PrimaryKey("id", false)]
    public int Id { get; set; }
    [Column("Username")]
    public string Username { get; set; }
    [Column("Password")]
    public string Password { get; set; }
    [Column("Position")]
    public string Position { get; set; }
    [Column("First")]
    public string First { get; set; }
    [Column("Last")]
    public string Last { get; set; }
}
