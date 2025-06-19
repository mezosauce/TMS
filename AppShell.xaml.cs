using Time_Managmeent_System.Pages;

namespace Time_Managmeent_System;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        RegisterForRoute<AddEmployeePage>();
        RegisterForRoute<UpdateEmployeePage>();

    }

    protected void RegisterForRoute<T>()
    {
        Routing.RegisterRoute(typeof(T).Name, typeof(T));
    }
}
