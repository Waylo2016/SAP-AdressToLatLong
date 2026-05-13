using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;
using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Helpers;
using SAP_AdresToLatLong.Models;
using SAP_AdresToLatLong.Services;

namespace SAP_AdresToLatLong;

class Program
{
    async static Task Main(string[] args)
    {
        
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information); // show everything
        });

        var logger = loggerFactory.CreateLogger<SapApiClient>();
        
        SapApiClient sapApiClient = new SapApiClient(logger);
        GoogleApiClient googleApiClient = new GoogleApiClient();
        GeocodeService geocodeService = new GeocodeService(googleApiClient);
        
        string connectionString = SecretParser.Build();

        await using var dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options
        );

        SapLoginData loginData = new SapLoginData
        {
            CompanyDB = Environment.GetEnvironmentVariable("SAP_SERVER")!,
            UserName = SecretParser.GetUsername(),
            Password = SecretParser.GetPassword()
        };
        
        
        SapCookieData cookieData = sapApiClient.LoginSAPRestApi(loginData);
        Console.WriteLine("Login successful");

        
        List<SAPData> customerAddresses = sapApiClient.GetCustomerAddresses(cookieData);
        Console.WriteLine("Customer addresses have been got");
        
        sapApiClient.SaveCustomerAddresses(customerAddresses, dbContext);
        Console.WriteLine("Customer addresses saved");
        
        sapApiClient.LogoutSAPRestApi(cookieData);
        Console.WriteLine("Logged out");
        
        // List<SAPData> ungeocoded;
        // ungeocoded = await dbContext.SAPData
        //     .Where(s => !dbContext.PostGeocodeData.Any(g => g.DocNum == s.DocNum))
        //     .ToListAsync();

        // List<PostGeocodeData?> results = await geocodeService.GetGeocodeDataBatchAsync(ungeocoded, dbContext);
        //
        // foreach (var result in results.Where(r => r != null))
        // {
        //     Console.WriteLine($"Geocode for DocNum: {result!.DocNum} — {result.Latitude}, {result.Longitude}");
        // }
        
    }

}