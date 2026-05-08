using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Helpers;

namespace SAP_AdresToLatLong.Factories;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options =  new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(SecretParser.Build())
            .Options;

        return new ApplicationDbContext(options);
    }
}