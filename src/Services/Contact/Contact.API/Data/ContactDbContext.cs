using Contact.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contact.API.Data;

public class ContactDbContext : DbContext
{
    public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
    {
    }

    public DbSet<Entities.Contact> Contacts { get; set; }
    public DbSet<ContactInformation> ContactInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Entities.Contact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Surname).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Company).HasMaxLength(200);

            entity.HasMany(e => e.ContactInformations)
                  .WithOne(ci => ci.Contact)
                  .HasForeignKey(ci => ci.ContactId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ContactInformation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.InfoType).IsRequired();
            entity.Property(e => e.InfoContent).IsRequired().HasMaxLength(500);
        });
    }
}
