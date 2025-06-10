using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;

namespace Time_Managmeent_System.ViewModels;

[ObservableObject] // Fix for MVVMTK0019: Add [ObservableObject] to make the class compatible with ObservableProperty  
[QueryProperty(nameof(Employee), "EmployeeObject")]
public partial class UpdateEmployeeViewModel
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private Employee _employee; // Fix for IDE0044: Add readonly modifier  

    public UpdateEmployeeViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    [RelayCommand]
    private async Task UpdateEmployee()
    {
        if (!string.IsNullOrEmpty(Employee.Username))
        {
            await _dataService.UpdateEmployee(Employee);

            await Shell.Current.GoToAsync("Employee Successfully Updated");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No Username", "OK");
        }
    }
}
