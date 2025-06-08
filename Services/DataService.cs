using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Time_Managmeent_System.Models;
using Supabase;

namespace Time_Managmeent_System.Services;
    

    public class DataService : IDataService
{
        private readonly Client _supabaseClient;
        public DataService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            var response = await _supabaseClient.From<Employee>().Get();
            return response.Models.OrderByDescending(b => b.Id);
    }
}



