using Supabase;
using Supabase.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using Time_Managmeent_System.Models;
using System.Net.Http.Json;


namespace Time_Managmeent_System.Services;

public class DataService : IDataService
{
    public Client SupabaseClient { get; private set; }
    private readonly HttpClient _httpClient;

    public DataService(Supabase.Client supabaseClient)
    {
        var url = AppConfig.SUPABASE_URL;
        var key = AppConfig.SUPABASE_KEY;
        SupabaseClient = new Client(url, key);
        _httpClient = new HttpClient();
    }


    public async Task<bool> CallDeleteUserEdgeFunctionAsync(string userId)
    {
        try
        {
            var functionUrl = $"{AppConfig.SUPABASE_URL}/functions/v1/delete-user";

            // Add Authorization header
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppConfig.SUPABASE_KEY);

            // Replace with your actual edge function URL
            var functionUrl = $"{AppConfig.SUPABASE_URL}/functions/v1/delete-user";

            // If your function expects JSON body:
            var payload = new { user_id = userId };
            var response = await _httpClient.PostAsJsonAsync(functionUrl, payload);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"User '{userId}' deleted via edge function.");
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to delete user '{userId}': {error}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling edge function: {ex.Message}");
            return false;
        }
    }
}

