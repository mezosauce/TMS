
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

    [RelayCommand]
    private async Task AddEmployee() => await Shell.Current.GoToAsync("AddEmployeePage");


    [RelayCommand]
    private async Task DeleteEmployee(Employee employee)
    {
        var result = await Shell.Current.DisplayAlert("Delete", $"Are you sure you want to delete \"{employee.First}\"?", "Yes", "No");

        if (result is true)
        {
            try
            {
                await _dataService.DeleteEmployee(employee.Id);
                await GetEmployees();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }


    [RelayCommand]
    private async Task UpdateEmployee(Employee employee) => await Shell.Current.GoToAsync("UpdateEmployeePage", new Dictionary<string, object>
    {
        { "EmployeeObject", employee }
    });
}
