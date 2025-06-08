using Microsoft.Extensions.Logging;
using Supabase;

namespace Time_Management_System
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var url = EnvironmentConfig.SUPABASE_URL;
            var key = EnvironmentConfig.SUPABASE_KEY;

            // Register Supabase client as a singleton
            builder.Services.AddSingleton<Supabase.Client>(provider => new Supabase.Client(url, key));

            // Register a service that uses the Supabase client
            builder.Services.AddSingleton<SupabaseService>();

            builder
                .UseMauiApp<Time_Managmeent_System.App>()
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