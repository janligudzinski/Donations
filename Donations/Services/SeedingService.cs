using Donations.Database;
using Donations.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Donations.Services;

public class SeedingService
{
    private readonly ApplicationDbContext _dbContext;

    public SeedingService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private async Task SeedLocations()
    {
        var citiesAlreadyExist = await _dbContext.Locations.AnyAsync();
        if (citiesAlreadyExist)
        {
            return;
        }
        Console.WriteLine("Seeding initial locations...");

        var locations = new List<Location>
        {
            new Location
            {
                Name = "Amman",
                Latitude = 31.9454,
                Longitude = 35.9284
            },
            new Location
            {
                Name = "Zarqa",
                Latitude = 32.0728,
                Longitude = 36.0894
            },
            new Location
            {
                Name = "Irbid",
                Latitude = 32.5556,
                Longitude = 35.8500
            },
            new Location
            {
                Name = "Aqaba",
                Latitude = 29.5267,
                Longitude = 35.0078
            },
            new Location
            {
                Name = "Jerash",
                Latitude = 32.2808,
                Longitude = 35.8982
            },
            new Location
            {
                Name = "Karak",
                Latitude = 31.1851,
                Longitude = 35.7047
            },
        };
        await _dbContext.Locations.AddRangeAsync(locations);
        await _dbContext.SaveChangesAsync();
        Console.WriteLine("Locations seeded successfully");
    }


    public async Task Seed()
    {
        await SeedLocations();
    }
}
