using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Supabase;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Time_Managmeent_System.Pages;
using Time_Managmeent_System.Services;
using Time_Managmeent_System.ViewModels;


namespace Time_Managmeent_System;


    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; }

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            ServiceProvider = serviceProvider;
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider is not initialized.");
            }
            var LoginPage = ServiceProvider.GetService<LoginPage>();
            if (LoginPage == null)
            {
                throw new InvalidOperationException("LoginPage is not registered in the service provider.");
            }
            MainPage = new NavigationPage(LoginPage);
        }

        
        
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return base.CreateWindow(activationState);
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
}

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Set the main page to the Shell
        MainPage = new AppShell();
    }
}