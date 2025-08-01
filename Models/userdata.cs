using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Time_Managmeent_System.Models;

// Define a UserProfile class to match the structure of your user_data table
[Table("user_data")]
public class UserProfile : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public string Id { get; set; } // Ensure this matches your database schema
    [Column("First")]
    public string First { get; set; }
    [Column("Last")]
    public string Last { get; set; }
    [Column("Position")]
    public string Position { get; set; }
    [Column("avatar_url")]
    public string AvatarUrl { get; set; }
}
