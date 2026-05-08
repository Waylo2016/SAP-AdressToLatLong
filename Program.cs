using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Helpers;

namespace SAP_AdresToLatLong;

class Program
{
    static void Main(string[] args)
    {
        var connectionString = SecretParser.Build();
        
        using var dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options);
        
    }

}