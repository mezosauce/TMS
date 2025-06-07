using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Time_Managmeent_System.Pages.LoginType;


namespace Time_Managmeent_System.SUPABASE;

public class UserData : ContentPage
{
    [Table("Employee")]
    class Employee : BaseModel
    {
        [PrimaryKey("id")]
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

    // Usage example in a service or page
    public class UserDataService
    {
        private readonly Supabase.Client _supabase;

        public UserDataService(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task GetEmployeesAsync()
        {
            var result = await _supabase
                .From<Employee>()
                .Select(x => new object[] { x.Username, x.Position })
                .Get();
            
        }
    }
}