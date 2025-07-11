using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace Time_Managmeent_System.Models;


[Table("geolocating")]
public class Geolocation : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; } // Ensure this matches your database schema
    [Column("name")]
    public string Name { get; set; }
    [Column("Longitude")]
    public string Longitude { get; set; } //Consider making Longitude and Latitude double if they are numeric values
    [Column("Latitude")]
    public string Latitude { get; set; }

}