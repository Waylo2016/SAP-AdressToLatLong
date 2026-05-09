using Microsoft.Extensions.Configuration;

namespace SAP_AdresToLatLong.Helpers;

public class SecretParser
{
    private static IConfiguration configuration;
    
    private static string? _envDirectory;

    static SecretParser()
    {
        string envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (File.Exists(envFile))
        {
            foreach (string line in File.ReadAllLines(envFile))
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#')) continue;
                int idx = trimmed.IndexOf('=');
                if (idx < 0) continue;
                string key = trimmed[..idx].Trim();
                string value = trimmed[(idx + 1)..].Trim();
                if (Environment.GetEnvironmentVariable(key) == null)
                    Environment.SetEnvironmentVariable(key, value);
            }
        }
        
        configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
    }
    
    public static string Build()
    {
        
        string host = GetRequiredConfig(configuration, "POSTGRES_HOST");
        string port = GetRequiredConfig(configuration, "POSTGRES_PORT");
        string db = GetRequiredConfig(configuration, "POSTGRES_DB");
        string user = GetRequiredSecretFromFile(configuration, "POSTGRES_USER_FILE");
        string password = GetRequiredSecretFromFile(configuration, "POSTGRES_PASSWORD_FILE");

        return $"Host={host};Port={port};Database={db};Username={user};Password={password}";
    }
    
    
    public static string GetGeocodingApi()
    {
        string geocodingApi = GetRequiredConfig(configuration, "GEOCODING_API");
        
        return geocodingApi;
    }
    
    public static string GetUsername()
    {
        return GetRequiredSecretFromFile(configuration, "SAP_REST_USERNAME_FILE");
    }
    
    public static string GetPassword()
    {
        return GetRequiredSecretFromFile(configuration, "SAP_REST_PASSWORD_FILE");
    }
    
    
    
    static string GetRequiredConfig(IConfiguration configuration, string key)
    {
        string? value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing environment variable '{key}'");
        }

        return value;
    }

    static string GetRequiredSecretFromFile(IConfiguration configuration, string key)
    {
        string path = GetRequiredConfig(configuration, key);

        if (!Path.IsPathRooted(path) && _envDirectory != null)
        {
            path = Path.GetFullPath(Path.Combine(_envDirectory, path));
        }

        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"Secret file does not exist for '{key}': {path}");
        }

        string value = File.ReadAllText(path).Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Secret file for '{key}' is empty: {path}");
        }

        return value;
    }
}