using CommunityToolkit.Mvvm.DependencyInjection;
using Supabase;
using Time_Managmeent_System.Pages;
using Time_Managmeent_System.Services;
using Time_Managmeent_System.ViewModels;
namespace Time_Managmeent_System;

public partial class App : Application
{

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Pass the required parameter to the LoginPage constructor
        var loginPage = Current.Services.GetService<LoginPage>();
        var window = new Window(loginPage);
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