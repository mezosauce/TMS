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

    public async Task<List<Time_Managmeent_System.Models.Geolocation>> GetAllGeolocationsAsync()
    {
        var response = await SupabaseClient.From<Time_Managmeent_System.Models.Geolocation>().Get();
        return response.Models;
    }

    public async Task SetGeolocationActiveAsync(string id)
    {
        // Get all entries
        var allEntries = await SupabaseClient.From<Time_Managmeent_System.Models.Geolocation>().Get();

        // Set all to inactive
        foreach (var entry in allEntries.Models)
        {
            if (entry.Active)
            {
                entry.Active = false;
                await SupabaseClient.From<Time_Managmeent_System.Models.Geolocation>().Update(entry);
            }
        }

        // Find and update selected entry
        var selected = await SupabaseClient.From<Time_Managmeent_System.Models.Geolocation>().Where(x => x.Id == id).Single();
        if (selected != null)
        {
            selected.Active = true;
            await SupabaseClient.From<Time_Managmeent_System.Models.Geolocation>().Update(selected);
        }
    }
}
