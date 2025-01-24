using Donations.Entities.Common;
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
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        #region User/role table configuration
        modelBuilder.Entity<Donor>().Navigation(d => d.User).AutoInclude();
        modelBuilder.Entity<Donor>().Navigation(d => d.Location).AutoInclude();
        modelBuilder.Entity<DonationCenter>().Navigation(d => d.User).AutoInclude();
        modelBuilder.Entity<DonationCenter>().Navigation(d => d.Location).AutoInclude();
        #endregion
    }
}