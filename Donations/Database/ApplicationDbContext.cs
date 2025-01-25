using Donations.Entities.Common;
using Donations.Entities.Medical;
using Donations.Entities.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Donations.Database;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Donor> Donors { get; set; }
    public DbSet<DonationCenter> DonationCenters { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<BloodRequest> BloodRequests { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region User/role table configuration

        // This is for our convenience so that we don't have to manually look up the donor or donation center entity belonging to the user's ID every time we need to access it
        modelBuilder.Entity<User>()
               .HasOne(u => u.Donor)
               .WithOne(d => d.User)
               .HasForeignKey<Donor>(d => d.UserId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.DonationCenter)
            .WithOne(d => d.User)
            .HasForeignKey<DonationCenter>(d => d.UserId);

        modelBuilder.Entity<User>().Navigation(d => d.Donor).AutoInclude();
        modelBuilder.Entity<User>().Navigation(d => d.DonationCenter).AutoInclude();
        modelBuilder.Entity<Donor>().Navigation(d => d.Location).AutoInclude();
        modelBuilder.Entity<DonationCenter>().Navigation(d => d.Location).AutoInclude();
        #endregion

        #region Blood Requests configuration
        modelBuilder.Entity<DonationCenter>()
            .HasMany(dc => dc.BloodRequests)
            .WithOne(br => br.DonationCenter)
            .HasForeignKey(br => br.DonationCenterId)
            .OnDelete(DeleteBehavior.NoAction); // requests won't be auto-deleted when the center is but that won't happen anyway

        modelBuilder.Entity<BloodRequest>()
            .Property(br => br.BloodTypesString)
            .HasColumnName("BloodTypes"); // this is to keep the column name meaningful instead of an ugly BloodTypesString (a technical detail we don't need to leak)
        
        modelBuilder.Entity<Donor>()
            .HasMany(br => br.Appointments)
            .WithOne(a => a.Donor)
            .HasForeignKey(a => a.DonorId)
            .OnDelete(DeleteBehavior.NoAction); // delete appointments when the request is deleted
            
        #endregion
    }
}