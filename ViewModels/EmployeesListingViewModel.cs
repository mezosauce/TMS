using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Time_Managmeent_System.ViewModels;
public partial class EmployeesListingViewModel : ObservableObject
{
    private readonly IDataService _dataService;
    public ObservableCollection<Employee> Employees { get; set; } = new();

    
    public EmployeesListingViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    [RelayCommand]
    public async Task GetEmployees()
    {
        Employees.Clear();

        try
        {
            var employees = await _dataService.GetEmployees();

            if (employees.Any())
            {
                foreach (var employee in employees)
                {
                    Employees.Add(employee);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }

    }
}
