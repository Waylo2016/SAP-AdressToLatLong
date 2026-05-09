using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Helpers;
using SAP_AdresToLatLong.Models;
using SAP_AdresToLatLong.Services;

namespace SAP_AdresToLatLong;

class Program
{
    static void Main(string[] args)
    {
        
        SapFunctions sapFunctions = new SapFunctions();
        
        var connectionString = SecretParser.Build();
        
        using var dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options
        );
        
        
        SapLoginData loginData = sapFunctions.LoginSAPRestApi(SecretParser.GetUsername(), SecretParser.GetPassword(), Environment.GetEnvironmentVariable("SAP_SERVER")!);
        
        
    }

}