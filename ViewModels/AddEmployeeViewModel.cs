using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;


namespace Time_Managmeent_System.ViewModels;

public partial class AddEmployeeViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string _EmployeeUsername;
    [ObservableProperty]
    private string _EmployeePassword;
    [ObservableProperty]
    private string _EmployeePosition;
    [ObservableProperty]
    private string _EmployeeFirst;
    [ObservableProperty]
    private string _EmployeeLast;


    public AddEmployeeViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    [RelayCommand]
    public async Task AddEmployee()
    {
        try
        {
            if (!string.IsNullOrEmpty(EmployeeUsername))
            {
                Employee employee = new()
                {
                    Username = EmployeeUsername,
                    Password = EmployeePassword,
                    Position = EmployeePosition,
                    First = EmployeeFirst,
                    Last = EmployeeLast
                };
                await _dataService.CreateEmployee(employee);

                await Shell.Current.GoToAsync("Success");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No title!", "OK");
            }

        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
