
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

    public async Task<IEnumerable<Employee>> GetEmployees()
        {
            var response = await SupabaseClient.From<Employee>().Get();
            return response.Models.OrderByDescending(b => b.Id);
        }


        public async Task CreateEmployee(Employee employee)
       {
        await SupabaseClient.From<Employee>().Insert(employee);
        }

        public async Task DeleteEmployee(int id)
    {
        await SupabaseClient.From<Employee>().Where(b => b.Id == id).Delete();
    }
        public async Task UpdateEmployee(Employee employee)
        {
        await SupabaseClient.From<Employee>().Where(b => b.Id == employee.Id)
        .Set(b => b.Username, employee.Username)
        .Set(b => b.Password, employee.Password)
        .Set(b => b.Position, employee.Position)
        .Set(b => b.First, employee.First)
        .Set(b => b.Last, employee.Last)
        .Update();

        }
}



