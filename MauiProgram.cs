using Microsoft.Extensions.Logging;
using Supabase;
using Supabase.Postgrest.Attributes;
using CommunityToolkit.Maui;
using Time_Managmeent_System;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.ViewModels;
using Time_Managmeent_System.Pages;
using Time_Managmeent_System.Services;

namespace Time_Management_System;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>()
            .UseMauiCommunityToolkit()

            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa-brands-400.ttf", "FaBrands");
                fonts.AddFont("fa-regular-400.ttf", "FaRegular");
                fonts.AddFont("fa-solid-900.ttf", "FaSolid");
            });

        //Configure Supabase
        var url = AppConfig.SUPABASE_URL;
        var key = AppConfig.SUPABASE_KEY;
        builder.Services.AddSingleton(provider => new Supabase.Client(url, key));


        //Add ViewModels

        builder.Services.AddSingleton<EmployeesListingViewModel>();

        //Add Pages
        builder.Services.AddSingleton<LoginPage>();

        //Data Service
        builder.Services.AddSingleton<IDataService, DataService>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build(); // Ensure this return statement is present to fix CS0161
    }
}