﻿using Microsoft.Extensions.Logging;
using Supabase;
using Supabase.Interfaces;


namespace Time_Managmeent_System
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();



            var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
            var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");
            //Automatic login persistence (like remembering a user when reopening the app)
            /* var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
            }; */
            // Note the creation as a singleton.
            builder.Services.AddSingleton(provider => new Supabase.Client(url, key));

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa-brands-400.ttf", "FaBrands");
                    fonts.AddFont("fa-regular-400.ttf", "FaRegular");
                    fonts.AddFont("fa-solid-900.ttf", "FaSolid");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build(); // Ensure this return statement is present to fix CS0161
        }
    }

    }
