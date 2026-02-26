using Microsoft.EntityFrameworkCore;

namespace Report.API.Data;

public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
    {
    }

    public DbSet<Entities.Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Entities.Report>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.RequestedLocation).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.RequestedAt).IsRequired();
        });
    }
}
