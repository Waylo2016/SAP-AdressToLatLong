using Microsoft.EntityFrameworkCore;
using SAP_AdresToLatLong.Helpers;
using Toolbelt.ComponentModel.DataAnnotations;
using SAP_AdresToLatLong.Models;

namespace SAP_AdresToLatLong.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext (options)

{ 
    public DbSet<SAPData> SAPData { get; set; }
    public DbSet<PostGeocodeData> PostGeocodeData { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.BuildDecimalColumnTypeFromAnnotations();
        
        modelBuilder.Entity<PostGeocodeData>()
            .HasKey(p => p.DocNum);

        modelBuilder.Entity<SAPData>()
            .HasKey(s => s.DocNum);

        modelBuilder.Entity<SAPData>()
            .HasOne(s => s.PostGeocodeData)
            .WithOne(p => p.SAPData)
            .HasForeignKey<PostGeocodeData>(p => p.DocNum);
    }

}