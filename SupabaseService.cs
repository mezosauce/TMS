using Supabase;
using System.Threading.Tasks;

namespace Time_Management_System
{
    public class SupabaseService
    {
        private readonly Supabase.Client _client;

        public SupabaseService(Supabase.Client client)
        {
            _client = client;
        }

        public async Task InitializeAsync()
        {
            await _client.InitializeAsync();
        }

        // Add additional methods to interact with Supabase as needed
    }
}
