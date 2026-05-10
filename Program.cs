using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Helpers;
using SAP_AdresToLatLong.Models;
using SAP_AdresToLatLong.Services;

namespace SAP_AdresToLatLong;

class Program
{
    async static Task Main(string[] args)
    {
        
        SapFunctions sapFunctions = new SapFunctions();
        GoogleFunctions googleFunctions = new GoogleFunctions();
        
        var connectionString = SecretParser.Build();

        await using var dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options
        );

        var loginData = new SapLoginData
        {
            CompanyDB = Environment.GetEnvironmentVariable("SAP_SERVER")!,
            UserName = SecretParser.GetUsername(),
            Password = SecretParser.GetPassword()
        };
        
        
        SapCookieData cookieData = sapFunctions.LoginSAPRestApi(loginData);
        Console.WriteLine("Login successful");

        
        List<SAPData>? customerAddresses = sapFunctions.GetCustomerAddresses(loginData, cookieData);
        Console.WriteLine("Customer addresses have been got");
        
        sapFunctions.SaveCustomerAddresses(customerAddresses!, dbContext);
        Console.WriteLine("Customer addresses saved");
        
        var allSapData = dbContext.SAPData.ToList(); // Or fetch in chunks if 50k is too many for memory

        foreach (var sapItem in allSapData)
        {
            // 1. Check if we already have this data to save money/time
            if (googleFunctions.CheckIfGeocodeDataExists(sapItem.DocNum, sapItem.CardCode, dbContext))
            {
                continue; 
            }

            // 2. Call the API asynchronously
            var geocodeResult = await googleFunctions.GetGeocodeDataAsync(sapItem, dbContext);

            Console.WriteLine($"Geocode for DocNum: {sapItem.DocNum} completed: {geocodeResult.Latitude.ToString()}, {geocodeResult.Longitude.ToString()}");
            if (geocodeResult != null)
            {
                // 3. Save result
                await googleFunctions.SaveGeocodeDataAsync(geocodeResult, dbContext);
                Console.WriteLine($"Saved geocode for DocNum: {sapItem.DocNum}");
            }
            
        }
        
        sapFunctions.LogoutSAPRestApi(cookieData);
        Console.WriteLine("Logged out");
        
        
        
    }

}