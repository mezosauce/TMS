using Microsoft.Extensions.Configuration;

public static class EnvironmentConfig
{
    private static IConfiguration _config;

    static EnvironmentConfig()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static string SUPABASE_URL => _config["Supabase:Url"];
    public static string SUPABASE_KEY => _config["Supabase:Key"];
}