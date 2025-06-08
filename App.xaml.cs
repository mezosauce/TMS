using Time_Managmeent_System.Pages;
using Time_Managmeent_System.ViewModels;
namespace Time_Managmeent_System;

public partial class App : Application
{
    private readonly EmployeesListingViewModel _employeesListingViewModel;

    public App(EmployeesListingViewModel employeesListingViewModel)
    {
        InitializeComponent();
        _employeesListingViewModel = employeesListingViewModel;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Pass the required parameter to the LoginPage constructor  
        var window = new Window(new NavigationPage(new Pages.LoginPage(_employeesListingViewModel)));
        return window;
    }

    public void ToggleTheme()
    {
        // Toggle between Light and Dark mode  
        if (App.Current.UserAppTheme == AppTheme.Dark)
        {
            App.Current.UserAppTheme = AppTheme.Light;
        }
        else
        {
            App.Current.UserAppTheme = AppTheme.Dark;
        }
    }
}