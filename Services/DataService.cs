
using Time_Managmeent_System.Models;
using Supabase;

namespace Time_Managmeent_System.Services;
    

    public class DataService : IDataService
{
        public Client SupabaseClient { get; private set;}

    public DataService(Supabase.Client supabaseClient)
        {
        var url = AppConfig.SUPABASE_URL;
        var key = AppConfig.SUPABASE_KEY;
        SupabaseClient = new Client(url, key);
    }

}



