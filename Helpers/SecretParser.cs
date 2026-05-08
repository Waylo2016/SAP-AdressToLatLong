using Microsoft.Extensions.Configuration;

namespace SAP_AdresToLatLong.Helpers;

public class SecretParser
{
    private static IConfiguration configuration;
    
    static SecretParser()
    {
        var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (File.Exists(envFile))
        {
            foreach (var line in File.ReadAllLines(envFile))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#')) continue;
                var idx = trimmed.IndexOf('=');
                if (idx < 0) continue;
                var key = trimmed[..idx].Trim();
                var value = trimmed[(idx + 1)..].Trim();
                Environment.SetEnvironmentVariable(key, value);
            }
        }
        
        configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
    }
    
    public static string Build()
    {
        
        var host = GetRequiredConfig(configuration, "POSTGRES_HOST");
        var port = GetRequiredConfig(configuration, "POSTGRES_PORT");
        var db = GetRequiredConfig(configuration, "POSTGRES_DB");
        var user = GetRequiredSecretFromFile(configuration, "POSTGRES_USER_FILE");
        var password = GetRequiredSecretFromFile(configuration, "POSTGRES_PASSWORD_FILE");

        return $"Host={host};Port={port};Database={db};Username={user};Password={password}";
    }

    public static string SapRestApi()
    {
        var sapRestApi = GetRequiredConfig(configuration, "SAP_REST_API");
        
        return sapRestApi;
    }
    
    public static string GeocodingApi()
    {
        var geocodingApi = GetRequiredConfig(configuration, "GEOCODING_API_FILE");
        
        return geocodingApi;
    }
    
    public static string SapUsername()
    {
        return GetRequiredSecretFromFile(configuration, "SAP_USERNAME_FILE");
    }
    
    public static string SapPassword()
    {
        return GetRequiredSecretFromFile(configuration, "SAP_PASSWORD_FILE");
    }
    
    
    
    static string GetRequiredConfig(IConfiguration configuration, string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing environment variable '{key}'");
        }

        return value;
    }

    static string GetRequiredSecretFromFile(IConfiguration configuration, string key)
    {
        var path = GetRequiredConfig(configuration, key);

        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"Secret file does not exist for '{key}': {path}");
        }

        var value = File.ReadAllText(path).Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Secret file for '{key}' is empty: {path}");
        }

        return value;
    }
}