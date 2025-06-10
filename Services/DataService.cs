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


        public async Task CreateEmployee(Employee employee)
       {
        await _supabaseClient.From<Employee>().Insert(employee);
        }

        public async Task DeleteEmployee(int id)
    {
        await _supabaseClient.From<Employee>().Where(b => b.Id == id).Delete();
    }
        public async Task UpdateEmployee(Employee employee)
        {
        await _supabaseClient.From<Employee>().Where(b => b.Id == employee.Id)
        .Set(b => b.Username, employee.Username)
        .Set(b => b.Password, employee.Password)
        .Set(b => b.Position, employee.Position)
        .Set(b => b.First, employee.First)
        .Set(b => b.Last, employee.Last)
        .Update();

        }
}



