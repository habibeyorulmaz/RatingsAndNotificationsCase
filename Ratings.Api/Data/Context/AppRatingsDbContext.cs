using Microsoft.EntityFrameworkCore;
using Ratings.Api.Data.Entities;

namespace Ratings.Api.Data.Context;

public class AppRatingsDbContext : DbContext
{
    public AppRatingsDbContext(DbContextOptions<AppRatingsDbContext> options) : base(options)
    {
    }

    public DbSet<Rating> Ratings { get; set; }
}