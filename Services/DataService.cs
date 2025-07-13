using Time_Managmeent_System.Models;
using Supabase;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Time_Managmeent_System.Services;

public class DataService : IDataService
{
    public Client SupabaseClient { get; private set; }

    public DataService(Supabase.Client supabaseClient)
    {
        var url = AppConfig.SUPABASE_URL;
        var key = AppConfig.SUPABASE_KEY;
        SupabaseClient = new Client(url, key);
    }

    public async Task<List<Geolocating>> GetAllGeolocationsAsync()
    {
        var response = await SupabaseClient.From<Geolocating>().Get();
        return response.Models;
    }

   
}
