using Donations.Database;
using Donations.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Donations.Services;

public class LocationService
{
    private readonly ApplicationDbContext _dbContext;

    public LocationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Location>> GetAllLocations()
    {
        return await _dbContext.Locations.ToListAsync();
    }
    
    public async Task AddLocation(Location location)
    {
        _dbContext.Locations.Add(location);
        await _dbContext.SaveChangesAsync();
    }
}