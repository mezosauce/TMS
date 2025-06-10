using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;
namespace Time_Managmeent_System.ViewModels;

public partial class AddEmployeeViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string _Username;
    [ObservableProperty]
    private string _Password;
    [ObservableProperty]
    private string _Position;
    [ObservableProperty]
    private string _First;
    [ObservableProperty]
    private string _Last;


    public AddEmployeeViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    [RelayCommand]
    public async Task AddEmployee()
    {
        try
        {
            if (!string.IsNullOrEmpty(Username))
            {
                Employee employee = new()
                {
                    Username = Username,
                    Password = Password,
                    Position = Position,
                    First = First,
                    Last = Last
                };
                await _dataService.CreateEmployee(employee);

                await Shell.Current.GoToAsync("Success");
            }

        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
