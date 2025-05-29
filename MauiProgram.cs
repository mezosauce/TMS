using Microsoft.Extensions.Logging;

namespace Time_Managmeent_System
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder

                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
#if WINDOWS
                fonts.AddFont("fa-solid-900.otf", "FontAwesome");
                fonts.AddFont("fa-brands-400.otf", "FontAwesomeBrands");
                fonts.AddFont("fa-regular-400.otf", "FontAwesomeRegular");
#elif MACCATALYST
                fonts.AddFont("fa-solid-900.otf", "FontAwesome");
                fonts.AddFont("fa-brands-400.otf", "FontAwesomeBrands");
                fonts.AddFont("fa-regular-400.otf", "FontAwesomeRegular");
#elif IOS
                fonts.AddFont("fa-solid-900.otf", "FontAwesome");
                fonts.AddFont("fa-brands-400.otf", "FontAwesomeBrands");
                fonts.AddFont("fa-regular-400.otf", "FontAwesomeRegular");
#elif ANDROID
                fonts.AddFont("fa-solid-900.otf", "FontAwesome");
                fonts.AddFont("fa-brands-400.otf", "FontAwesomeBrands");
                fonts.AddFont("fa-regular-400.otf", "FontAwesomeRegular");
#endif
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
