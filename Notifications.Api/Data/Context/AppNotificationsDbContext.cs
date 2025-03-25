using Microsoft.EntityFrameworkCore;
using Notifications.Api.Data.Entities;

namespace Notifications.Api.Data.Context;

public class AppNotificationsDbContext : DbContext
{
    public AppNotificationsDbContext(DbContextOptions<AppNotificationsDbContext> options) : base(options)
    {
    }

    public DbSet<Notification> Notifications { get; set; }
}