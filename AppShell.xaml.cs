using Time_Managmeent_System.Pages;

namespace Time_Managmeent_System;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(typeof(LoginPage).Name, typeof(LoginPage));

    }

}
