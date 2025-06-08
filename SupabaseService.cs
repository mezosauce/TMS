using Supabase;

namespace Time_Management_System
{
    public class SupabaseService
    {
        private readonly Client _client;

        public SupabaseService(Client client)
        {
            _client = client;
        }

        // Example method to interact with Supabase
        public async Task<List<T>> GetDataAsync<T>(string tableName) where T : class
        {
            var response = await _client.From<T>(tableName).Get();
            return response.Models;
        }
    }
}
