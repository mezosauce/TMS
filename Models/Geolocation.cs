using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace Time_Managmeent_System.Models;


[Table("geolocating")]
public class Geolocating : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; } // Ensure this matches your database schema
    [Column("name")]
    public string Name { get; set; }
    [Column("longitude")]
    public string Longitude { get; set; } //Consider making Longitude and Latitude double if they are numeric values
    [Column("latitude")]
    public string Latitude { get; set; }
    [Column("active")]
    public bool Active { get; set; } 


}